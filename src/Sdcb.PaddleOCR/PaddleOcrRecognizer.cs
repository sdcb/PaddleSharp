﻿using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Sdcb.PaddleOCR
{
    public class PaddleOcrRecognizer : IDisposable
    {
        private readonly PaddlePredictor _p;

        public RecognizationModel Model { get; init; }

        public PaddleOcrRecognizer(RecognizationModel model)
        {
            Model = model;
            _p = Model.CreateConfig().CreatePredictor();
        }

        public PaddleOcrRecognizer Clone() => new PaddleOcrRecognizer(Model);

        public void Dispose() => _p.Dispose();

        public PaddleOcrRecognizerResult[] Run(Mat[] srcs, int batchSize = 0)
        {
            if (srcs.Length == 0)
            {
                return new PaddleOcrRecognizerResult[0];
            }

            int chooseBatchSize = batchSize != 0 ? batchSize : Math.Min(8, Environment.ProcessorCount);
            PaddleOcrRecognizerResult[] allResult = new PaddleOcrRecognizerResult[srcs.Length];

            return srcs
                .Select((x, i) => (mat: x, i))
                .OrderBy(x => x.mat.Width)
                .Chunk(chooseBatchSize)
                .Select(x => (result: RunMulti(x.Select(x => x.mat).ToArray()), ids: x.Select(x => x.i).ToArray()))
                .SelectMany(x => x.result.Zip(x.ids, (result, i) => (result, i)))
                .OrderBy(x => x.i)
                .Select(x => x.result)
                .ToArray();
        }

        public PaddleOcrRecognizerResult Run(Mat src) => RunMulti(new[] { src }).Single();

        private PaddleOcrRecognizerResult[] RunMulti(Mat[] srcs)
        {
            if (srcs.Length == 0)
            {
                return new PaddleOcrRecognizerResult[0];
            }

            for (int i = 0; i < srcs.Length; ++i)
            {
                Mat src = srcs[i];
                if (src.Empty())
                {
                    throw new ArgumentException($"src[{i}] size should not be 0, wrong input picture provided?");
                }
            }

            int modelHeight = Model.Shape.height;
            int maxWidth = (int)Math.Ceiling(srcs.Max(src =>
            {
                Size size = src.Size();
                return 1.0 * size.Width / size.Height * modelHeight;
            }));

            Mat[] normalizeds = srcs
                .Select(src =>
                {
                    using Mat channel3 = src.Channels() switch
                    {
                        4 => src.CvtColor(ColorConversionCodes.RGBA2BGR),
                        1 => src.CvtColor(ColorConversionCodes.GRAY2RGB),
                        3 => src,
                        var x => throw new Exception($"Unexpect src channel: {x}, allow: (1/3/4)")
                    };
                    using Mat resized = ResizePadding(channel3, modelHeight, maxWidth);
                    return Normalize(resized);
                })
                .ToArray();

            using (PaddleTensor input = _p.GetInputTensor(_p.InputNames[0]))
            {
                int channel = normalizeds[0].Channels();
                input.Shape = new[] { normalizeds.Length, channel, modelHeight, maxWidth };
                float[] data = ExtractMat(normalizeds, channel, modelHeight, maxWidth);
                input.SetData(data);
            }
            if (!_p.Run())
            {
                throw new Exception($"PaddlePredictor(Recognizer) run failed.");
            }

            using (PaddleTensor output = _p.GetOutputTensor(_p.OutputNames[0]))
            {
                float[] data = output.GetData<float>();
                int[] shape = output.Shape;

                GCHandle dataHandle = default;
                try
                {
                    dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                    IntPtr dataPtr = dataHandle.AddrOfPinnedObject();
                    int labelCount = shape[2];
                    int charCount = shape[1];

                    return Enumerable.Range(0, shape[0])
                        .Select(i =>
                        {
                            StringBuilder sb = new();
                            int lastIndex = 0;
                            float score = 0;
                            for (int n = 0; n < charCount; ++n)
                            {
                                using Mat mat = new Mat(1, labelCount, MatType.CV_32FC1, dataPtr + (n + i * charCount) * labelCount * sizeof(float));
                                int[] maxIdx = new int[2];
                                mat.MinMaxIdx(out double _, out double maxVal, new int[0], maxIdx);

                                if (maxIdx[1] > 0 && (!(n > 0 && maxIdx[1] == lastIndex)))
                                {
                                    score += (float)maxVal;
                                    sb.Append(Model.GetLabelByIndex(maxIdx[1]));
                                }
                                lastIndex = maxIdx[1];
                            }

                            return new PaddleOcrRecognizerResult(sb.ToString(), score / sb.Length);
                        })
                        .ToArray();
                }
                finally
                {
                    dataHandle.Free();
                }
            }
        }

        private static Mat ResizePadding(Mat src, int height, int targetWidth)
        {
            Size size = src.Size();
            float whRatio = 1.0f * size.Width / size.Height;
            int width = (int)Math.Ceiling(height * whRatio);

            if (width == targetWidth)
            {
                return src.Resize(new Size(width, height));
            }
            else
            {
                using Mat resized = src.Resize(new Size(width, height));
                return resized.CopyMakeBorder(0, 0, 0, targetWidth - width, BorderTypes.Constant, Scalar.Gray);
            }
        }

        private static Mat Normalize(Mat src)
        {
            using Mat normalized = new();
            src.ConvertTo(normalized, MatType.CV_32FC3, 1.0 / 255);
            Mat[] bgr = normalized.Split();
            float[] scales = new[] { 2.0f, 2.0f, 2.0f };
            float[] means = new[] { 0.5f, 0.5f, 0.5f };
            for (int i = 0; i < bgr.Length; ++i)
            {
                bgr[i].ConvertTo(bgr[i], MatType.CV_32FC1, 1.0 * scales[i], (0.0 - means[i]) * scales[i]);
            }

            Mat dest = new();
            Cv2.Merge(bgr, dest);

            foreach (Mat channel in bgr)
            {
                channel.Dispose();
            }

            return dest;
        }

        private static float[] ExtractMat(Mat[] srcs, int channel, int height, int width)
        {
            float[] result = new float[srcs.Length * channel * width * height];
            GCHandle resultHandle = GCHandle.Alloc(result, GCHandleType.Pinned);
            IntPtr resultPtr = resultHandle.AddrOfPinnedObject();
            try
            {
                for (int i = 0; i < srcs.Length; ++i)
                {
                    Mat src = srcs[i];
                    if (src.Channels() != channel)
                    {
                        throw new Exception($"src[{i}] channel={src.Channels()}, expected {channel}");
                    }
                    for (int c = 0; c < channel; ++c)
                    {
                        using Mat dest = new Mat(height, width, MatType.CV_32FC1, resultPtr + (c + i * channel) * height * width * sizeof(float));
                        Cv2.ExtractChannel(src, dest, c);
                    }
                }
                return result;
            }
            finally
            {
                resultHandle.Free();
            }
        }
    }
}

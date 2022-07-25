using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

        public PaddleOcrRecognizerResult Run(Mat src)
        {
            if (src.Empty())
            {
                throw new ArgumentException("src size should not be 0, wrong input picture provided?");
            }

            if (!(src.Channels() switch { 3 or 1 => true, _ => false }))
            {
                throw new NotSupportedException($"{nameof(src)} channel must be 3 or 1, provided {src.Channels()}.");
            }

            using Mat resized = ResizePadding(src, Model.Shape.height);
            using Mat normalized = Normalize(resized);

            using (PaddleTensor input = _p.GetInputTensor(_p.InputNames[0]))
            {
                input.Shape = new[] { 1, 3, normalized.Rows, normalized.Cols };
                float[] data = PaddleOcrDetector.ExtractMat(normalized);
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

                var sb = new StringBuilder();
                int lastIndex = 0;
                float score = 0;

                GCHandle dataHandle = default;
                try
                {
                    dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                    IntPtr dataPtr = dataHandle.AddrOfPinnedObject();
                    int len = shape[2];

                    for (int n = 0; n < shape[1]; ++n)
                    {
                        using Mat mat = new Mat(1, len, MatType.CV_32FC1, dataPtr + n * len * sizeof(float));
                        int[] maxIdx = new int[2];
                        mat.MinMaxIdx(out double _, out double maxVal, new int[0], maxIdx);

                        if (maxIdx[1] > 0 && (!(n > 0 && maxIdx[1] == lastIndex)))
                        {
                            score += (float)maxVal;
                            sb.Append(Model.GetLabelByIndex(maxIdx[1]));
                        }
                        lastIndex = maxIdx[1];
                    }
                }
                finally
                {
                    dataHandle.Free();
                }

                return new(sb.ToString(), score / sb.Length);
            }
        }

        private static Mat ResizePadding(Mat src, int height)
        {
            Size size = src.Size();
            float whRatio = 1.0f * size.Width / size.Height;
            int width = (int)Math.Ceiling(height * whRatio);

            return src.Resize(new Size(width, height));
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
    }
}

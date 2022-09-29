using OpenCvSharp;
using System.Runtime.InteropServices;
using System;
using Sdcb.PaddleInference;
using System.Linq;

namespace Sdcb.RotationDetector
{
    public class PaddleRotationDetector : IDisposable
    {
        private readonly RotationDetectionModel _model;
        private readonly PaddlePredictor _p;

        public InputShape Shape => _model.Shape;

        public PaddleRotationDetector(RotationDetectionModel model)
        {
            _model = model;
            _p = model.CreateConfig().CreatePredictor();
        }

        public PaddleRotationDetector Clone() => new PaddleRotationDetector(_model);

        public void Dispose() => _p.Dispose();

        public RotationResult Run(Mat src, float rotateThreshold = 0.50f)
        {
            if (src.Empty())
            {
                throw new ArgumentException("src size should not be 0, wrong input picture provided?");
            }

            if (!(src.Channels() switch { 3 or 1 => true, _ => false }))
            {
                throw new NotSupportedException($"{nameof(src)} channel must be 3 or 1, provided {src.Channels()}.");
            }

            using Mat resized = ResizePadding(src, Shape);
            using Mat normalized = Normalize(resized);

            using (PaddleTensor input = _p.GetInputTensor(_p.InputNames[0]))
            {
                input.Shape = new[] { 1, 3, normalized.Rows, normalized.Cols };
                float[] data = ExtractMat(normalized);
                input.SetData(data);
            }
            if (!_p.Run())
            {
                throw new Exception("PaddlePredictor(Classifier) run failed.");
            }

            using (PaddleTensor output = _p.GetOutputTensor(_p.OutputNames[0]))
            {
                float[] softmax = output.GetData<float>();
                float max = softmax.Max();
                int maxIndex = Array.IndexOf(softmax, max);
                if (max > rotateThreshold)
                {
                    return new RotationResult((RotationDegree)maxIndex, softmax[maxIndex]);
                }
                else
                {
                    return new RotationResult(RotationDegree._0, softmax[0]);
                }
            }
        }

        internal static float[] ExtractMat(Mat src)
        {
            int rows = src.Rows;
            int cols = src.Cols;
            float[] result = new float[rows * cols * 3];
            GCHandle resultHandle = default;
            try
            {
                resultHandle = GCHandle.Alloc(result, GCHandleType.Pinned);
                IntPtr resultPtr = resultHandle.AddrOfPinnedObject();
                for (int i = 0; i < src.Channels(); ++i)
                {
                    using Mat dest = new Mat(rows, cols, MatType.CV_32FC1, resultPtr + i * rows * cols * sizeof(float));
                    Cv2.ExtractChannel(src, dest, i);
                }
            }
            finally
            {
                resultHandle.Free();
            }
            return result;
        }

        private static Mat ResizePadding(Mat src, InputShape shape)
        {
            OpenCvSharp.Size srcSize = src.Size();
            using Mat roi = srcSize.Width / srcSize.Height > shape.width / shape.height ?
                src[0, srcSize.Height, 0, (int)Math.Floor(1.0 * srcSize.Height * shape.width / shape.height)] :
                src.Clone();
            double scaleRate = 1.0 * shape.height / srcSize.Height;
            Mat resized = roi.Resize(new OpenCvSharp.Size(Math.Floor(roi.Width * scaleRate), shape.height));
            if (resized.Width < shape.width)
            {
                Cv2.CopyMakeBorder(resized, resized, 0, 0, 0, shape.width - resized.Width, BorderTypes.Constant, Scalar.Black);
            }
            return resized;
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
using System.Runtime.InteropServices;
using OpenCvSharp;
using Sdcb.PaddleInference;

namespace Sdcb.PaddleSeg
{
    /// <summary>
    /// Handles image segmentation prediction using PaddlePaddle inference
    /// </summary>
    internal class PaddleSegPredictor : IDisposable
    {
        private readonly PaddlePredictor _p;

        /// <summary>
        /// Initializes a new instance of PaddleSegPredictor
        /// </summary>
        /// <param name="model">The segmentation model to use</param>
        /// <param name="configure">Optional configuration action for PaddleConfig</param>
        public PaddleSegPredictor(SegModel model, Action<PaddleConfig>? configure = null)
        {
            PaddleConfig c = model.CreateConfig();
            model.ConfigureDevice(c, configure);

            _p = c.CreatePredictor();
        }

        /// <summary>
        /// Performs segmentation on the input image
        /// </summary>
        /// <param name="img">Input image in BGR format</param>
        /// <returns>Array of segmentation labels where each element represents the class ID for a pixel</returns>
        public int[] Run(Mat img)
        {
            // Convert BGR to RGB color space
            Cv2.CvtColor(img, img, ColorConversionCodes.BGR2RGB);
            
            // Normalize the image
            Mat mat = Normalize(img);
            
            using PaddlePredictor predictor = _p.Clone();
            using (PaddleTensor input = predictor.GetInputTensor(predictor.InputNames[0]))
            {
                // Set input shape: [batch_size=1, channels=3, height, width]
                input.Shape = new[] { 1, 3, mat.Rows, mat.Cols };
                float[] data = ExtractMat(mat);
                input.SetData(data);
            }
            
            if (!predictor.Run())
            {
                throw new Exception("PaddlePredictor(Detector) run failed.");
            }
            
            using (PaddleTensor output = predictor.GetOutputTensor(predictor.OutputNames[0]))
            {
                int[] data = output.GetData<int>();
                int[] shape = output.Shape;
                return data;
            }
        }

        /// <summary>
        /// Normalizes the input image by:
        /// 1. Converting to float32 and scaling to [0,1]
        /// 2. Applying formula: (x - 0.5) / 0.5
        /// </summary>
        /// <param name="src">Source image</param>
        /// <returns>Normalized image</returns>
        private static Mat Normalize(Mat src)
        {
            Mat imFloat = new Mat();
            src.ConvertTo(imFloat, MatType.CV_32FC3, 1.0 / 255.0);

            imFloat = (imFloat - (Scalar)0.5) / 0.5;

            return imFloat;
        }

        /// <summary>
        /// Extracts image data into a float array in CHW (Channel, Height, Width) format
        /// required by PaddlePaddle
        /// </summary>
        /// <param name="src">Source image with 3 channels</param>
        /// <returns>Float array containing image data in CHW format</returns>
        private static float[] ExtractMat(Mat src)
        {
            int rows = src.Rows;
            int cols = src.Cols;
            float[] result = new float[rows * cols * 3];
            GCHandle resultHandle = default;
            try
            {
                resultHandle = GCHandle.Alloc(result, GCHandleType.Pinned);
                IntPtr resultPtr = resultHandle.AddrOfPinnedObject();
                
                // Extract each channel and store in CHW format
                for (int i = 0; i < src.Channels(); ++i)
                {
                    // Convert BGR to RGB order
                    int rgb = 2 - i;
                    using Mat dest = Mat.FromPixelData(rows, cols, MatType.CV_32FC1, resultPtr + i * rows * cols * sizeof(float));
                    Cv2.ExtractChannel(src, dest, rgb);
                }
            }
            finally
            {
                resultHandle.Free();
            }
            return result;
        }

        /// <summary>
        /// Disposes the underlying PaddlePredictor
        /// </summary>
        public void Dispose()
        {
            _p.Dispose();
        }
    }
}
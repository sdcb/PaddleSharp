using OpenCvSharp;
using System;
using System.Runtime.InteropServices;

namespace Sdcb.PaddleDetection.Preprocesses;

internal class PermuteOperation : PreprocessOperation
	{
		public override string Name => PreprocessOperation.Permute;

		public override void Run(Mat src, ImageProcessContext data)
		{
			Size size = src.Size();
			int channels = src.Channels();
			float[] result = new float[size.Width * size.Height * channels];
			GCHandle resultHandle = default;
			try
			{
				resultHandle = GCHandle.Alloc(result, GCHandleType.Pinned);
				IntPtr resultPtr = resultHandle.AddrOfPinnedObject();
				for (int i = 0; i < channels; ++i)
				{
					using Mat cmat = Mat.FromPixelData(src.Height, src.Width, MatType.CV_32FC1, resultPtr + i * size.Width * size.Height * sizeof(float));
					Cv2.ExtractChannel(src, cmat, i);
				}

				data.Data = result;
			}
			finally
			{
				resultHandle.Free();
			}
		}
	}
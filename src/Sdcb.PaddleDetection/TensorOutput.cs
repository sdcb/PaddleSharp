using Sdcb.PaddleInference;
using System;

namespace Sdcb.PaddleDetection;

internal struct TensorOutput
	{
		public int[] Shape;
		public PaddleDataType DataType;
		public string Name;
		public Array Data;
	}
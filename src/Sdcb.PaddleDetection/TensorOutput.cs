using Sdcb.PaddleInference;
using System;

namespace Sdcb.PaddleDetection
{
    internal struct TensorOutput
	{
		public int[] Shape;
		public DataTypes DataType;
		public string Name;
		public Array Data;
	}

}
using System.Collections.Generic;

namespace Sdcb.PaddleInference.TensorRt
{
    public record struct TensorRtShape(int _0, int _1, int _2, int _3)
    {
        public int[] ToArray() => new int[] { _0, _1, _2, _3 };
    }
}

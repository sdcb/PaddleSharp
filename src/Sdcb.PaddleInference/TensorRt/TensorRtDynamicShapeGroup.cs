namespace Sdcb.PaddleInference.TensorRt;

public record TensorRtDynamicShapeGroup(int[] Min, int[] Max, int[] Optimal);

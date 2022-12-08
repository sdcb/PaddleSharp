namespace Sdcb.PaddleInference.TensorRt
{
    public record TensorRtDynamicShapeGroup(TensorRtShape Min, TensorRtShape Max, TensorRtShape Opt);
}

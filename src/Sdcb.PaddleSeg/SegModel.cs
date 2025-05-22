using Sdcb.PaddleInference;

namespace Sdcb.PaddleSeg
{
    public abstract class SegModel : BaseModel
    {
        public static SegModel FromDirectory(string directoryPath) => new FileSegModel(directoryPath);

        public override Action<PaddleConfig> DefaultDevice => PaddleDevice.Mkldnn();
    }
}
using Sdcb.RotationDetector;
using Sdcb.PaddleInference;

namespace Sdcb.RotationDetector;

public abstract class RotationDetectionModel
{
    public abstract InputShape Shape { get; }
    public abstract PaddleConfig CreateConfig();

    public static InputShape DefaultShape = new(3, 224, 224);

    public static RotationDetectionModel FromDirectory(string directoryPath) => new FileRotationDetectionModel(directoryPath);

    public static RotationDetectionModel EmbeddedDefault => new EmbeddedResourceDetectionModel();
}

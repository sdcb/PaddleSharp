using Sdcb.PaddleInference;

namespace Sdcb.RotationDetector;

/// <summary>
/// A base abstract class for rotation detection models.
/// </summary>
public abstract class RotationDetectionModel
{
    /// <summary>
    /// Gets the shape of the input.
    /// </summary>
    public abstract InputShape Shape { get; }

    /// <summary>
    /// Creates a PaddleConfig.
    /// </summary>
    /// <returns>A PaddleConfig instance suitable for the model.</returns>
    public abstract PaddleConfig CreateConfig();

    /// <summary>
    /// The default shape of the input.
    /// </summary>
    public static InputShape DefaultShape = new(3, 224, 224);

    /// <summary>
    /// Load a RotationDetectionModel from a directory.
    /// </summary>
    /// <param name="directoryPath">The full path to the directory where the model resides.</param>
    /// <returns>A rotation detection model.</returns>
    public static RotationDetectionModel FromDirectory(string directoryPath) => new FileRotationDetectionModel(directoryPath);

    /// <summary>
    /// An embedded resource rotation detection model.
    /// </summary>
    public static RotationDetectionModel EmbeddedDefault => new EmbeddedResourceDetectionModel();
}

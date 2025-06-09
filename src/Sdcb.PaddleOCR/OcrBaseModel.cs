using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models;
using System;

namespace Sdcb.PaddleOCR;

/// <summary>
/// Represents an abstract base class for OCR models.
/// </summary>
public abstract class OcrBaseModel
{
    /// <summary>
    /// Constructor for initializing an instance of the <see cref="OcrBaseModel"/> class.
    /// </summary>
    /// <param name="version">The version of model.</param>
    public OcrBaseModel(ModelVersion version)
    {
        Version = version;
    }

    /// <summary>
    /// Gets the version of the OCR model.
    /// </summary>
    public ModelVersion Version { get; }

    /// <summary>
    /// Create the paddle config for the model.
    /// </summary>
    /// <returns>The paddle config for model.</returns>
    public abstract PaddleConfig CreateConfig();

    /// <summary>
    /// Gets the default device for the model.
    /// </summary>
    public abstract Action<PaddleConfig> DefaultDevice { get; }

    /// <summary>
    /// Configure the device related config of the <see cref="PaddleConfig"/>.
    /// </summary>
    /// <param name="configure">The device and configure of the <see cref="PaddleConfig"/>, pass null to using model's <see cref="DefaultDevice"/>.</param>
    /// <param name="config">The PaddleConfig to modify.</param>
    public virtual void ConfigureDevice(PaddleConfig config, Action<PaddleConfig>? configure = null)
    {
        config.Apply((configure ?? DefaultDevice).And(c =>
        {
            if (Version == ModelVersion.V5)
            {
                //c.NewIREnabled = true;
                c.NewExecutorEnabled = true;
            }
        }));
    }
}

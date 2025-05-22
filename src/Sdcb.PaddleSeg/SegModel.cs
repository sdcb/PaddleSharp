using Sdcb.PaddleInference;

namespace Sdcb.PaddleSeg
{
    public abstract class SegModel
    {
        /// <summary>
        /// Create the paddle config for the model.
        /// </summary>
        /// <returns>The paddle config for model.</returns>
        public abstract PaddleConfig CreateConfig();

        /// <summary>
        /// Configure the device related config of the <see cref="PaddleConfig"/>.
        /// </summary>
        /// <param name="configure">The device and configure of the <see cref="PaddleConfig"/>, pass null to using model's <see cref="DefaultDevice"/>.</param>
        /// <param name="config">The PaddleConfig to modify.</param>
        public virtual void ConfigureDevice(PaddleConfig config, Action<PaddleConfig>? configure = null)
        {
            config.Apply(configure ?? DefaultDevice);
        }

        /// <summary>
        /// Create a <see cref="SegModel"/> from the directory path.
        /// </summary>
        public static SegModel FromDirectory(string directoryPath) => new FileSegModel(directoryPath);

        /// <summary>
        /// Default device configuration for the model.
        /// </summary>
        public virtual Action<PaddleConfig> DefaultDevice => PaddleDevice.Mkldnn();
    }
}
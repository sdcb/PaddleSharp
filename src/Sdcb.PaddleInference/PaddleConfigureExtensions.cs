using System;

namespace Sdcb.PaddleInference;

/// <summary>
/// A static class containing extension methods for configuring a <see cref="PaddleConfig"/> object.
/// </summary>
public static class PaddleConfigureExtensions
{
    /// <summary>
    /// Combines two <see cref="Action"/>s that configure a <see cref="PaddleConfig"/> object.
    /// </summary>
    /// <param name="action1">The first <see cref="Action"/>.</param>
    /// <param name="action2">The second <see cref="Action"/>.</param>
    /// <returns>A combined <see cref="Action"/> that can configure a <see cref="PaddleConfig"/> object.</returns>
    public static Action<PaddleConfig> And(this Action<PaddleConfig> action1, Action<PaddleConfig> action2)
    {
        return cfg =>
        {
            action1(cfg);
            action2(cfg);
        };
    }
}

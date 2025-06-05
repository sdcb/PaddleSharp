using System.Collections.Generic;

namespace Sdcb.PaddleOCR.Models.LocalV5;

/// <summary>
/// Contains known models for local version 4
/// </summary>
public static class KnownModels
{
    /// <summary>
    /// HashSet containing all the model names
    /// </summary>
    public static HashSet<string> All = new(new[]
    {
        "mobile-zh-det",
        "mobile-zh-rec",
    });
}

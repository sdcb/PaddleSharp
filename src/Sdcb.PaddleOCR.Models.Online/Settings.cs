using System;
using System.IO;

namespace Sdcb.PaddleOCR.Models.Online;

/// <summary>
/// A static class that contains settings for PaddleOCR Models.
/// </summary>
public static class Settings
{
    /// <summary>
    /// The directory where PaddleOCR Models are saved.
    /// </summary>
    public static string GlobalModelDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "paddleocr-models");
}

using Sdcb.PaddleOCR.Models;
using SharpCompress.Archives;
using System;
using System.IO;

namespace Sdcb.PaddleOCR.KnownModels
{
    public class Settings
    {
        public static string GlobalModelDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "paddleocr-models");
    }
}

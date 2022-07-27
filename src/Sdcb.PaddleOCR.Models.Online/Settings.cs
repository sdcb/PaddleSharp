using System;
using System.IO;

namespace Sdcb.PaddleOCR.Models.Online
{
    public class Settings
    {
        public static string GlobalModelDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "paddleocr-models");
    }
}

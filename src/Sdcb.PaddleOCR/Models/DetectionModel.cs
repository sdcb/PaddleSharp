using Sdcb.PaddleInference;
using Sdcb.PaddleInference.TensorRt;
using Sdcb.PaddleOCR.Models.Details;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sdcb.PaddleOCR.Models
{
    public abstract class DetectionModel
    {
        public abstract PaddleConfig CreateConfig();

        public static DetectionModel FromDirectory(string directoryPath) => new FileDetectionModel(directoryPath);
    }
}

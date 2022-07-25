using Sdcb.PaddleInference;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sdcb.PaddleOCR.Models.Details
{
    public class FileRecognizationModel : RecognizationModel
    {
        private readonly IReadOnlyList<string> _labels;
        private ModelVersion Version { get; init; }

        public string DirectoryPath { get; init; }

        public FileRecognizationModel(string directoryPath, string labelFilePath, ModelVersion version)
        {
            DirectoryPath = directoryPath;
            _labels = File.ReadAllLines(labelFilePath);
            Version = version;
        }

        public override OcrShape Shape => Version switch
        {
            ModelVersion.V2 => new(3, 32, 320),
            ModelVersion.V3 => new(3, 48, 320),
            _ => throw new ArgumentOutOfRangeException($"Unknown OCR model version: {Version}."),
        };

        public override PaddleConfig CreateConfig()
        {
            PaddleConfig config = PaddleConfig.FromModelDir(DirectoryPath);
            if (Version == ModelVersion.V3)
            {
                config.DeletePass("matmul_transpose_reshape_fuse_pass");
            }
            return config;
        }

        public override string GetLabelByIndex(int i)
        {
            return i switch
            {
                var x when x > 0 && x <= _labels.Count => _labels[x - 1],
                var x when x == _labels.Count + 1 => " ",
                _ => throw new Exception($"Unable to GetLabelByIndex: index {i} out of range {_labels.Count}, OCR model or labels not matched?"), 
            };
        }
    }
}

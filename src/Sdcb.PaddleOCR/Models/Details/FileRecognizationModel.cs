using Sdcb.PaddleInference;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sdcb.PaddleOCR.Models.Details
{
    public class FileRecognizationModel : VersionedRecognizationModel
    {
        private readonly IReadOnlyList<string> _labels;

        public string DirectoryPath { get; }

        public FileRecognizationModel(string directoryPath, string labelFilePath, ModelVersion version) : base(version)
        {
            DirectoryPath = directoryPath;
            _labels = File.ReadAllLines(labelFilePath);
        }

        public override PaddleConfig CreateConfig()
        {
            PaddleConfig config = PaddleConfig.FromModelDir(DirectoryPath);
            ConfigPostProcess(config);
            return config;
        }

        public override string GetLabelByIndex(int i) => GetLabelByIndex(i, _labels);
    }

    public abstract class VersionedRecognizationModel : RecognizationModel
    {
        public VersionedRecognizationModel(ModelVersion version)
        {
            Version = version;
        }

        public ModelVersion Version { get; }

        public override OcrShape Shape => Version switch
        {
            ModelVersion.V2 => new(3, 32, 320),
            ModelVersion.V3 => new(3, 48, 320),
            _ => throw new ArgumentOutOfRangeException($"Unknown OCR model version: {Version}."),
        };

        protected static string GetLabelByIndex(int i, IReadOnlyList<string> labels)
        {
            return i switch
            {
                var x when x > 0 && x <= labels.Count => labels[x - 1],
                var x when x == labels.Count + 1 => " ",
                _ => throw new Exception($"Unable to GetLabelByIndex: index {i} out of range {labels.Count}, OCR model or labels not matched?"),
            };
        }

        protected void ConfigPostProcess(PaddleConfig config)
        {
            if (Version == ModelVersion.V3)
            {
                config.DeletePass("matmul_transpose_reshape_fuse_pass");
            }
        }
    }
}

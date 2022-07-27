using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Details;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdcb.PaddleOCR.Models.Online.Details
{
    internal class StreamDictFileRecognizationModel : VersionedRecognizationModel
    {
        private readonly IReadOnlyList<string> _labels;

        public string DirectoryPath { get; }

        public StreamDictFileRecognizationModel(string directoryPath, Stream dictStream, ModelVersion version) : base(version)
        {
            DirectoryPath = directoryPath;
            _labels = ReadLinesFromStream(dictStream).ToArray();
        }

        public static IEnumerable<string> ReadLinesFromStream(Stream stream)
        {
            using StreamReader reader = new(stream);
            while (!reader.EndOfStream)
            {
                yield return reader.ReadLine();
            }
        }

        public override PaddleConfig CreateConfig()
        {
            PaddleConfig config = PaddleConfig.FromModelDir(DirectoryPath);
            ConfigPostProcess(config);
            return config;
        }

        public override string GetLabelByIndex(int i) => GetLabelByIndex(i, _labels);
    }
}

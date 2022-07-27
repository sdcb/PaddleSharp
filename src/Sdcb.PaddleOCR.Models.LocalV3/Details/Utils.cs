using Sdcb.PaddleInference;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sdcb.PaddleOCR.Models.LocalV3.Details
{
    internal static class Utils
    {
        public readonly static Type RootType = typeof(LocalV3FullModels);
        public readonly static Assembly RootAssembly = typeof(LocalV3FullModels).Assembly;

        public static PaddleConfig LoadLocalModel(string key)
        {
            string ns = RootType.Namespace;
            byte[] programBuffer = ReadResourceAsBytes($"{ns}.models.{key}.pdmodel");
            byte[] paramsBuffer = ReadResourceAsBytes($"{ns}.models.{key}.pdiparams");
            return PaddleConfig.FromMemoryModel(programBuffer, paramsBuffer);
        }

        static byte[] ReadResourceAsBytes(string key)
        {
            using Stream stream = RootAssembly.GetManifestResourceStream(key);
            using MemoryStream ms = new ();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        public static IReadOnlyList<string> LoadDictsAsArray(string dictName)
        {
            string ns = RootType.Namespace;
            return ReadLinesFromStream(RootAssembly.GetManifestResourceStream($"{ns}.models.dicts.{dictName}")).ToArray();

            static IEnumerable<string> ReadLinesFromStream(Stream stream)
            {
                using StreamReader reader = new(stream);
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine();
                }
            }
        }
    }
}

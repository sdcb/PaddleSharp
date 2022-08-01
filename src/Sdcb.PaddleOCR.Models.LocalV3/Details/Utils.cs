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
        public readonly static Type RootType = typeof(LocalFullModels);
        public readonly static Assembly RootAssembly = typeof(LocalFullModels).Assembly;

        public static PaddleConfig LoadLocalModel(string key)
        {
            string ns = RootType.Namespace;
            byte[] programBuffer = ReadResourceAsBytes($"{ns}.models.{EmbeddedResourceTransform(key)}.inference.pdmodel");
            byte[] paramsBuffer = ReadResourceAsBytes($"{ns}.models.{EmbeddedResourceTransform(key)}.inference.pdiparams");
            return PaddleConfig.FromMemoryModel(programBuffer, paramsBuffer);
        }

        static byte[] ReadResourceAsBytes(string key)
        {
            using Stream? stream = RootAssembly.GetManifestResourceStream(key);
            if (stream == null)
            {
                throw new Exception($"Unable to load model embedded resource {key} from assembly, model not exists?");
            }

            using MemoryStream ms = new ();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        public static IReadOnlyList<string> LoadDictsAsArray(string dictName)
        {
            string ns = RootType.Namespace;
            string resourcePath = $"{ns}.models.dicts.{EmbeddedResourceTransform(dictName)}";
            using Stream? dictStream = RootAssembly.GetManifestResourceStream(resourcePath);
            if (dictStream == null)
            {
                throw new Exception($"Unable to load rec model dicts file embedded resource {resourcePath} from assembly , model not exists?");
            }

            return ReadLinesFromStream(dictStream).ToArray();

            static IEnumerable<string> ReadLinesFromStream(Stream stream)
            {
                using StreamReader reader = new(stream);
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine();
                }
            }
        }

        static string EmbeddedResourceTransform(string name) => name.Replace('-', '_').Replace(".0", "._0");
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sdcb.PaddleOCR.Models.Shared;

internal class SharedUtils
{
    public static List<string> LoadDicts(string dictName)
    {
        Type RootType = typeof(SharedUtils);
        Assembly RootAssembly = RootType.Assembly;

        string ns = RootType.Namespace;
        string resourcePath = $"{ns}.dicts.{EmbeddedResourceTransform(dictName)}";
        using Stream? dictStream = RootAssembly.GetManifestResourceStream(resourcePath)
            ?? throw new Exception($"Unable to load model dicts file embedded resource {resourcePath} from assembly , model not exists?");
        return ReadLinesFromStream(dictStream).ToList();

        static IEnumerable<string> ReadLinesFromStream(Stream stream)
        {
            using StreamReader reader = new(stream);
            while (!reader.EndOfStream)
            {
                yield return reader.ReadLine();
            }
        }
    }

    internal static string EmbeddedResourceTransform(string name) => name.Replace('-', '_').Replace(".0", "._0");

    internal static byte[] ReadResourceAsBytes(string key, Assembly assembly)
    {
        using Stream? stream = assembly.GetManifestResourceStream(key)
            ?? throw new Exception($"Unable to load model embedded resource {key} from assembly, model not exists?");
        using MemoryStream ms = new();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}

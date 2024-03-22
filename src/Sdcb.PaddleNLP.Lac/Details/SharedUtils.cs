using Sdcb.PaddleInference;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sdcb.PaddleNLP.Lac.Tests")]

namespace Sdcb.PaddleNLP.Lac.Details;

internal class SharedUtils
{
    public readonly static Type RootType = typeof(ChineseSegmenter);
    public readonly static Assembly RootAssembly = typeof(ChineseSegmenter).Assembly;
    public static string Prefix => RootType.Namespace;

    public static PaddleConfig CreateLacConfig()
    {
        string prefix = Prefix;
        byte[] programBuffer = ReadResourceAsBytes($"{prefix}.models.lac.inference.pdmodel");
        byte[] paramsBuffer = ReadResourceAsBytes($"{prefix}.models.lac.inference.pdiparams");
        return PaddleConfig.FromMemoryModel(programBuffer, paramsBuffer);
    }

    public static Dictionary<string, int> LoadTokenMap()
    {
        string key = $"{Prefix}.models.lac.word.dic";
        using Stream stream = RootAssembly.GetManifestResourceStream(key)!;
        return ReadLines(stream)
            .Select(x => x.Split('\t'))
            .ToDictionary(parts => parts[1], parts => int.Parse(parts[0]));
    }

    public static Dictionary<string, string> LoadQ2B()
    {
        string key = $"{Prefix}.models.lac.q2b.dic";
        using Stream stream = RootAssembly.GetManifestResourceStream(key)!;
        return ReadLines(stream)
            .Select(line => line.Split('\t'))
            .ToDictionary(parts => parts[0], parts => parts[1]);
    }

    public static string[] LoadTagMap()
    {
        string key = $"{Prefix}.models.lac.tag.dic";
        using Stream stream = RootAssembly.GetManifestResourceStream(key)!;
        Dictionary<int, string> tagMap = ReadLines(stream)
            .Select(x => x.Split('\t'))
            .GroupBy(x => int.Parse(x[0]))
            .ToDictionary(k => k.Key, v => v.Last()[1]);

        int maxTag = tagMap.Keys.Max();
        string[] result = new string[maxTag + 1];
        foreach (KeyValuePair<int, string> item in tagMap)
        {
            result[item.Key] = item.Value;
        }
        return result;
    }

    internal static IEnumerable<string> ReadLines(Stream stream)
    {
        using StreamReader reader = new (stream);
        while (!reader.EndOfStream)
        {
            yield return reader.ReadLine()!;
        }
    }

    /// <summary>
    /// Transforms the name of the embedded resource by changing hyphens to underscores and replaces '.0' with '._0'.
    /// This is probably because resource names cannot contain hyphen and '.0' is not correctly interpreted.
    /// </summary>
    /// <param name="name">The original name of the resource.</param>
    /// <returns>The transformed name of the resource.</returns>
    public static string EmbeddedResourceTransform(string name) => name.Replace('-', '_').Replace(".0", "._0");

    /// <summary>
    /// Reads the embedded resource as an array of bytes from the specified assembly.
    /// </summary>
    /// <param name="key">The identifier (name) of the embedded resource.</param>
    /// <returns>An array of bytes, representing the embedded resource content.</returns>
    /// <exception cref="System.Exception">Exception thrown when the specified embedded resource does not exist in the assembly.</exception>
    public static byte[] ReadResourceAsBytes(string key)
    {
        using Stream? stream = RootAssembly.GetManifestResourceStream(key)
            ?? throw new Exception($"Unable to load model embedded resource {key} from assembly, model not exists?");
        using MemoryStream ms = new();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
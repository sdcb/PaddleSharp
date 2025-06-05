using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sdcb.PaddleOCR.Models.Shared;

/// <summary>
/// A utility class that handles the operations shared between Local and Online modes of PaddleOCR.
/// </summary>
public class SharedUtils
{
    /// <summary>
    /// Reads lines from embedded resources within the assembly. This method returns a list of strings where each
    /// string represents a line of the embedded resource.
    /// </summary>
    /// <param name="dictName">The name of the embedded dictionary resource being read from.</param>
    /// <returns>A list of strings, each of which represents a line from the specified dictionary.</returns>
    /// <exception cref="System.Exception">Exception thrown when the specified embedded resource does not exist in the assembly.</exception>
    public static List<string> LoadDicts(string dictName)
    {
        Type RootType = typeof(SharedUtils);
        Assembly RootAssembly = RootType.Assembly;

        string ns = RootType.Namespace;
        string resourcePath = $"{ns}.dicts.{EmbeddedResourceTransform(dictName)}";
        using Stream? dictStream = RootAssembly.GetManifestResourceStream(resourcePath)
            ?? throw new Exception($"Unable to load model dicts file embedded resource {resourcePath} from assembly, model not exists?");
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
    /// <param name="assembly">The assembly where the embedded resource is located.</param>
    /// <returns>An array of bytes, representing the embedded resource content.</returns>
    /// <exception cref="System.Exception">Exception thrown when the specified embedded resource does not exist in the assembly.</exception>
    public static byte[] ReadResourceAsBytes(string key, Assembly assembly)
    {
        using Stream? stream = assembly.GetManifestResourceStream(key)
            ?? throw new Exception($"Unable to load model embedded resource {key} from assembly, model not exists?");
        using MemoryStream ms = new();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
using Sdcb.PaddleInference;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sdcb.PaddleOCR.Models.LocalV3.Details;

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
        using Stream? stream = RootAssembly.GetManifestResourceStream(key) 
            ?? throw new Exception($"Unable to load model embedded resource {key} from assembly, model not exists?");
        using MemoryStream ms = new ();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    static string EmbeddedResourceTransform(string name) => name.Replace('-', '_').Replace(".0", "._0");
}

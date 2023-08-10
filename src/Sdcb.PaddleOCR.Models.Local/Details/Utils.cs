using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Shared;
using System;
using System.IO;
using System.Reflection;

namespace Sdcb.PaddleOCR.Models.Local.Details;

internal static class Utils
{
    public readonly static Type RootType = typeof(LocalFullModels);
    public readonly static Assembly RootAssembly = typeof(LocalFullModels).Assembly;

    public static PaddleConfig LoadLocalModel(string key)
    {
        string ns = RootType.Namespace;
        byte[] programBuffer = SharedUtils.ReadResourceAsBytes($"{ns}.models.{SharedUtils.EmbeddedResourceTransform(key)}.inference.pdmodel");
        byte[] paramsBuffer = SharedUtils.ReadResourceAsBytes($"{ns}.models.{SharedUtils.EmbeddedResourceTransform(key)}.inference.pdiparams");
        return PaddleConfig.FromMemoryModel(programBuffer, paramsBuffer);
    }
}

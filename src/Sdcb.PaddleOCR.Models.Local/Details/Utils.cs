using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Shared;
using System;
using System.Reflection;

namespace Sdcb.PaddleOCR.Models.Local.Details;

internal static class Utils
{
    public readonly static Type RootType = typeof(LocalFullModels);
    public readonly static Assembly RootAssembly = typeof(LocalFullModels).Assembly;

    public static PaddleConfig LocalModel(string key, ModelVersion version)
    {
        Type rootType = version switch 
        {
            ModelVersion.V2 => typeof(LocalFullModels),
            ModelVersion.V3 => typeof(LocalV3.KnownModels),
            ModelVersion.V4 => typeof(LocalV4.KnownModels),
            _ => throw new NotImplementedException()
        };
        string prefix = rootType.Namespace;
        Assembly assembly = rootType.Assembly;

        byte[] programBuffer = SharedUtils.ReadResourceAsBytes($"{prefix}.models.{SharedUtils.EmbeddedResourceTransform(key)}.inference.pdmodel", assembly);
        byte[] paramsBuffer = SharedUtils.ReadResourceAsBytes($"{prefix}.models.{SharedUtils.EmbeddedResourceTransform(key)}.inference.pdiparams", assembly);
        return PaddleConfig.FromMemoryModel(programBuffer, paramsBuffer);
    }

    public static PaddleConfig LoadLocalModel(string key) => LocalModel(key, ModelVersion.V2);
}

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
        string assemblyName = version switch 
        {
            ModelVersion.V2 => "Sdcb.PaddleOCR.Models.Local",
            ModelVersion.V3 => "Sdcb.PaddleOCR.Models.LocalV3",
            ModelVersion.V4 => "Sdcb.PaddleOCR.Models.LocalV4",
            _ => throw new NotImplementedException()
        };
        Assembly assembly = Assembly.LoadFrom(assemblyName + ".dll");

        byte[] programBuffer = SharedUtils.ReadResourceAsBytes($"{assemblyName}.models.{SharedUtils.EmbeddedResourceTransform(key)}.inference.pdmodel", assembly);
        byte[] paramsBuffer = SharedUtils.ReadResourceAsBytes($"{assemblyName}.models.{SharedUtils.EmbeddedResourceTransform(key)}.inference.pdiparams", assembly);
        return PaddleConfig.FromMemoryModel(programBuffer, paramsBuffer);
    }

    public static PaddleConfig LoadLocalModel(string key) => LocalModel(key, ModelVersion.V2);
}

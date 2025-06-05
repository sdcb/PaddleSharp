using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using YamlDotNet.RepresentationModel;

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
            ModelVersion.V5 => typeof(LocalV5.KnownModels),
            _ => throw new NotImplementedException()
        };
        string prefix = rootType.Namespace;
        Assembly assembly = rootType.Assembly;

        string programBufferKey = $"{prefix}.models.{SharedUtils.EmbeddedResourceTransform(key)}.inference" + (version == ModelVersion.V5 ? ".json" : ".pdmodel");

        byte[] programBuffer = SharedUtils.ReadResourceAsBytes(programBufferKey, assembly);
        byte[] paramsBuffer = SharedUtils.ReadResourceAsBytes($"{prefix}.models.{SharedUtils.EmbeddedResourceTransform(key)}.inference.pdiparams", assembly);
        return PaddleConfig.FromMemoryModel(programBuffer, paramsBuffer);
    }

    public static IReadOnlyList<string> LoadV5Dicts(string key)
    {
        Type rootType = typeof(LocalV5.KnownModels);
        string prefix = rootType.Namespace;
        Assembly assembly = rootType.Assembly;

        string resourceKey = $"{prefix}.models.{SharedUtils.EmbeddedResourceTransform(key)}.inference.yml";

        using Stream? stream = assembly.GetManifestResourceStream(resourceKey)
            ?? throw new Exception($"Unable to load model embedded resource {resourceKey} from assembly {assembly.FullName}, model not exists?");
        YamlStream yaml = new();
        yaml.Load(new StreamReader(stream));
        return ((YamlSequenceNode)yaml.Documents[0].RootNode["PostProcess"]["character_dict"])
            .Select(x => ((YamlScalarNode)x).Value!)
            .ToList();
    }

    public static PaddleConfig LoadLocalModel(string key) => LocalModel(key, ModelVersion.V2);
}

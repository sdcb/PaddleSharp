using Sdcb.RotationDetector;
using Sdcb.PaddleInference;
using System;
using System.IO;
using System.Reflection;

namespace Sdcb.RotationDetector;

/// <summary>
/// Downloaded from: https://paddleclas.bj.bcebos.com/models/PULC/text_image_orientation_infer.tar
/// </summary>
public class EmbeddedResourceDetectionModel : RotationDetectionModel
{
    public override InputShape Shape => DefaultShape;

    public override PaddleConfig CreateConfig()
    {
        return LoadLocalModel("text_image_orientation_infer");
    }

    static PaddleConfig LoadLocalModel(string key)
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

        using MemoryStream ms = new();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    static Assembly RootAssembly = typeof(EmbeddedResourceDetectionModel).Assembly;

    static string EmbeddedResourceTransform(string name) => name.Replace('-', '_').Replace(".0", "._0");

    public readonly static Type RootType = typeof(EmbeddedResourceDetectionModel);
}

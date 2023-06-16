using Sdcb.PaddleInference;
using System;
using System.IO;
using System.Reflection;

namespace Sdcb.RotationDetector;

/// <summary>
/// Downloaded from https://paddleclas.bj.bcebos.com/models/PULC/text_image_orientation_infer.tar.
/// Represents a rotation detection model based on text image orientation input.
/// </summary>
public class EmbeddedResourceDetectionModel : RotationDetectionModel
{
    /// <summary>
    /// Gets the input shape.
    /// </summary>
    /// <value>
    /// The input shape.
    /// </value>
    public override InputShape Shape => DefaultShape;

    /// <summary>
    /// Creates a PaddlePaddle configuration object for inferencing the model.
    /// </summary>
    /// <returns>A PaddleConfig object.</returns>
    public override PaddleConfig CreateConfig()
    {
        return LoadLocalModel("text_image_orientation_infer");
    }

    static PaddleConfig LoadLocalModel(string key)
    {
        string ns = RootType.Namespace;

        // Loads the model files as memory buffers. 
        byte[] programBuffer = ReadResourceAsBytes($"{ns}.models.{EmbeddedResourceTransform(key)}.inference.pdmodel");
        byte[] paramsBuffer = ReadResourceAsBytes($"{ns}.models.{EmbeddedResourceTransform(key)}.inference.pdiparams");

        // Creates a PaddleConfig object that represents the memory buffers.
        return PaddleConfig.FromMemoryModel(programBuffer, paramsBuffer);
    }

    static byte[] ReadResourceAsBytes(string key)
    {
        using Stream? stream = RootAssembly.GetManifestResourceStream(key) ?? throw new Exception($"Unable to load model embedded resource {key} from assembly, model not exists?");
        using MemoryStream ms = new();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    static readonly Assembly RootAssembly = typeof(EmbeddedResourceDetectionModel).Assembly;

    static string EmbeddedResourceTransform(string name) => name.Replace('-', '_').Replace(".0", "._0");

    /// <summary>
    /// Gets the root type.
    /// </summary>
    /// <value>
    /// The root type.
    /// </value>
    public readonly static Type RootType = typeof(EmbeddedResourceDetectionModel);
}

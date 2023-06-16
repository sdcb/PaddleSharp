using YamlDotNet.RepresentationModel;

namespace Sdcb.PaddleDetection;

/// <summary>
/// Non-max suppression configuration settings for object detection.
/// </summary>
public class NMSConfig
{
    /// <summary>
    /// The maximum number of output top scores for object detection.
    /// </summary>
    public int KeepTopK { get; set; }

    /// <summary>
    /// The name of the configuration.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The threshold value for non-max suppression.
    /// </summary>
    public float NmsThreshold { get; set; }

    /// <summary>
    /// The maximum number of boxes to keep after non-max suppression.
    /// </summary>
    public int NmsTopK { get; set; }

    /// <summary>
    /// The minimum score threshold for object detection.
    /// </summary>
    public float ScoreThreshold { get; set; }

    /// <summary>
    /// Parses a YAML mapping node into an NMS configuration object.
    /// </summary>
    /// <param name="config">The YAML mapping node containing NMS configuration settings.</param>
    /// <returns>An NMS configuration object.</returns>
    public static NMSConfig Parse(YamlMappingNode config)
    {
        return new NMSConfig
        {
            KeepTopK = int.Parse(((YamlScalarNode)config["keep_top_k"]).Value),
            Name = ((YamlScalarNode)config["name"]).Value,
            NmsThreshold = float.Parse(((YamlScalarNode)config["nms_threshold"]).Value),
            NmsTopK = int.Parse(((YamlScalarNode)config["nms_top_k"]).Value),
            ScoreThreshold = float.Parse(((YamlScalarNode)config["score_threshold"]).Value)
        };
    }
}

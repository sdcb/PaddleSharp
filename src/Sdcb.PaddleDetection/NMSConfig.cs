using YamlDotNet.RepresentationModel;

namespace Sdcb.PaddleDetection;

public class NMSConfig
	{
		public int KeepTopK { get; set; }
		public string Name { get; set; }
		public float NmsThreshold { get; set; }
		public int NmsTopK { get; set; }
		public float ScoreThreshold { get; set; }

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
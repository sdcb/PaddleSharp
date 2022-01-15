using System;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Sdcb.PaddleDetection
{
    public class DetectionModelConfig
	{
		public static DetectionModelConfig Parse(YamlMappingNode config)
		{
			DetectionModelConfig r = new();
			if (config.Children.TryGetValue("mode", out YamlNode modeNode))
			{
				r.Mode = ((YamlScalarNode)modeNode).Value;
			}
			else
			{
				throw new Exception("Please set mode in config yml, allowed values: paddle/trt_fp16/trt_fp32.");
			}

			if (config.Children.TryGetValue("arch", out YamlNode archNode))
			{
				r.Arch = ((YamlScalarNode)archNode).Value;
			}
			else
			{
				throw new Exception("Please set model arch in config yml, allowed values: YOLO, SSD, RetinaNet, RCNN, Face.");
			}

			if (config.Children.TryGetValue("min_subgraph_size", out YamlNode minSubGraphSizeNode))
			{
				r.MinSubgraphSize = int.Parse(((YamlScalarNode)minSubGraphSizeNode).Value);
			}
			else
			{
				throw new Exception("Please set min_subgraph_size in config yml.");
			}

			if (config.Children.TryGetValue("draw_threshold", out YamlNode drawThresholdNode))
			{
				r.DrawThreshold = float.Parse(((YamlScalarNode)drawThresholdNode).Value);
			}
			else
			{
				throw new Exception("Please set draw_threshold in config yml.");
			}

			if (!config.Children.TryGetValue("Preprocess", out YamlNode preprocessNode))
			{
				throw new Exception("Please set Preprocess in config yml.");
			}

			if (config.Children.TryGetValue("label_list", out YamlNode labelListNode))
			{
				r.LabelList = ((YamlSequenceNode)labelListNode).Select(x => x.ToString()).ToArray();
			}
			else
			{
				throw new Exception("Please set label_list in config yml.");
			}

			if (config.Children.TryGetValue("use_dynamic_shape", out YamlNode useDynamicShapeNode))
			{
				r.UseDynamicShape = bool.Parse(((YamlScalarNode)useDynamicShapeNode).Value);
			}
			else
			{
				throw new Exception("Please set use_dynamic_shape in config yml.");
			}

			if (config.Children.TryGetValue("tracker", out YamlNode trackerNode))
			{
				if (((YamlMappingNode)config).Children.TryGetValue("conf_thres", out YamlNode configThresholdNode))
				{
					r.ConfigThreshold = float.Parse(((YamlScalarNode)configThresholdNode).Value);
				}
				else
				{
					throw new Exception("Please set tracker.conf_thres in config yml.");
				}
			}

			if (config.Children.TryGetValue("NMS", out YamlNode nmsNode))
			{
				r.NmsInfo = NMSConfig.Parse((YamlMappingNode)nmsNode);
			}

			if (config.Children.TryGetValue("fpn_stride", out YamlNode fpnStrideNode))
			{
				r.FpnStride = ((YamlSequenceNode)fpnStrideNode)
					.OfType<YamlScalarNode>()
					.Select(x => int.Parse(x.Value))
					.ToArray();
			}
			return r;
		}

		public string Mode { get; set; }
		public float DrawThreshold { get; set; }
		public string Arch { get; set; }
		public int MinSubgraphSize { get; set; }
		public NMSConfig NmsInfo { get; set; }
		public string[] LabelList { get; set; }
		public int[] FpnStride { get; set; }
		public bool UseDynamicShape { get; set; }
		public float ConfigThreshold { get; set; }
	}

}
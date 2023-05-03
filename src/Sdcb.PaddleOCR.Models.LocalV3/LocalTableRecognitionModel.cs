using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.LocalV3.Details;

namespace Sdcb.PaddleOCR.Models.LocalV3
{
    public class LocalTableRecognitionModel : TableRecognitionModel
    {
        public string Name { get; }

        public LocalTableRecognitionModel(string name, string dictName) : base(Utils.LoadDicts(dictName))
        {
            Name = name;
        }

        public override PaddleConfig CreateConfig()
        {
            PaddleConfig config = Utils.LoadLocalModel(Name);
            ConfigPostProcess(config);
            return config;
        }

        public static LocalTableRecognitionModel EnglishMobileV2_SLANET => new("en_ppstructure_mobile_v2.0_SLANet", "table_structure_dict.txt");

        public static LocalTableRecognitionModel ChineseMobileV2_SLANET => new("ch_ppstructure_mobile_v2.0_SLANet", "table_structure_dict_ch.txt");

        public static LocalTableRecognitionModel[] All => new[]
        {
            EnglishMobileV2_SLANET,
            ChineseMobileV2_SLANET, 
        };
    }
}

using Sdcb.PaddleOCR.Models.Online.Details;
using System;
using System.Threading.Tasks;
using System.Threading;
using Sdcb.PaddleOCR.Models.Online;
using System.IO;

namespace Sdcb.PaddleOCR.Models.LocalV3
{
    public class OnlineTableRecognitionModel
    {
        public string Name { get; }
        public Uri ModelDownloadUri { get; }
        public string DictName { get; }

        public string RootDirectory => Path.Combine(Settings.GlobalModelDirectory, Name);

        public OnlineTableRecognitionModel(string name, Uri modelDownloadUri, string dictName)
        {
            Name = name;
            ModelDownloadUri = modelDownloadUri;
            DictName = dictName;
        }

        public async Task<TableRecognitionModel> DownloadAsync(CancellationToken cancellationToken = default)
        {
            await Utils.DownloadAndExtractAsync(Name, ModelDownloadUri, RootDirectory, cancellationToken);
            return new StreamDictTableRecognizationModel(RootDirectory, Utils.LoadDicts(DictName));
        }

        public static OnlineTableRecognitionModel EnglishMobileV2_SLANET => new(
            "en_ppstructure_mobile_v2.0_SLANet", 
            new Uri("https://paddleocr.bj.bcebos.com/ppstructure/models/slanet/en_ppstructure_mobile_v2.0_SLANet_infer.tar"), 
            "table_structure_dict.txt");

        public static OnlineTableRecognitionModel ChineseMobileV2_SLANET => new(
            "ch_ppstructure_mobile_v2.0_SLANet", 
            new Uri("https://paddleocr.bj.bcebos.com/ppstructure/models/slanet/ch_ppstructure_mobile_v2.0_SLANet_infer.tar"),
            "table_structure_dict_ch.txt");

        public static OnlineTableRecognitionModel[] All => new[]
        {
            EnglishMobileV2_SLANET,
            ChineseMobileV2_SLANET, 
        };
    }
}

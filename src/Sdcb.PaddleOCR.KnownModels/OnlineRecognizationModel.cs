using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Details;
using SharpCompress.Archives;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.PaddleOCR.KnownModels
{
    public record LocalDictOnlineRecognizationModel(string name, string dictName, Uri uri, ModelVersion version)
    {
        public string RootDirectory = Path.Combine(OCRModel.GlobalModelDirectory, name);

        public static string DictRootDirectory = Path.Combine(OCRModel.GlobalModelDirectory, "dicts");

        public async Task<FileRecognizationModel> DownloadAsync(CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(RootDirectory);

            string paramsFile = Path.Combine(RootDirectory, "inference.pdiparams");
            if (!File.Exists(paramsFile))
            {
                string localTarFile = Path.Combine(RootDirectory, uri.Segments.Last());
                if (!File.Exists(localTarFile))
                {
                    Console.WriteLine($"Downloading {name} model from {uri}");
                    await OCRModel.DownloadFile(uri, localTarFile, cancellationToken);
                }

                Console.WriteLine($"Extracting {localTarFile} to {RootDirectory}");
                using (IArchive archive = ArchiveFactory.Open(localTarFile))
                {
                    archive.WriteToDirectory(RootDirectory);
                }

                File.Delete(localTarFile);
            }

            string keyFile = Path.Combine(DictRootDirectory, Path.GetFileName(dictName));
            if (!File.Exists(keyFile))
            {
                using FileStream localFile = File.OpenWrite(keyFile);
                using Stream resource = GetType().Assembly.GetManifestResourceStream($"Sdcb.PaddleOCR.KnownModels.dicts.{dictName}");
                await resource.CopyToAsync(localFile, 81920, cancellationToken);
            }

            return new FileRecognizationModel(RootDirectory, keyFile, version);
        }

        internal static Stream GetDictStreamByName(string dictName)
        {
            Type type = typeof(LocalDictOnlineRecognizationModel);
            string ns = type.Namespace;
            
            return type.Assembly.GetManifestResourceStream($"{ns}.dicts.{dictName}");
        }
    }
}

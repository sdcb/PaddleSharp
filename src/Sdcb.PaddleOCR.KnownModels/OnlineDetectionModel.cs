using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Details;
using SharpCompress.Archives;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.PaddleOCR.KnownModels
{
    public record OnlineDetectionModel(string name, Uri uri, ModelVersion version)
    {
        public string RootDirectory => Path.Combine(OCRModel.GlobalModelDirectory, name);

        public async Task<FileDetectionModel> DownloadAsync(CancellationToken cancellationToken = default)
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

            return new FileDetectionModel(RootDirectory);
        }
    }
}

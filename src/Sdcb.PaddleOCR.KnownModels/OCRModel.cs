using SharpCompress.Archives;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.PaddleOCR.KnownModels
{
    public partial class OCRModel
    {
        public OCRModel(string name, Uri detectionModelUri, Uri classifierModelUri, Uri recognitionModelUri, Uri keyUri)
        {
            Name = name;
            DetectionModelUri = detectionModelUri;
            ClassifierModelUri = classifierModelUri;
            RecognitionModelUri = recognitionModelUri;
            KeyUri = keyUri;
        }

        public Uri DetectionModelUri { get; }
        public Uri ClassifierModelUri { get; }
        public Uri RecognitionModelUri { get; }
        public Uri KeyUri { get; }
        public string Name { get; }

        public static readonly string GlobalModelDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "paddleocr-models");
        public string RootDirectory => Path.Combine(GlobalModelDirectory, Name);

        private async Task EnsureModelFile(Uri uri, string prefix, CancellationToken cancellationToken = default)
        {
            string directory = Path.Combine(RootDirectory, prefix);
            string paramsFile = Path.Combine(directory, "inference.pdiparams");
            Directory.CreateDirectory(directory);

            if (!File.Exists(paramsFile))
            {
                string localTarFile = Path.Combine(directory, uri.Segments.Last());
                if (!File.Exists(localTarFile))
                {
                    Console.WriteLine($"Downloading {prefix} model from {uri}");
                    await DownloadFile(uri, localTarFile, cancellationToken);
                }

                Console.WriteLine($"Extracting {localTarFile} to {directory}");
                using (IArchive archive = ArchiveFactory.Open(localTarFile))
                {
                    archive.WriteToDirectory(directory);
                }

                File.Delete(localTarFile);
            }
        }

        private static async Task DownloadFile(Uri uri, string localFile, CancellationToken cancellationToken)
        {
            using HttpClient http = new();

            HttpResponseMessage resp = await http.GetAsync(uri, cancellationToken);
            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to download: {uri}, status code: {(int)resp.StatusCode}({resp.StatusCode})");
            }

            using (FileStream file = File.OpenWrite(localFile))
            {
                await resp.Content.CopyToAsync(file/*, cancellationToken*/);
            }
        }

        public string DetectionDirectory => Path.Combine(RootDirectory, "det");
        public string ClassifierDirectory => Path.Combine(RootDirectory, "cls");
        public string RecognitionDirectory => Path.Combine(RootDirectory, "rec");
        public string KeyPath => Path.Combine(RootDirectory, "key.txt");

        public Task EnsureDetectionModel(CancellationToken cancellationToken = default) => EnsureModelFile(DetectionModelUri, "det", cancellationToken);
        public Task EnsureClassifierModel(CancellationToken cancellationToken = default) => EnsureModelFile(ClassifierModelUri, "cls", cancellationToken);
        public Task EnsureRecognitionModel(CancellationToken cancellationToken = default) => EnsureModelFile(RecognitionModelUri, "cls", cancellationToken);
        public async Task EnsureKeyFile(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(KeyPath))
            {
                Console.WriteLine($"Downloading key file {KeyPath} from {KeyUri}");
                await DownloadFile(KeyUri, KeyPath, cancellationToken);
            }
        }

        public Task EnsureAll(CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(
                EnsureDetectionModel(cancellationToken),
                EnsureClassifierModel(cancellationToken),
                EnsureRecognitionModel(cancellationToken),
                EnsureKeyFile(cancellationToken));
        }
    }
}

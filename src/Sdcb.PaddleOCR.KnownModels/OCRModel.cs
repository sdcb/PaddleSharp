using Sdcb.PaddleOCR.Models;
using SharpCompress.Archives;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.PaddleOCR.KnownModels
{
    public class OCRModel
    {
        public OCRModel(string name, Uri[] detectionModelUris, Uri[] classifierModelUris, Uri[] recognitionModelUris, Uri[] keyUris)
        {
            if (name == null) throw new ArgumentNullException("name");

            if (detectionModelUris == null) throw new ArgumentNullException(nameof(detectionModelUris));
            if (classifierModelUris == null) throw new ArgumentNullException(nameof(classifierModelUris));
            if (recognitionModelUris == null) throw new ArgumentNullException(nameof(recognitionModelUris));
            if (keyUris == null) throw new ArgumentNullException(nameof(keyUris));

            if (detectionModelUris.Length == 0) throw new ArgumentException($"Length of {nameof(detectionModelUris)} should > 0.");
            if (classifierModelUris.Length == 0) throw new ArgumentException($"Length of {nameof(classifierModelUris)} should > 0.");
            if (recognitionModelUris.Length == 0) throw new ArgumentException($"Length of {nameof(recognitionModelUris)} should > 0.");
            if (keyUris.Length == 0) throw new ArgumentException($"Length of {nameof(keyUris)} should > 0.");

            Name = name;
            DetectionModelUris = detectionModelUris;
            ClassifierModelUris = classifierModelUris;
            RecognitionModelUris = recognitionModelUris;
            KeyUris = keyUris;
            RootDirectory = Path.Combine(GlobalModelDirectory, Name);
        }

        public OCRModel(string name, Uri detectionModelUri, Uri classifierModelUri, Uri recognitionModelUri, Uri keyUri)
            : this(name, new[] { detectionModelUri }, new[] { classifierModelUri }, new[] { recognitionModelUri }, new[] { keyUri })
        {
        }

        public string Name { get; }
        public Uri[] DetectionModelUris { get; }
        public Uri[] ClassifierModelUris { get; }
        public Uri[] RecognitionModelUris { get; }
        public Uri[] KeyUris { get; }
        public ModelVersion Version => Name.Contains("v2") ? ModelVersion.V2 : ModelVersion.V3;
        public string RootDirectory { get; }

        private async Task EnsureModelFile(Uri[] uris, string prefix, CancellationToken cancellationToken = default)
        {
            string directory = Path.Combine(RootDirectory, prefix);
            string paramsFile = Path.Combine(directory, "inference.pdiparams");
            Directory.CreateDirectory(directory);

            if (!File.Exists(paramsFile))
            {
                string localTarFile = Path.Combine(directory, uris[0].Segments.Last());
                if (!File.Exists(localTarFile))
                {
                    Console.WriteLine($"Downloading {prefix} model from {string.Join(", ", uris.Select(x => x.ToString()))}");
                    await DownloadFiles(uris, localTarFile, cancellationToken);
                }

                Console.WriteLine($"Extracting {localTarFile} to {directory}");
                using (IArchive archive = ArchiveFactory.Open(localTarFile))
                {
                    archive.WriteToDirectory(directory);
                }

                File.Delete(localTarFile);
            }
        }

        internal static Task DownloadFile(Uri uri, string localFile, CancellationToken cancellationToken) => DownloadFiles(new Uri[] { uri }, localFile, cancellationToken);

        internal static async Task DownloadFiles(Uri[] uris, string localFile, CancellationToken cancellationToken)
        {
            using HttpClient http = new();

            foreach (Uri uri in uris)
            {
                try
                {
                    HttpResponseMessage resp = await http.GetAsync(uri, cancellationToken);
                    if (!resp.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to download: {uri}, status code: {(int)resp.StatusCode}({resp.StatusCode})");
                        continue;
                    }

                    using (FileStream file = File.OpenWrite(localFile))
                    {
                        await resp.Content.CopyToAsync(file/*, cancellationToken*/);
                        return;
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Failed to download: {uri}, {ex}");
                    continue;
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine($"Failed to download: {uri}, timeout.");
                    continue;
                }
            }

            throw new Exception($"Failed to download {localFile} from all uris: {string.Join(", ", uris.Select(x => x.ToString()))}");
        }

        public static string GlobalModelDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "paddleocr-models");

        public string DetectionDirectory => Path.Combine(RootDirectory, "det");
        public string ClassifierDirectory => Path.Combine(RootDirectory, "cls");
        public string RecognitionDirectory => Path.Combine(RootDirectory, "rec");
        public string KeyPath => Path.Combine(RootDirectory, "key.txt");

        public Task EnsureDetectionModel(CancellationToken cancellationToken = default) => EnsureModelFile(DetectionModelUris, "det", cancellationToken);
        public Task EnsureClassifierModel(CancellationToken cancellationToken = default) => EnsureModelFile(ClassifierModelUris, "cls", cancellationToken);
        public Task EnsureRecognitionModel(CancellationToken cancellationToken = default) => EnsureModelFile(RecognitionModelUris, "rec", cancellationToken);
        public async Task EnsureKeyFile(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(KeyPath))
            {
                Console.WriteLine($"Downloading key file {KeyPath} from {string.Join(", ", KeyUris.Select(x => x))}");
                await DownloadFiles(KeyUris, KeyPath, cancellationToken);
            }
        }

        public async Task<FullOcrModel> EnsureAll(CancellationToken cancellationToken = default)
        {
            await Task.WhenAll(
                EnsureDetectionModel(cancellationToken),
                EnsureClassifierModel(cancellationToken),
                EnsureRecognitionModel(cancellationToken),
                EnsureKeyFile(cancellationToken));

            return new FullOcrModel(
                DetectionModel.FromDirectory(DetectionDirectory),
                ClassificationModel.FromDirectory(ClassifierDirectory),
                RecognizationModel.FromDirectory(RecognitionDirectory, KeyPath, Version));
        }
    }
}

using SharpCompress.Archives;
using SharpCompress.Archives.GZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.PaddleOCR.Models.Online.Details;

internal static class Utils
{
    public static Task DownloadFile(Uri uri, string localFile, CancellationToken cancellationToken) => DownloadFiles(new Uri[] { uri }, localFile, cancellationToken);

    public static async Task DownloadFiles(Uri[] uris, string localFile, CancellationToken cancellationToken)
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

    public static async Task DownloadAndExtractAsync(string name, Uri uri, string rootDir, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(rootDir);
        string paramsFile = Path.Combine(rootDir, "inference.pdiparams");

        if (!File.Exists(paramsFile))
        {
            string localTarFile = Path.Combine(rootDir, uri.Segments.Last());
            if (!File.Exists(localTarFile) || new FileInfo(localTarFile).Length == 0)
            {
                Console.WriteLine($"Downloading {name} model from {uri}");
                await DownloadFile(uri, localTarFile, cancellationToken);
            }

            Console.WriteLine($"Extracting {localTarFile} to {rootDir}");
            using (IArchive archive = ArchiveFactory.Open(localTarFile))
            {
                if (archive is GZipArchive)
                {
                    using Stream stream = archive.Entries.Single().OpenEntryStream();
                    using MemoryStream ms = new();
                    stream.CopyTo(ms);
                    ms.Position = 0;
                    IArchive inner = ArchiveFactory.Open(ms);
                    inner.WriteToDirectory(rootDir);
                }
                else
                {
                    archive.WriteToDirectory(rootDir);
                }

                CheckLocalOCRModel(rootDir);
            }

            File.Delete(localTarFile);
        }
    }

    public static void CheckLocalOCRModel(string rootDir)
    {
        string[] filesToCheck = new[]
        {
            Path.Combine(rootDir, "inference.pdiparams"),
            Path.Combine(rootDir, "inference.pdmodel"), 
        };

        foreach (string path in filesToCheck)
        {
            string fileName = Path.GetFileName(path);

            if (!File.Exists(path))
            {
                throw new Exception($"{fileName} not found in {rootDir}, model error?");
            }

            if (new FileInfo(path).Length == 0)
            {
                throw new Exception($"{fileName} invalid(length = 0), model error?");
            }
        }
    }

    public static List<string> LoadDicts(string dictName)
    {
        string ns = RootType.Namespace;
        string resourcePath = $"{ns}.dicts.{EmbeddedResourceTransform(dictName)}";
        using Stream? dictStream = RootAssembly.GetManifestResourceStream(resourcePath);
        if (dictStream == null)
        {
            throw new Exception($"Unable to load rec model dicts file embedded resource {resourcePath} from assembly , model not exists?");
        }

        return ReadLinesFromStream(dictStream).ToList();

        static IEnumerable<string> ReadLinesFromStream(Stream stream)
        {
            using StreamReader reader = new(stream);
            while (!reader.EndOfStream)
            {
                yield return reader.ReadLine();
            }
        }
    }

    static string EmbeddedResourceTransform(string name) => name.Replace('-', '_').Replace(".0", "._0");

    public readonly static Type RootType = typeof(Settings);
    public readonly static Assembly RootAssembly = typeof(Settings).Assembly;
}

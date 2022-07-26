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
    public class Settings
    {
        public static string GlobalModelDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "paddleocr-models");
    }

    internal static class Utils
    {
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
    }
}

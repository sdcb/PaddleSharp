<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

async Task Main()
{
	await SetupAsync(QueryCancelToken);
}

async Task SetupAsync(CancellationToken cancellationToken = default)
{
	Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
	Environment.CurrentDirectory = Util.CurrentQuery.Location;
	await EnsureNugetExe(cancellationToken);
}

static void NuGetRun(string args) => Run(@".\nuget.exe", args, Encoding.GetEncoding("gb2312"));
static void DotNetRun(string args) => Run("dotnet", args.Dump(), Encoding.GetEncoding("utf-8"));
static void Run(string exe, string args, Encoding encoding) => Util.Cmd(exe, args, encoding);
static ProjectVersion[] Projects = new[]
{
	new ProjectVersion("Sdcb.Onnx", "1.11.22.423"), // 1.11.22.423
	new ProjectVersion("Sdcb.Mkldnn", "0.19"), // 0.19
	new ProjectVersion("Sdcb.Paddle2Onnx", "1.0.0.2"), // 1.0.0-rc.2
	new ProjectVersion("Sdcb.PaddleInference", "2.6.0-preview.2"),
	new ProjectVersion("Sdcb.PaddleOCR", "2.7.0.1"),
	new ProjectVersion("Sdcb.PaddleOCR.Models.Online", "2.7.0.1"),
	new ProjectVersion("Sdcb.PaddleOCR.Models.Shared", "2.7.0.1"),
	new ProjectVersion("Sdcb.PaddleOCR.Models.Local", "2.7.0"),
	new ProjectVersion("Sdcb.PaddleOCR.Models.LocalV3", "2.7.0.1"),
	new ProjectVersion("Sdcb.PaddleOCR.Models.LocalV4", "2.7.0.1"),
	new ProjectVersion("Sdcb.PaddleDetection", "2.3.3"),
	new ProjectVersion("Sdcb.RotationDetector", "1.0.3"),
	new ProjectVersion("Sdcb.PaddleNLP.Lac", "1.0.0-preview.6"),
	new ProjectVersion("Sdcb.PaddleNLP.Lac.Model", "1.0.0"),
};

static async Task DownloadFile(Uri uri, string localFile, CancellationToken cancellationToken = default)
{
	if (uri.Scheme == "https" || uri.Scheme == "http")
	{
		using HttpClient http = new();

		HttpResponseMessage resp = await http.GetAsync(uri, cancellationToken);
		if (!resp.IsSuccessStatusCode)
		{
			throw new Exception($"Failed to download: {uri}, status code: {(int)resp.StatusCode}({resp.StatusCode})");
		}

		using (FileStream file = File.OpenWrite(localFile))
		{
			await resp.Content.CopyToAsync(file, cancellationToken);
		}
	}
	else if (uri.Scheme == "file")
	{
		File.Copy(uri.ToString()[8..], localFile, overwrite: true);
	}
	else
	{
		throw new Exception($"Uri scheme: {uri.Scheme} not supported.");
	}
}

static async Task<string> EnsureNugetExe(CancellationToken cancellationToken = default)
{
	Uri uri = new Uri(@"https://dist.nuget.org/win-x86-commandline/latest/nuget.exe");
	string localPath = @".\nuget.exe";
	if (!File.Exists(localPath))
	{
		await DownloadFile(uri, localPath, cancellationToken);
	}
	return localPath;
}

record ProjectVersion(string name, string version);
<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

const string Version = "2.2.1.15";

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

static void NuGetRun(string args) => Run(@".\nuget.exe", args);
static void DotNetRun(string args) => Run("dotnet", args);
static void Run(string exe, string args) => Util.Cmd(exe, args, Encoding.GetEncoding("gb2312"));
string[] Projects = new[]
{
	"Sdcb.PaddleInference",
	"Sdcb.PaddleOCR",
	"Sdcb.PaddleOCR.KnownModels", 
};

static async Task DownloadFile(Uri uri, string localFile, CancellationToken cancellationToken = default)
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
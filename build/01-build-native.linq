<Query Kind="Program">
  <NuGetReference>SharpZipLib</NuGetReference>
  <NuGetReference>System.Linq.Async</NuGetReference>
  <Namespace>ICSharpCode.SharpZipLib.Tar</Namespace>
  <Namespace>System.IO.Compression</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

#load ".\00-common"

async Task Main()
{
	await SetupAsync(QueryCancelToken);
	//await new LinuxNuGetSource().Process(QueryCancelToken);

	string mklDnnUrl = "https://io.starworks.cc:88/paddlesharp/native-libs/2.5.0/x64-mkldnn-avx.zip";

	await MakeWin64Onnx(mklDnnUrl, QueryCancelToken);
	await MakeWin64Mkldnn(mklDnnUrl, QueryCancelToken);
	await MakeWin64Paddle2Onnx(mklDnnUrl, QueryCancelToken);

	await MakeWin64PaddleMkl("mkl", mklDnnUrl, QueryCancelToken);
	await MakeWin64PaddleOpenblas("openblas", "https://paddle-inference-lib.bj.bcebos.com/2.5.0/cxx_c/Windows/CPU/x86-64_avx-openblas-vs2017/paddle_inference_c.zip", QueryCancelToken);
	await MakeWin64PaddleOpenblas("openblas-noavx", "https://io.starworks.cc:88/paddlesharp/native-libs/2.5.0/x64-openblas-noavx.zip", QueryCancelToken);
	await MakeWin64PaddleMkl("cuda102_cudnn76_tr72_sm61_75", "https://io.starworks.cc:88/paddlesharp/native-libs/2.5.0/cuda102-cudnn76-tr72-sm61%2C75.zip", QueryCancelToken);
	await MakeWin64PaddleMkl("cuda118_cudnn86_tr85_sm86_89", "https://io.starworks.cc:88/paddlesharp/native-libs/2.5.0/cuda118-cudnn86-tr85-sm86%2C89.zip", QueryCancelToken);
}

static Task MakeWin64PaddleOpenblas(string ridSuffix, string url, CancellationToken cancellationToken = default)
	=> Make("Sdcb.PaddleInference", "win-x64", $"win64.{ridSuffix}", new("paddle_inference_c.dll", "openblas.dll"), url, new[] { "Sdcb.Onnx", "Sdcb.Paddle2Onnx" }, cancellationToken);
static Task MakeWin64PaddleMkl(string ridSuffix, string url, CancellationToken cancellationToken = default)
	=> Make("Sdcb.PaddleInference", "win-x64", $"win64.{ridSuffix}", new("paddle_inference_c.dll", "mkldnn.dll"), url, new[] { "Sdcb.Onnx", "Sdcb.Paddle2Onnx", "Sdcb.Mkldnn" }, cancellationToken);
static Task MakeWin64Onnx(string url, CancellationToken cancellationToken = default) 
	=> Make("Sdcb.Onnx", "win-x64", "win64", new("onnxruntime.dll", "onnxruntime_providers_shared.dll"), url, null, cancellationToken);
static Task MakeWin64Mkldnn(string url, CancellationToken cancellationToken = default) 
	=> Make("Sdcb.Mkldnn", "win-x64", "win64", new("libiomp5md.dll", "mklml.dll"), url, null, cancellationToken);
static Task MakeWin64Paddle2Onnx(string url, CancellationToken cancellationToken = default) 
	=> Make("Sdcb.Paddle2Onnx", "win-x64", "win64", new("paddle2onnx.dll"), url, null, cancellationToken);

static string BuildNuspec(string pkgDir, string pkgName, string[] libs, string rid, string titleRid, string[] deps)
{
	// props
	{
		string normalizedName = pkgName.Replace(".", "") + "Dlls";
		XDocument props = XDocument.Parse($"""
		<Project ToolsVersion="4.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
			<PropertyGroup>
				<{normalizedName}>$(MSBuildThisFileDirectory)..\..\runtimes</{normalizedName}>
			</PropertyGroup>
			<ItemGroup Condition="$(TargetFrameworkVersion.StartsWith('v4')) Or $(TargetFramework.StartsWith('net4'))">
			</ItemGroup>
		</Project>
		""");
		string ns = props.Root!.GetDefaultNamespace().NamespaceName;
		XmlNamespaceManager nsr = new(new NameTable());
		nsr.AddNamespace("p", ns);

		string platform = rid.Split("-").Last();

		XElement itemGroup = props.XPathSelectElement("/p:Project/p:ItemGroup", nsr)!;
		itemGroup.Add(libs
			.Select(x => Path.GetFileName(x))
			.Select(x => new XElement(XName.Get("Content", ns), new XAttribute("Include", $@"$({normalizedName})\{rid}\native\{x}"),
				new XElement(XName.Get("Link", ns), @$"dll\{platform}\{x}"),
				new XElement(XName.Get("CopyToOutputDirectory", ns), "PreserveNewest")))
				);

		props.Save(@$"{pkgDir}/{pkgName}.runtime.{titleRid}.props");
	}

	// nuspec
	{

		XDocument nuspec = XDocument.Parse(File
			.ReadAllText($"./{pkgName}.runtime.nuspec")
			.Replace("{rid}", rid)
			.Replace("{pkgName}", pkgName)
			.Replace("{year}", DateTime.Now.Year.ToString())
			.Replace("{titleRid}", titleRid));

		string ns = nuspec.Root!.GetDefaultNamespace().NamespaceName;
		XmlNamespaceManager nsr = new(new NameTable());
		nsr.AddNamespace("p", ns);

		XElement files = nuspec.XPathSelectElement("/p:package/p:files", nsr)!;
		files.Add(libs.Select(x => new XElement(
			XName.Get("file", ns),
			new XAttribute("src", x.Replace($@"{pkgName}.{titleRid}/", "")),
			new XAttribute("target", @$"runtimes\{rid}\native"))));
		files.Add(new[] { "net", "netstandard", "netcoreapp" }.Select(x => new XElement(
			XName.Get("file", ns),
			new XAttribute("src", $"{pkgName}.runtime.{titleRid}.props"),
			new XAttribute("target", @$"build\{x}\{pkgName}.runtime.{titleRid}.props"))));

		if (deps.Any())
		{
			XElement? dependencies = nuspec.XPathSelectElement("/p:package/p:metadata/p:dependencies", nsr);
			if (dependencies == null) throw new Exception($"{nameof(dependencies)} invalid in nuspec file.");
			
			string depPkgSuffix = titleRid.Split('.')[0];
			dependencies.Add(deps.Select(depId => new XElement(XName.Get("dependency", ns),
				new XAttribute("id", $"{depId}.runtime.{depPkgSuffix}"),
				new XAttribute("version", Projects.First(x => x.name == depId).version))));
		}

		string destinationFile = @$"{pkgDir}/{rid}.nuspec";
		nuspec.Save(destinationFile);
		return destinationFile;
	}
}

static async Task Make(string pkgName, string rid, string titleRid, LibNames libNames, string uri, string[]? deps = null, CancellationToken cancellationToken = default)
{
	deps = deps ?? new string[0];
	string pkgDir = $@"./{pkgName}.{titleRid}";
	string pkgBinDir = @$"{pkgDir}/bin";
	string clibFilePath = $@"{pkgBinDir}/{libNames.Primary}";
	string downloadDir = "./download";
	string pkgVersion = Projects.First(x => x.name == pkgName).version;

	Directory.CreateDirectory(pkgBinDir);
	Directory.CreateDirectory(downloadDir);

	string NuGetPath = $@".\nupkgs\{pkgName}.runtime.{titleRid}.{pkgVersion}.nupkg";

	Console.WriteLine($"processing {titleRid}...");
	if (!File.Exists(clibFilePath))
	{
		string localZipFile = await EnsurePackage(downloadDir, uri, clibFilePath, cancellationToken);
		await Decompress(pkgBinDir, localZipFile, cancellationToken);
	}
	string[] libs = GetDlls(pkgBinDir, libNames, rid);
	if (!libNames.All.SetEquals(libs.Select(x => Path.GetFileName(x))))
	{
		Util.Dif(libNames.All, libs.Select(x => Path.GetFileName(x))).Dump("some dlls missing...");
		throw new Exception("some dlls missing...");
	}

	string nugetExePath = await EnsureNugetExe(cancellationToken);

	string nuspecPath = BuildNuspec(pkgDir, pkgName, libs, rid, titleRid, deps);
	if (!File.Exists(NuGetPath))
	{
		NuGetRun($@"pack {nuspecPath} -Version {pkgVersion} -OutputDirectory .\nupkgs".Dump());
	}
	else
	{
		Util.HorizontalRun(false, Util.Metatext(NuGetPath), " exists, skip.").Dump();
	}
}

static async Task<string> EnsurePackage(string downloadDir, string uri, string clibFilePath, CancellationToken cancellationToken = default)
{
	string prefix = string.Concat(Array.ConvertAll(MD5.HashData(Encoding.UTF8.GetBytes(uri)), x => x.ToString("X2")).Take(8));
	string localZipFile = Path.Combine(downloadDir, prefix + new Uri(uri).Segments.Last());
	if (!File.Exists(clibFilePath))
	{
		if (!File.Exists(localZipFile))
		{
			Console.Write($"{clibFilePath} not exists, downloading from {uri}... ");
			await DownloadFile(new Uri(uri), localZipFile, cancellationToken);
			Console.WriteLine("Done");
		}
		else
		{
			Util.HorizontalRun(false, Util.Metatext(localZipFile), " exists, skip.").Dump();
		}
	}
	return localZipFile;
}

static async Task Decompress(string pkgBinDir, string localZipFile, CancellationToken cancellationToken)
{
	using (ZipArchive zip = ZipFile.OpenRead(localZipFile))
	{
		foreach (ZipArchiveEntry entry in zip.Entries.Where(x => x.FullName.EndsWith(".dll")))
		{
			string localEntryDest = Path.Combine(pkgBinDir, Path.GetFileName(entry.FullName));
			Console.Write($"Expand {entry.FullName} -> {localEntryDest}... ");
			using Stream stream = entry.Open();
			using FileStream localFile = File.OpenWrite(localEntryDest);
			await stream.CopyToAsync(localFile, cancellationToken);
			Console.WriteLine("Done");
		}
	}
}

static string[] GetDlls(string pkgBinDir, LibNames libNames, string rid)
{
	return Directory.EnumerateFiles(pkgBinDir, "*")
		.Where(x => libNames.Contains(Path.GetFileName(x)))
		.ToArray();
}

public class LibNames
{
	public string Primary { get; }
	public IReadOnlySet<string> All { get; }

	public LibNames(params string[] libNames)
	{
		if (libNames.Length == 0) throw new ArgumentException(nameof(libNames));

		Primary = libNames[0];
		All = libNames.ToHashSet();
	}

	public bool Contains(string name) => All.Contains(name);
}
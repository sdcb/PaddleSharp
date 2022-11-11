<Query Kind="Program">
  <NuGetReference>SharpZipLib</NuGetReference>
  <NuGetReference>System.Linq.Async</NuGetReference>
  <Namespace>ICSharpCode.SharpZipLib.Tar</Namespace>
  <Namespace>System.IO.Compression</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load ".\00-common"

static string NativeVersion => Projects.First(x => x.name == "Sdcb.PaddleInference").version;

async Task Main()
{
	await SetupAsync(QueryCancelToken);
	//await new LinuxNuGetSource().Process(QueryCancelToken);
	await new WindowsNugetSource("win-x64", "win64.mkl", "paddle_inference_c.dll", new Uri(@"https://paddle-inference-lib.bj.bcebos.com/2.4.0-rc0/cxx_c/Windows/CPU/x86-64_avx-mkl-vs2017/paddle_inference_c.zip"))
		.Process(QueryCancelToken);
	//await new WindowsNugetSource("win-x64", "win64.openblas", "paddle_inference_c.dll", new Uri(@"https://paddle-inference-lib.bj.bcebos.com/2.3.2/cxx_c/Windows/CPU/x86-64_avx-openblas-vs2017/paddle_inference_c.zip"))
	//	.Process(QueryCancelToken);
	//await new WindowsNugetSource("win-x64", "win64.cuda101_cudnn76_sm61", "paddle_inference_c.dll", new Uri("https://io.starworks.cc:88/paddlesharp/native-libs/2.3.2.avx-x64_cuda101_tr76_c.zip"))
	//	.Process(QueryCancelToken);
	//await new WindowsNugetSource("win-x64", "win64.cuda102_cudnn85_pascal", "paddle_inference_c.dll", new Uri("https://io.starworks.cc:88/paddlesharp/native-libs/2.3.2.avx-x64_cuda102_cudnn85_pascal.zip"))
	//	.Process(QueryCancelToken);
	//await new WindowsNugetSource("win-x64", "win64.cuda117_cudnn85_ampere", "paddle_inference_c.dll", new Uri("https://io.starworks.cc:88/paddlesharp/native-libs/2.3.2.avx-x64_cuda117_cudnn85_ampere.zip"))
	//	.Process(QueryCancelToken);
	//await new WindowsNugetSource("win-x64", "win64.cuda116_cudnn84_tr84", "paddle_inference_c.dll", new Uri("https://paddle-inference-lib.bj.bcebos.com/2.3.2/cxx_c/Windows/GPU/x86-64_cuda11.6_cudnn8.4.0_trt8.4.1.5_mkl_avx_vs2019/paddle_inference_c.zip"))
	//	.Process(QueryCancelToken);
	//await new WindowsNugetSource("win-x64", "win64.cuda116_cudnn84", "paddle_inference_c.dll", new Uri("https://io.starworks.cc:88/paddlesharp/native-libs/2.3.2.mkl-onnx-x64_cuda116_cudnn84_sm61_75_86.zip"))
	//	.Process(QueryCancelToken);
}

static string BuildNuspec(string[] libs, string rid, string titleRid)
{
	// props
	{
		XDocument props = XDocument.Parse(File
			.ReadAllText("./Sdcb.PaddleInference.runtime.props"));
		string ns = props.Root.GetDefaultNamespace().NamespaceName;
		XmlNamespaceManager nsr = new(new NameTable());
		nsr.AddNamespace("p", ns);

		string platform = rid.Split("-").Last();

		XElement itemGroup = props.XPathSelectElement("/p:Project/p:ItemGroup", nsr);
		itemGroup.Add(
		libs
			.Select(x => Path.GetFileName(x))
			.Select(x => new XElement(XName.Get("Content", ns), new XAttribute("Include", $@"$(PaddleInferenceDlls)\{rid}\native\{x}"),
				new XElement(XName.Get("Link", ns), @$"dll\{platform}\{x}"),
				new XElement(XName.Get("CopyToOutputDirectory", ns), "PreserveNewest")))
				);
		props.Save(@$"./{titleRid}/Sdcb.PaddleInference.runtime.{titleRid}.props");
	}

	// nuspec
	{
		XDocument nuspec = XDocument.Parse(File
			.ReadAllText("./Sdcb.PaddleInference.runtime.nuspec")
			.Replace("{rid}", rid)
			.Replace("{titleRid}", titleRid));

		string ns = nuspec.Root.GetDefaultNamespace().NamespaceName;
		XmlNamespaceManager nsr = new(new NameTable());
		nsr.AddNamespace("p", ns);

		XElement files = nuspec.XPathSelectElement("/p:package/p:files", nsr);
		files.Add(libs.Select(x => new XElement(
			XName.Get("file", ns),
			new XAttribute("src", x.Replace($@"{titleRid}\", "")),
			new XAttribute("target", @$"runtimes\{rid}\native"))));
		files.Add(new[] { "net", "netstandard", "netcoreapp" }.Select(x => new XElement(
			XName.Get("file", ns),
			new XAttribute("src", $"Sdcb.PaddleInference.runtime.{titleRid}.props"),
			new XAttribute("target", @$"build\{x}\Sdcb.PaddleInference.runtime.{titleRid}.props"))));

		string destinationFile = @$"./{titleRid}/{rid}.nuspec";
		nuspec.Save(destinationFile);
		return destinationFile;
	}
}

public record WindowsNugetSource(string rid, string titleRid, string libName, Uri uri) : NupkgBuildSource(rid, titleRid, libName, uri)
{
	protected override async Task Decompress(string localZipFile, CancellationToken cancellationToken)
	{
		using (ZipArchive zip = ZipFile.OpenRead(localZipFile))
		{
			foreach (ZipArchiveEntry entry in zip.Entries.Where(x => x.FullName.EndsWith(".dll")))
			{
				string localEntryDest = Path.Combine(PlatformDir, Path.GetFileName(entry.FullName));
				Console.Write($"Expand {entry.FullName} -> {localEntryDest}... ");
				using Stream stream = entry.Open();
				using FileStream localFile = File.OpenWrite(localEntryDest);
				await stream.CopyToAsync(localFile, cancellationToken);
				Console.WriteLine("Done");
			}
		}
	}

	protected override string[] GetDlls()
	{
		return Directory.EnumerateFiles(PlatformDir, "*.dll")
			.Select(f => f.Replace(rid + @"\", ""))
			.ToArray();
	}
}

public record LinuxNuGetSource() : NupkgBuildSource("linux-x64", "linux64.mkl", "libpaddle_inference_c.so", new Uri(@"https://paddle-inference-lib.bj.bcebos.com/2.2.1/cxx_c/Linux/CPU/gcc8.2_avx_mkl/paddle_inference_c.tgz"))
{
	protected override Task Decompress(string localZipFile, CancellationToken cancellationToken)
	{
		using (FileStream file = File.OpenRead(localZipFile))
		using (GZipStream gzip = new(file, CompressionMode.Decompress))
		using (TarInputStream tar = new(gzip, Encoding.UTF8))
		{
			static IEnumerable<TarEntry> GetAllEntries(TarInputStream tar)
			{
				TarEntry entry = null;
				while ((entry = tar.GetNextEntry()) != null)
				{
					yield return entry;
				}
			}

			Dictionary<string, string> symlinkMap = new();
			foreach (TarEntry soEntry in GetAllEntries(tar).Where(x => x.Name.Contains(".so")))
			{
				string fileName = Path.GetFileName(soEntry.Name);
				Console.WriteLine(soEntry.Name);
				if (soEntry.TarHeader.LinkName != "")
				{
					symlinkMap[fileName] = soEntry.TarHeader.LinkName;
				}

				using (FileStream destFile = File.OpenWrite(Path.Combine(PlatformDir, fileName)))
				{
					tar.CopyEntryContents(destFile);
				}
			}
			foreach (var map in symlinkMap)
			{
				string src = Path.Combine(PlatformDir, map.Key);
				string dest = Path.Combine(PlatformDir, map.Value);
				File.Copy(dest, src, /* override */ true);
				// nuget not support symlink, using File.Copy: https://github.com/NuGet/Home/issues/10734
				// File.Delete(src);
				// File.CreateSymbolicLink(src, dest).Dump();
				// Console.WriteLine($"{src} -> {dest}");
			}
		}

		return Task.FromResult(0);
	}

	protected override string[] GetDlls()
	{
		return Directory.EnumerateFiles(PlatformDir, "*.so*")
			.Select(f => f.Replace(rid + @"\", ""))
			.ToArray();
	}
}

public abstract record NupkgBuildSource(string rid, string titleRid, string libName, Uri uri)
{
	public string CLibFilePath => $@"./{titleRid}/bin/{libName}";
	public string PlatformDir => Path.GetDirectoryName(CLibFilePath);
	public string NuGetPath => $@".\nupkgs\Sdcb.PaddleInference.runtime.{titleRid}.{NativeVersion}.nupkg";

	public async Task<string> EnsurePackage(CancellationToken cancellationToken = default)
	{
		string localZipFile = Path.Combine(PlatformDir, uri.Segments.Last());
		if (!File.Exists(CLibFilePath))
		{
			Directory.CreateDirectory(PlatformDir);

			if (!File.Exists(localZipFile))
			{
				Console.Write($"{CLibFilePath} not exists, downloading from {uri}... ");
				await DownloadFile(uri, localZipFile, cancellationToken);
				Console.WriteLine("Done");
			}
		}
		return localZipFile;
	}

	protected abstract Task Decompress(string localZipFile, CancellationToken cancellationToken);
	protected abstract string[] GetDlls();

	public async Task Process(CancellationToken cancellationToken)
	{
		Console.WriteLine($"processing {titleRid}...");
		if (!File.Exists(CLibFilePath))
		{
			string localZipFile = await EnsurePackage(cancellationToken);
			await Decompress(localZipFile, cancellationToken);
			File.Delete(localZipFile);
		}
		string[] libs = GetDlls();

		string nugetExePath = await EnsureNugetExe(cancellationToken);

		string nuspecPath = BuildNuspec(libs, rid, titleRid);
		if (!File.Exists(NuGetPath))
		{
			string iconDestPath = @$".\{titleRid}\icon.jpg";
			if (!File.Exists(iconDestPath)) File.Copy(@$".\icon.jpg", iconDestPath);
			NuGetRun($@"pack {nuspecPath} -Version {NativeVersion} -OutputDirectory .\nupkgs".Dump());
			File.Delete(@$".\{titleRid}\icon.jpg");
		}
		else
		{
			Util.HorizontalRun(false, Util.Metatext(NuGetPath), " exists, skip.").Dump();
		}
	}
}
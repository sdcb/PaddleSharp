<Query Kind="Program">
  <NuGetReference>System.Linq.Async</NuGetReference>
  <Namespace>System.IO.Compression</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
</Query>

#load ".\00-common"

async Task Main()
{
	Environment.CurrentDirectory = Util.CurrentQuery.Location;
	Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
	
	string[] libs = await EnsureCLib().ToArrayAsync(QueryCancelToken);
	string nugetExePath = await EnsureNugetExe(QueryCancelToken);
	Util.Cmd(nugetExePath, $@"pack .\win64\Sdcb.PaddleInference.runtime.win64.mkl.nuspec -Version {Version} -OutputDirectory .\nupkgs", Encoding.GetEncoding("gb2312"));
}

const string PaddleInferenceCLib = @".\win64\bin\paddle_inference_c.dll";

async IAsyncEnumerable<string> EnsureCLib([EnumeratorCancellation]CancellationToken cancellationToken = default)
{
	string directory = Path.GetDirectoryName(PaddleInferenceCLib);
	if (!File.Exists(PaddleInferenceCLib))
	{
		Uri uri = new Uri(@"https://paddle-inference-lib.bj.bcebos.com/2.2.1/cxx_c/Windows/CPU/x86-64_vs2017_avx_mkl/paddle_inference_c.zip");
		Directory.CreateDirectory(directory);
		string localZipFile = Path.Combine(directory, uri.Segments.Last());

		if (!File.Exists(localZipFile))
		{
			Console.Write($"{PaddleInferenceCLib} not exists, downloading from {uri}... ");
			await DownloadFile(uri, localZipFile, cancellationToken);
			Console.WriteLine("Done");
		}

		using (ZipArchive zip = ZipFile.OpenRead(localZipFile))
		{
			foreach (ZipArchiveEntry entry in zip.Entries.Where(x => x.FullName.EndsWith(".dll")))
			{
				string localEntryDest = Path.Combine(directory, Path.GetFileName(entry.FullName));
				Console.Write($"Expand {entry.FullName} -> {localEntryDest}... ");
				using Stream stream = entry.Open();
				using FileStream localFile = File.OpenWrite(localEntryDest);
				await stream.CopyToAsync(localFile, cancellationToken);
				Console.WriteLine("Done");
			}
		}

		File.Delete(localZipFile);
	}
	
	foreach (string path in Directory.EnumerateFiles(directory, "*.dll"))
	{
		yield return path;
	}
}
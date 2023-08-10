<Query Kind="Statements">
  <NuGetReference Prerelease="true">Sdcb.PaddleOCR.Models.Online</NuGetReference>
  <Namespace>Sdcb.PaddleOCR.Models.Online</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Sdcb.PaddleOCR.Models</Namespace>
</Query>

Settings.GlobalModelDirectory = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "models");

await Parallel.ForEachAsync(OnlineDetectionModel.All.Where(x => x.Version != ModelVersion.V2 && !x.Name.Contains("_slim")), QueryCancelToken, async (m, cts) =>
{
	await m.DownloadAsync(cts);
});

await Parallel.ForEachAsync(OnlineClassificationModel.All.Where(x =>  !x.Name.Contains("_slim")), QueryCancelToken, async (m, cts) =>
{
	await m.DownloadAsync(cts);
});

await Parallel.ForEachAsync(LocalDictOnlineRecognizationModel.All.Where(x => x.Version != ModelVersion.V2 && !x.Name.Contains("_slim")), QueryCancelToken, async (m, cts) =>
{
	await m.DownloadAsync(cts);
});

foreach (string file in Directory.EnumerateFiles(Settings.GlobalModelDirectory, "*", SearchOption.AllDirectories)
	.Where(x => Path.GetFileName(x) switch
	{
		"inference.pdiparams" or "inference.pdmodel" => false,
		_ => true
	}))
{
	// Delete unnecessary files
	File.Delete(file);
}

Assembly asm = typeof(OnlineFullModels).Assembly;
string dictsFolder = Path.Combine(Settings.GlobalModelDirectory, "dicts");
Directory.CreateDirectory(dictsFolder);
foreach (string name in asm.GetManifestResourceNames())
{
	string fileName = string.Join('.', name.Split('.').TakeLast(2));
	string destFileName = Path.Combine(dictsFolder, fileName);
	if (!File.Exists(destFileName))
	{
		using FileStream stream = File.OpenWrite(destFileName);
		await asm.GetManifestResourceStream(name).CopyToAsync(stream);
	}
}

Process.Start("explorer", @$"/select, ""{dictsFolder}""".Dump());
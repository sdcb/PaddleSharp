<Query Kind="Statements">
  <NuGetReference Prerelease="true">Sdcb.PaddleOCR.Models.Online</NuGetReference>
  <Namespace>Sdcb.PaddleOCR.Models.Online</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Sdcb.PaddleOCR.Models</Namespace>
</Query>

Settings.GlobalModelDirectory = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "models");

await Parallel.ForEachAsync(OnlineDetectionModel.All.Where(x => x.Version == ModelVersion.V4 && !x.Name.Contains("_slim")), QueryCancelToken, async (m, cts) =>
{
	await m.DownloadAsync(cts);
});

await Parallel.ForEachAsync(OnlineClassificationModel.All.Where(x => !x.Name.Contains("_slim")), QueryCancelToken, async (m, cts) =>
{
	await m.DownloadAsync(cts);
});

await Parallel.ForEachAsync(LocalDictOnlineRecognizationModel.All.Where(x => x.Version == ModelVersion.V4 && !x.Name.Contains("_slim")), QueryCancelToken, async (m, cts) =>
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

Process.Start("explorer", @$"/select, ""{Settings.GlobalModelDirectory}""".Dump());
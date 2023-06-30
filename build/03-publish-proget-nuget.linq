<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

#load ".\00-common"

DumpContainer dc = new DumpContainer().Dump();

async Task Main()
{
	await SetupAsync(QueryCancelToken);	
	Refresh();
}

void PublishProGet(string path)
{
	string nugetApiKey = Util.GetPassword("proget-api-key");
	string nugetApiUrl = Util.GetPassword("proget-api-test");
	//DotNetRun($@"nuget push {path} -s {nugetApiUrl} -k {nugetApiKey}");
	// dotnet nuget add source -n proget -u zhoujie -p "xxxx"
	DotNetRun($@"nuget push {path} -s proget");
}

void PublishNuGet(string path)
{
	string nugetApiUrl = "nuget.org";
	string nugetApiKey = Util.GetPassword("nuget-api-key");
	NuGetRun($@"push {path} {nugetApiKey} -Source {nugetApiUrl}".Dump());
}

void Refresh()
{
	dc.Content = BuildTable();

	object BuildTable()
	{
		return Directory.EnumerateFiles(@".\nupkgs\", "*.nupkg")
			//.Where(x => x.Contains(Version))
			.Select(x => new
			{
				Package = Path.GetFileNameWithoutExtension(x),
				Size = $"{new FileInfo(x).Length / 1024.0 / 1024:N2}MB",
				Publish = new Button("Publish", o => PublishProGet(x)),
				PublishNuGet = new Button("Publish NuGet", o => PublishNuGet(x)),
				Delete = new Button("Delete", o =>
				{
					File.Delete(x);
					Refresh();
				}),
			});
	}
}
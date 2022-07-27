<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

#load ".\00-common"

async Task Main()
{
	await SetupAsync(QueryCancelToken);
	
	DumpContainer dc = new DumpContainer().Dump();
	Refresh(dc, Publish);

	void Publish(string path)
	{
		string nugetApiKey = Util.GetPassword("proget-api-key");
		string nugetApiUrl = Util.GetPassword("proget-api-test");
		NuGetRun($@"push {path} {nugetApiKey} -Source {nugetApiUrl}");
		Refresh(dc, Publish);
	}
}

static void Refresh(DumpContainer dc, Action<string> publish)
{
	dc.Content = BuildTable();

	object BuildTable()
	{
		return Directory.EnumerateFiles(@".\nupkgs\", "*.nupkg")
			//.Where(x => x.Contains(Version))
			.Select(x => new
			{
				Package = Path.GetFileNameWithoutExtension(x),
				Publish = new Button("Publish", o => publish(x)),
				Delete = new Button("Delete", o =>
				{
					File.Delete(x);
					Refresh(dc, publish);
				}),
			});
	}
}
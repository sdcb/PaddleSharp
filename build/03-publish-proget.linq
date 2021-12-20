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

void Refresh()
{
	dc.Content = BuildTable();
}

object BuildTable()
{
	return Directory.EnumerateFiles(@".\nupkgs\", "*.nupkg")
		.Where(x => x.Contains(Version))
		.Select(x => new 
		{ 
			Package = Path.GetFileNameWithoutExtension(x), 
			Publish = new Button("Publish", o => Publish(x)), 
		});
}

void Publish(string path)
{
	string nugetApiKey = Util.GetPassword("proget-api-key");
	string nugetApiUrl = Util.GetPassword("proget-api-test");
	NuGetRun($@"push {path} {nugetApiKey} -Source {nugetApiUrl}");
	Refresh();
}
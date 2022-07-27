<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

#load ".\00-common"
#load ".\03-publish-proget"

async Task Main()
{
	Util.Metatext("注意，这个是发布到nuget！").Dump();
	await SetupAsync(QueryCancelToken);

	DumpContainer dc = new DumpContainer().Dump();
	Refresh(dc, Publish);

	void Publish(string path)
	{
		string nugetApiUrl = "nuget.org";
		string nugetApiKey = Util.GetPassword("nuget-api-key");
		NuGetRun($@"push {path} {nugetApiKey} -Source {nugetApiUrl}".Dump());
		Refresh(dc, Publish);
	}
}
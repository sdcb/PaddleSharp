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
	dc.Content = LoadTable();
}

object LoadTable()
{
	return Projects
		.Select(x => new
		{
			Project = x, 
			Build = new Button("Build", o => Build(x))
		});
}

void Build(string project)
{
	//DotNetRun($@"build ..\src\{project}\{project}.csproj -c Release");
	DotNetRun($@"pack ..\src\{project}\{project}.csproj -p:Version={Version} -c Release -o .\nupkgs".Dump());
	Refresh();
}
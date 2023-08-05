using System;
using Xunit.Abstractions;

namespace Sdcb.PaddleInference.Tests;

public class UnitTest1
{
    private readonly ITestOutputHelper _console;

    public UnitTest1(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public void VersionTest()
    {
        _console.WriteLine(PaddleConfig.Version);
    }

    [Fact]
    public void GetVersionTest()
    {
        _console.WriteLine(PaddleConfig.GetVersion().ToString());
    }
}
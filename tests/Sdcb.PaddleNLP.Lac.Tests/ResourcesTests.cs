using Sdcb.PaddleInference;
using Sdcb.PaddleNLP.Lac.Details;

namespace Sdcb.PaddleNLP.Lac.Tests;

public class ResourcesTests
{
    [Fact]
    public void Q2bTest()
    {
        Dictionary<string, string> q2b = SharedUtils.LoadQ2B();
        Assert.NotEmpty(q2b);
    }

    [Fact]
    public void TokenTest()
    {
        Dictionary<string, int> tokens = SharedUtils.LoadTokenMap();
        Assert.NotEmpty(tokens);
    }

    [Fact]
    public void TagTest()
    {
        string[] tags = SharedUtils.LoadTagMap();
        Assert.NotEmpty(tags);
    }

    [Fact]
    public void CreateConfigTest()
    {
        using PaddleConfig c = SharedUtils.CreateLacConfig();
    }
}
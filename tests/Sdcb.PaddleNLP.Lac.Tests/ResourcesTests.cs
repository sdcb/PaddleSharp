using Sdcb.PaddleNLP.Lac.Model;

namespace Sdcb.PaddleNLP.Lac.Tests;

public class ResourcesTests
{
    [Fact]
    public void Q2bTest()
    {
        Dictionary<string, string> q2b = LacModelUtils.LoadQ2B();
        Assert.NotEmpty(q2b);
    }

    [Fact]
    public void TokenTest()
    {
        Dictionary<string, int> tokens = LacModelUtils.LoadTokenMap();
        Assert.NotEmpty(tokens);
    }

    [Fact]
    public void TagTest()
    {
        string[] tags = LacModelUtils.LoadTagMap();
        Assert.NotEmpty(tags);
    }

    [Fact]
    public void CreateConfigTest()
    {
        using ChineseSegmenter chineseSegmenter = new();
    }
}
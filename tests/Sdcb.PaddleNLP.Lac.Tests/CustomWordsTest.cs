namespace Sdcb.PaddleNLP.Lac.Tests;

public class CustomWordsTest
{
    [Fact]
    public void CanAddCustomWords()
    {
        string input = "我爱北京天安门";
        using ChineseSegmenter segmenter = new(new LacOptions(new()
        {
            ["北京天安门"] = null
        }));
        WordAndTag[] result = segmenter.Tagging(input);
        string labels = string.Join(",", result.Select(x => x.Label));
        string words = string.Join(",", result.Select(x => x.Word));
        string tags = string.Join(",", result.Select(x => x.Tag));
        Assert.Equal("我,爱,北京天安门", words);
        Assert.Equal("r,v,LOC", labels);
        Assert.Equal("Pronoun,Verb,LocationName", tags);
    }
}

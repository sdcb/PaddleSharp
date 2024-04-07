namespace Sdcb.PaddleNLP.Lac.Tests;

public class FastTrieTreeTests
{
    [Fact]
    public void AddAndFindSingleWord()
    {
        FastTrieTree tree = ["word"];
        var results = tree.FindAll("word");

        Assert.Single(results);
        Assert.Equal((0, 4), results[0]);
    }

    [Fact]
    public void AddAndFindMultipleWords()
    {
        FastTrieTree tree = ["word", "another"];

        var resultsWord = tree.FindAll("word");
        var resultsAnother = tree.FindAll("anotherworld");
        var resultsBoth = tree.FindAll("anotherword");

        Assert.Single(resultsWord);
        Assert.Equal((0, 4), resultsWord[0]);

        Assert.Single(resultsAnother);
        Assert.Contains((0, 7), resultsAnother);

        Assert.Equal(2, resultsBoth.Count);
        Assert.Contains((0, 7), resultsBoth);
        Assert.Contains((7, 11), resultsBoth);
    }

    [Fact]
    public void EmptyTree()
    {
        FastTrieTree tree = [];
        var results = tree.FindAll("word");

        Assert.Empty(results);
    }

    [Fact]
    public void NoMatchFound()
    {
        FastTrieTree tree = ["word"];
        var results = tree.FindAll("hello");

        Assert.Empty(results);
    }

    [Fact]
    public void PartialMatchOnly()
    {
        FastTrieTree tree = ["word"];
        var results = tree.FindAll("wor");

        // No full word match; expect nothing.
        Assert.Empty(results);
    }
}

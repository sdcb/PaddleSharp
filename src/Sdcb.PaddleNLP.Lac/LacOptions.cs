using Sdcb.PaddleNLP.Lac.Model;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sdcb.PaddleNLP.Lac;

/// <summary>
/// 定义了LAC模型的配置选项。
/// </summary>
public record LacOptions
{
    /// <summary>Token词汇表的对照，用于将字符映射为其相应的token值。</summary>
    public Dictionary<string, int> TokenMap { get;  }

    /// <summary>繁体字到简体字的映射表，支持将繁体字转换为简体字。</summary>
    public Dictionary<string, string> Q2b { get; }

    /// <summary>词性及实体标签数组，定义了模型能够识别的所有标签。</summary>
    public string[] TagMap { get; }

    /// <summary>自定义词汇表，允许用户定义特定词汇及其对应的标签。如果不提供，则默认为null。</summary>
    public Dictionary<string, WordTag?>? CustomizedWords { get; }

    /// <summary>
    /// 用于缓存自定义词汇表的Trie树。
    /// </summary>
    private readonly FastTrieTree _trieTree;

    /// <summary>
    /// 初始化LAC模型的配置选项。
    /// </summary>
    /// <param name="tokenMap">Token词汇表的对照，用于将字符映射为其相应的token值。</param>
    /// <param name="q2b">繁体字到简体字的映射表，支持将繁体字转换为简体字。</param>
    /// <param name="tagMap">词性及实体标签数组，定义了模型能够识别的所有标签。</param>
    /// <param name="customizedWords">自定义词汇表，允许用户定义特定词汇及其对应的标签。如果不提供，则默认为null。</param>
    public LacOptions(
        Dictionary<string, int> tokenMap,
        Dictionary<string, string> q2b,
        string[] tagMap,
        Dictionary<string, WordTag?>? customizedWords = null)
    {
        TokenMap = tokenMap;
        Q2b = q2b;
        TagMap = tagMap;
        CustomizedWords = customizedWords;

        _trieTree = [.. CustomizedWords != null ? CustomizedWords.Keys : Enumerable.Empty<string>()];
    }

    /// <summary>
    /// 以默认配置选项初始化LAC模型。
    /// </summary>
    /// <param name="customizedWords">自定义词汇表，允许用户定义特定词汇及其对应的标签。如果不提供，则默认为null。</param>
    public LacOptions(Dictionary<string, WordTag?>? customizedWords = null) : this(
        LacModelUtils.LoadTokenMap(),
        LacModelUtils.LoadQ2B(),
        LacModelUtils.LoadTagMap(), 
        customizedWords)
    {
    }

    /// <summary>
    /// 获取LAC模型的默认配置选项。
    /// </summary>
    public static LacOptions Default { get; } = new();

    /// <summary>
    /// 将单个字符转换为对应的token。如果字符存在于繁体到简体的映射表中，则转换为简体后查找；
    /// 否则直接查找。如果查找失败，返回0。
    /// </summary>
    /// <param name="c">要转换的字符。</param>
    /// <returns>字符对应的token值；如果未找到对应token，则返回0。</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int TransformChar(char c)
    {
        string q2b = Q2b.TryGetValue(c.ToString(), out string value) ? value : c.ToString();
        return TokenMap.TryGetValue(q2b, out int token) ? token : 0;
    }

    /// <summary>
    /// 根据自定义词汇和其标签对输入字符串进行转换，更新其标签数组。
    /// </summary>
    /// <param name="input">输入的字符串。</param>
    /// <param name="tags">原标签数组。</param>
    /// <returns>更新后的标签数组。</returns>
    public int[] TransformCustomizedWords(string input, int[] tags)
    {
        int[] resultTags = [.. tags];

        foreach ((int start, int end) in _trieTree.FindAll(input))
        {
            string word = input[start..end];
            WordTag? tag = CustomizedWords![word];
            tag ??= (WordTag)tags[start];

            resultTags[start] = (int)tag;
            for (int i = start + 1; i < end; i++)
            {
                resultTags[i] = (int)tag + 1;
            }
        }

        return resultTags;
    }
}
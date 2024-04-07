using System.Collections.Generic;
using System.Linq;

namespace Sdcb.PaddleNLP.Lac;

/// <summary>
/// 词云扩展类
/// </summary>
public static class WordCloudExtensions
{
    /// <summary>
    /// 词云常用的词类型
    /// </summary>
    public static readonly HashSet<WordTag> WordCloudDefaultTags =
    [
        WordTag.Adjective,         // 形容词
        WordTag.CommonNoun,        // 普通名词
        WordTag.PersonalName,      // 人名
        WordTag.LocationName,      // 地名
        WordTag.OrganizationName,  // 机构名
        WordTag.WorkName,          // 作品名
        WordTag.Verb               // 普通动词
    ];

    /// <summary>
    /// 将输入文本转换为词云
    /// </summary>
    /// <param name="chineseSegmenter">中文分词器</param>
    /// <param name="inputText">输入文本</param>
    /// <param name="filterTags">过滤标签，如果为空，则使用默认的词云标签：<see cref="WordCloudDefaultTags"/></param>
    /// <returns>返回一个字典，其中键是单词，值是单词出现的次数</returns>
    public static Dictionary<string, int> ToWordCloud(this ChineseSegmenter chineseSegmenter, string inputText, HashSet<WordTag>? filterTags = null)
    {
        filterTags ??= WordCloudDefaultTags;
        return chineseSegmenter.Tagging(inputText)
            .Where(x => filterTags.Contains(x.Tag))
            .GroupBy(x => x.Word)
            .ToDictionary(x => x.Key, x => x.Count());
    }
}

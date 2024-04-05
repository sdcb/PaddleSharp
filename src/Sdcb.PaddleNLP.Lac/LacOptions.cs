using Sdcb.PaddleNLP.Lac.Details;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sdcb.PaddleNLP.Lac;

/// <summary>
/// 定义了LAC模型的配置选项。
/// </summary>
/// <param name="TokenMap">Token词汇表的对照，用于将字符映射为其相应的token值。</param>
/// <param name="Q2b">繁体字到简体字的映射表，支持将繁体字转换为简体字。</param>
/// <param name="TagMap">词性及实体标签数组，定义了模型能够识别的所有标签。</param>
/// <param name="CustomizedWords">自定义词汇表，允许用户定义特定词汇及其对应的标签。如果不提供，则默认为null。</param>
public record LacOptions(
    Dictionary<string, int> TokenMap,
    Dictionary<string, string> Q2b,
    string[] TagMap,
    Dictionary<string, WordTag?>? CustomizedWords = null)
{
    /// <summary>
    /// 获取LAC模型的默认配置选项。
    /// </summary>
    public static LacOptions Default { get; } = new(
        SharedUtils.LoadTokenMap(),
        SharedUtils.LoadQ2B(),
        SharedUtils.LoadTagMap());

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
}
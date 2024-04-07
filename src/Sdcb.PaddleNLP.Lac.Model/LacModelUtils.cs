using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sdcb.PaddleNLP.Lac.Model;

/// <summary>
/// LacModelUtils 类包含用于操作和读取模型资源的静态方法。
/// </summary>
public class LacModelUtils
{
    /// <summary>
    /// 根类型，用于辅助定位资源的命名空间。
    /// </summary>
    internal readonly static Type RootType = typeof(LacModelUtils);

    /// <summary>
    /// 根程序集，用于从中获取嵌入的资源。
    /// </summary>
    internal readonly static Assembly RootAssembly = typeof(LacModelUtils).Assembly;

    /// <summary>
    /// 资源前缀，用于构建资源的完整名称。
    /// </summary>
    internal static string Prefix { get; } = $"{RootType.Namespace}.models.lac";

    /// <summary>
    /// 读取模型程序的数据。
    /// </summary>
    /// <returns>表示模型程序的字节数组。</returns>
    public static byte[] ReadModelProgram() => ReadResourceAsBytes($"{Prefix}.inference.pdmodel");

    /// <summary>
    /// 读取模型参数的数据。
    /// </summary>
    /// <returns>表示模型参数的字节数组。</returns>
    public static byte[] ReadModelParams() => ReadResourceAsBytes($"{Prefix}.inference.pdiparams");

    /// <summary>
    /// 加载词汇映射。
    /// </summary>
    /// <returns>词汇映射的字典，其中键为词汇，值为对应的整数标识。</returns>
    public static Dictionary<string, int> LoadTokenMap()
    {
        string key = $"{Prefix}.word.dic";
        using Stream stream = RootAssembly.GetManifestResourceStream(key)!;
        return ReadLines(stream)
            .Select(x => x.Split('\t'))
            .ToDictionary(parts => parts[1], parts => int.Parse(parts[0]));
    }

    /// <summary>
    /// 加载全角到半角的映射。
    /// </summary>
    /// <returns>全角到半角的映射字典。</returns>
    public static Dictionary<string, string> LoadQ2B()
    {
        string key = $"{Prefix}.q2b.dic";
        using Stream stream = RootAssembly.GetManifestResourceStream(key)!;
        return ReadLines(stream)
            .Select(line => line.Split('\t'))
            .ToDictionary(parts => parts[0], parts => parts[1]);
    }

    /// <summary>
    /// 加载标签映射。
    /// </summary>
    /// <returns>包含标签映射的字符串数组，数组索引对应标签的整数标识。</returns>
    public static string[] LoadTagMap()
    {
        string key = $"{Prefix}.tag.dic";
        using Stream stream = RootAssembly.GetManifestResourceStream(key)!;
        Dictionary<int, string> tagMap = ReadLines(stream)
            .Select(x => x.Split('\t'))
            .GroupBy(x => int.Parse(x[0]))
            .ToDictionary(k => k.Key, v => v.Last()[1]);

        int maxTag = tagMap.Keys.Max();
        string[] result = new string[maxTag + 1];
        foreach (KeyValuePair<int, string> item in tagMap)
        {
            result[item.Key] = item.Value;
        }
        return result;
    }

    /// <summary>
    /// 从流中逐行读取数据。
    /// </summary>
    /// <param name="stream">要读取数据的流。</param>
    /// <returns>包含每一行数据的可枚举集合。</returns>
    internal static IEnumerable<string> ReadLines(Stream stream)
    {
        using StreamReader reader = new(stream);
        while (!reader.EndOfStream)
        {
            yield return reader.ReadLine()!;
        }
    }

    /// <summary>
    /// 转换嵌入资源的名称，将连字符转换为下划线，并将“.0”替换为“._0”。
    /// </summary>
    /// <param name="name">资源的原始名称。</param>
    /// <returns>转换后的资源名称。</returns>
    public static string EmbeddedResourceTransform(string name) => name.Replace('-', '_').Replace(".0", "._0");

    /// <summary>
    /// 从指定的程序集中读取嵌入资源作为字节数组。
    /// </summary>
    /// <param name="key">嵌入资源的标识符（名称）。</param>
    /// <returns>表示嵌入资源内容的字节数组。</returns>
    /// <exception cref="System.Exception">当指定的嵌入资源不存在于程序集中时抛出异常。</exception>
    public static byte[] ReadResourceAsBytes(string key)
    {
        using Stream? stream = RootAssembly.GetManifestResourceStream(key)
            ?? throw new Exception($"无法从程序集中加载模型嵌入资源 {key}，模型不存在？");
        using MemoryStream ms = new();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
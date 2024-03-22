using Sdcb.PaddleInference;
using Sdcb.PaddleNLP.Lac.Details;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdcb.PaddleNLP.Lac;

/// <summary>
/// 用于处理中文文本的类，实现了分词和标签标注的功能，并支持自动资源管理。
/// </summary>
/// <remarks>
/// 使用详细配置创建一个中文文本处理器的实例。
/// </remarks>
/// <param name="config">Paddle配置。</param>
/// <param name="tokenMap">token词汇表对照。</param>
/// <param name="q2b">繁体字对照表。</param>
/// <param name="tagMap">标签映射数组。</param>
public class ChineseSegmenter(PaddleConfig config, Dictionary<string, int> tokenMap, Dictionary<string, string> q2b, string[] tagMap) : IDisposable
{
    private PaddleConfig _config = config;
    private readonly Dictionary<string, int> _tokenMap = tokenMap;
    private readonly Dictionary<string, string> _q2b = q2b;
    private readonly string[] _tagMap = tagMap;

    /// <summary>
    /// 是否已释放资源。
    /// </summary>
    public bool Disposed => _config == null;

    /// <summary>
    /// 使用指定的Paddle设备配置创建一个中文文本处理器的实例。
    /// </summary>
    /// <param name="paddleDevice">用于配置Paddle设备，详见<see cref="PaddleDevice"/>，如果为<c>null</c>，则使用<see cref="PaddleDevice.Onnx"/>。</param>
    public ChineseSegmenter(Action<PaddleConfig>? paddleDevice = null) : this(
        SharedUtils.CreateLacConfig().Apply(paddleDevice ?? PaddleDevice.Onnx()),
        SharedUtils.LoadTokenMap(),
        SharedUtils.LoadQ2B(),
        SharedUtils.LoadTagMap())
    {
    }

    /// <summary>
    /// 对单个文本进行分词。
    /// </summary>
    /// <param name="text">需要分词的文本。</param>
    /// <returns>分词后的结果，一个包含分词的字符串数组。</returns>
    public string[] Segment(string text)
    {
        return SegmentAll([text])[0];
    }

    /// <summary>
    /// 对多个文本进行分词。
    /// </summary>
    /// <param name="inputTexts">需要分词的文本数组。</param>
    /// <returns>每个文本分词后的结果数组。</returns>
    public string[][] SegmentAll(string[] inputTexts)
    {
        return TaggingAll(inputTexts)
            .Select(x => x.Select(y => y.Word).ToArray())
            .ToArray();
    }

    /// <summary>
    /// 对单个文本进行分词并标注词性。
    /// </summary>
    /// <param name="inputText">需要处理的文本。</param>
    /// <returns>包含词和对应标签的数组。</returns>
    public WordAndTag[] Tagging(string inputText)
    {
        return TaggingAll([inputText])[0];
    }

    /// <summary>
    /// 对多个文本进行分词并标注词性。
    /// </summary>
    /// <param name="inputTexts">需要处理的文本数组。</param>
    /// <returns>每个文本处理的结果，是一个二维数组。</returns>
    public WordAndTag[][] TaggingAll(string[] inputTexts)
    {
        int maxLength = inputTexts.Max(x => x.Length);
        long[] tokens = inputTexts
            .Select(input => input
                .Select(c => _tokenMap[_q2b.TryGetValue(c.ToString(), out string value) ? value : c.ToString()])
                .ToArray())
            .Aggregate(Enumerable.Empty<long>(), (a, b) => a.Concat([.. b, .. new long[maxLength - b.Length]]))
            .ToArray();
        using PaddlePredictor pred = _config.CreatePredictorNoDelete();
        using (PaddleTensor inputTensor = pred.GetInputTensor("token_ids"))
        {
            inputTensor.Shape = [inputTexts.Length, maxLength];
            inputTensor.SetData(tokens);
        }
        using (PaddleTensor inputTensor = pred.GetInputTensor("length"))
        {
            inputTensor.Shape = [inputTexts.Length];
            inputTensor.SetData(inputTexts.Select(x => (long)x.Length).ToArray());
        }
        if (!pred.Run()) throw new Exception("Failed to run predictor.");

        int[] resultTokens = null!;
        using (PaddleTensor outputTensor = pred.GetOutputTensor(pred.OutputNames[0]))
        {
            resultTokens = Array.ConvertAll(outputTensor.GetData<long>(), x => (int)x);
        }

        int[][] tags = resultTokens
            .Chunk(maxLength)
            .ToArray();

        return tags.Select((x, i) => ToSentOut(tags[i], _tagMap, inputTexts[i]))
            .ToArray();
    }

    static WordAndTag[] ToSentOut(int[] tags, string[] tagMap, string input)
    {
        List<string> sentOut = [];
        List<int> tagsOut = [];
        string partialWord = string.Empty;

        for (int ind = 0; ind < input.Length; ind++)
        {
            int tag = tags[ind];
            string tagName = tagMap[tag];
            char c = input[ind];

            if (string.IsNullOrEmpty(partialWord))
            {
                partialWord = c.ToString();
                tagsOut.Add(tag);
                continue;
            }

            if (tagName.EndsWith("-B") || (tagName == "O" && tagMap[tags[ind - 1]] != "O"))
            {
                sentOut.Add(partialWord);
                tagsOut.Add(tag % 2 == 0 ? tag : tag - 1);
                partialWord = c.ToString();
            }
            else
            {
                partialWord += c;
            }
        }
        if (!string.IsNullOrEmpty(partialWord))
        {
            sentOut.Add(partialWord);
        }

        WordAndTag[] result = new WordAndTag[sentOut.Count];
        for (int i = 0; i < sentOut.Count; i++)
        {
            string word = sentOut[i];
            string label = tagMap[tagsOut[i]];
            result[i] = new WordAndTag(word, label, (WordTag)tagsOut[i]);
        }
        return result;
    }

    /// <summary>
    /// 释放由<see cref="ChineseSegmenter"/>占用的资源。
    /// </summary>
    public void Dispose()
    {
        // 执行实际的资源释放操作
        Dispose(true);
        // 防止垃圾回收器再次调用Finalize
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放由对象占用的非托管资源，并可选择性地释放托管资源。
    /// </summary>
    /// <param name="disposing">如果设置为true，则同时释放托管资源和非托管资源；如果为false，则仅释放非托管资源。</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!Disposed)
        {
            if (disposing)
            {
                // 释放托管资源
                _config.Dispose();
                _config = null!;
            }
            // 释放非托管资源
        }
    }

    /// <summary>
    /// 析构函数
    /// </summary>
    ~ChineseSegmenter()
    {
        // 不直接释放资源，而是调用Dispose(false)
        Dispose(false);
    }
}

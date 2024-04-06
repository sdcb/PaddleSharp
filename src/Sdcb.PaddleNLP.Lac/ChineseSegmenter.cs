using Sdcb.PaddleInference;
using Sdcb.PaddleNLP.Lac.Details;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdcb.PaddleNLP.Lac;

/// <summary>
/// 实现对中文文本的处理，包括分词和词性标注功能。此类通过集成PaddlePaddle模型来实现高效率的文本分析，并提供自动资源管理支持。
/// </summary>
/// <param name="lacOptions">指定分词和标注过程中使用的配置选项，<see cref="LacOptions"/>定义了模型的主要配置。如果未指定，则使用默认配置<see cref="LacOptions.Default"/>。</param>
/// <param name="paddleDevice">用于配置和初始化PaddlePaddle运算设备。接收一个<see cref="PaddleConfig"/>通过<paramref name="paddleDevice"/>动作进行配置。如果设为<c>null</c>，则默认使用<see cref="PaddleDevice.Onnx"/>作为运算设备。</param>
public class ChineseSegmenter(LacOptions? lacOptions = null, Action<PaddleConfig>? paddleDevice = null) : IDisposable
{
    private PaddlePredictor _config = SharedUtils.CreateLacConfig().Apply(paddleDevice ?? PaddleDevice.Onnx()).CreatePredictor();
    private readonly LacOptions _lacOptions = lacOptions ?? LacOptions.Default;

    /// <summary>
    /// 是否已释放资源。
    /// </summary>
    public bool Disposed => _config == null;

    /// <summary>
    /// 对单个文本进行分词。
    /// </summary>
    /// <param name="text">需要分词的文本。</param>
    /// <returns>分词后的结果，一个包含分词的字符串数组。</returns>
    public string[] Segment(string text)
    {
        ThrowIfDisposed();
        return SegmentAll([text])[0];
    }

    /// <summary>
    /// 对多个文本进行分词。
    /// </summary>
    /// <param name="inputTexts">需要分词的文本数组。</param>
    /// <returns>每个文本分词后的结果数组。</returns>
    public string[][] SegmentAll(string[] inputTexts)
    {
        ThrowIfDisposed();

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
        ThrowIfDisposed();
        return TaggingAll([inputText])[0];
    }

    /// <summary>
    /// 对多个文本进行分词并标注词性。
    /// </summary>
    /// <param name="inputTexts">需要处理的文本数组。</param>
    /// <returns>每个文本处理的结果，是一个二维数组。</returns>
    public WordAndTag[][] TaggingAll(string[] inputTexts)
    {
        ThrowIfDisposed();

        int maxLength = inputTexts.Max(x => x.Length);
        long[] tokens = inputTexts
            .Select(input => input
                .Select(_lacOptions.TransformChar)
                .ToArray())
            .Aggregate(Enumerable.Empty<long>(), (a, b) => a.Concat([.. b, .. new long[maxLength - b.Length]]))
            .ToArray();
        using PaddlePredictor pred = _config.Clone();
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

        int[][] allTags = resultTokens
            .Chunk(maxLength)
            .ToArray();

        return allTags.Select((tags, i) =>
        {
            string input = inputTexts[i];
            int[] transformedTags = _lacOptions.TransformCustomizedWords(input, tags);
            return ToSentOut(transformedTags, _lacOptions.TagMap, input);
        })
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
            string labelPrefix = label[..label.IndexOf('-')];
            result[i] = new WordAndTag(word, labelPrefix, (WordTag)tagsOut[i]);
        }
        return result;
    }

    void ThrowIfDisposed()
    {
        if (Disposed) throw new ObjectDisposedException(nameof(ChineseSegmenter));
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

using Sdcb.PaddleInference;
using Sdcb.PaddleNLP.Lac.Details;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdcb.PaddleNLP.Lac;

public class ChineseTextProcessor : IDisposable
{
    private readonly PaddleConfig _config;
    private readonly Dictionary<string, int> _tokenMap;
    private readonly Dictionary<string, string> _q2b;
    private readonly Dictionary<int, string> _tagMap;

    public ChineseTextProcessor()
    {
        _config = SharedUtils.CreateLacConfig();
        _tokenMap = SharedUtils.LoadTokenMap();
        _q2b = SharedUtils.LoadQ2B();
        _tagMap = SharedUtils.LoadTagMap();
    }

    public ChineseTextProcessor(PaddleConfig config, Dictionary<string, int> tokenMap, Dictionary<string, string> q2b, Dictionary<int, string> tagMap)
    {
        _config = config;
        _tokenMap = tokenMap;
        _q2b = q2b;
        _tagMap = tagMap;
    }

    public string[] Segment(string text)
    {
        throw new NotImplementedException();
    }

    public string[][] SegmentAll(string[] inputTexts)
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

        long[] resultTokens = null!;
        using (PaddleTensor outputTensor = pred.GetOutputTensor(pred.OutputNames[0]))
        {
            resultTokens = [.. outputTensor.GetData<long>()];
        }

        string[][] tags = resultTokens
            .Chunk(maxLength)
            .Select(x => x.Select(v => _tagMap[(int)v]).ToArray())
            .ToArray();

        tags.Select((x, i) => ToSentOut(tags[i], inputTexts[i])).Dump();
    }

    public WordAndTag[] Tagging(string text)
    {
        throw new NotImplementedException();
    }

    static List<string> ToSentOut(string[] tags, string input)
    {
        List<string> sentOut = [];
        List<string> tagsOut = [];
        string partialWord = string.Empty;

        for (int ind = 0; ind < input.Length; ind++)
        {
            string tag = tags[ind];
            char c = input[ind];

            if (string.IsNullOrEmpty(partialWord))
            {
                partialWord = c.ToString();
                tagsOut.Add(tag.Split('-')[0]);
                continue;
            }

            if (tag.EndsWith("-B") || (tag == "O" && tags[ind - 1] != "O"))
            {
                sentOut.Add(partialWord);
                tagsOut.Add(tag.Split('-')[0]);
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

        return sentOut;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

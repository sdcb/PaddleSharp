# Sdcb.PaddleNLP.Lac分词模型

## PaddleNLP Lac模型NuGet包

| 包名 💼            | 版本号 📌                                                                                                        | 描述 📚   |
| ------------------ | ---------------------------------------------------------------------------------------------------------------- | --------- |
| Sdcb.PaddleNLP.Lac | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleNLP.Lac.svg)](https://nuget.org/packages/Sdcb.PaddleNLP.Lac) | 模型自包含 |

# 使用方法及示例

## 需要安装的NuGet包
* Sdcb.PaddleNLP.Lac
* Sdcb.PaddleInference
* Sdcb.PaddleInference.runtime.win64.mkl

## 示例
## 1. 最简单的分词：
```csharp
string input = "我是中国人，我爱我的祖国。";
using ChineseSegmenter segmenter = new();
string[] result = segmenter.Segment(input);
Console.WriteLine(string.Join(",", result)); // 我,是,中国,人,，,我,爱,我的祖国,。
```

## 2. 词性标注：
```csharp
string input = "我爱北京天安门";
using ChineseSegmenter segmenter = new();
WordAndTag[] result = segmenter.Tagging(input);
string labels = string.Join(",", result.Select(x => x.Label));
string words = string.Join(",", result.Select(x => x.Word));
string tags = string.Join(",", result.Select(x => x.Tag));
Console.WriteLine(words);  // 我,爱,北京,天安门
Console.WriteLine(labels); // r,v,LOC,LOC
Console.WriteLine(tags);   // Pronoun,Verb,LocationName,LocationName
```

## 3. 自定义词库

```csharp
string input = "我爱北京天安门";
using ChineseSegmenter segmenter = new(new ()
{
    CustomDictionary = new()
    {
        { "北京天安门", WordTag.LocationName },
    }
});
WordAndTag[] result = segmenter.Tagging(input);
string labels = string.Join(",", result.Select(x => x.Label));
string words = string.Join(",", result.Select(x => x.Word));
string tags = string.Join(",", result.Select(x => x.Tag));
Console.WriteLine(words);  // 我,爱,北京天安门
Console.WriteLine(labels); // r,v,LOC
Console.WriteLine(tags);   // Pronoun,Verb,LocationName
```
namespace Sdcb.PaddleNLP.Lac;

/// <summary>
/// 词性标注枚举
/// </summary>
public enum WordTag
{
    /// <summary>形容词</summary>
    Adjective = 0,

    /// <summary>副形词</summary>
    AdverbialAdjective = 2,

    /// <summary>名形词</summary>
    NominalAdjective = 4,

    /// <summary>连词</summary>
    Conjunction = 6,

    /// <summary>副词</summary>
    Adverb = 8,

    /// <summary>方位名词</summary>
    DirectionalNoun = 10,

    /// <summary>数量词</summary>
    MeasureWord = 12,

    /// <summary>普通名词</summary>
    CommonNoun = 14,

    /// <summary>人名</summary>
    PersonalName = 16,

    /// <summary>地名</summary>
    LocationName = 18,

    /// <summary>机构名</summary>
    OrganizationName = 20,

    /// <summary>作品名</summary>
    WorkName = 22,

    /// <summary>其他专名</summary>
    OtherProperNoun = 24,

    /// <summary>介词</summary>
    Preposition = 26,

    /// <summary>量词</summary>
    QuantityWord = 28,

    /// <summary>代词</summary>
    Pronoun = 30,

    /// <summary>处所名词</summary>
    PlaceNoun = 32,

    /// <summary>时间</summary>
    Time = 34,

    /// <summary>助词</summary>
    AuxiliaryWord = 36,

    /// <summary>普通动词</summary>
    Verb = 38,

    /// <summary>动副词</summary>
    VerbAdverb = 40,

    /// <summary>名动词</summary>
    VerbalNoun = 42,

    /// <summary>标点符号</summary>
    Punctuation = 44,

    /// <summary>其他虚词</summary>
    OtherFunctionWord = 46,

    /// <summary>其它</summary>
    Other = 48
}

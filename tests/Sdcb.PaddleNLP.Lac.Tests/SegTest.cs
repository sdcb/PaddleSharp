namespace Sdcb.PaddleNLP.Lac.Tests;

public class SegTest
{
    [Fact]
    public void Normal()
    {
        string input = "我是中国人，我爱我的祖国。";
        using ChineseSegmenter segmenter = new();
        string[] result = segmenter.Segment(input);
        Assert.Equal(new string[] { "我", "是", "中国", "人", "，", "我", "爱", "我的祖国", "。" }, result);
    }

    [Fact]
    public void Tag()
    {
        string input = "我爱北京天安门";
        using ChineseSegmenter segmenter = new();
        WordAndTag[] result = segmenter.Tagging(input);
        string labels = string.Join(",", result.Select(x => x.Label));
        string words = string.Join(",", result.Select(x => x.Word));
        string tags = string.Join(",", result.Select(x => x.Tag));
        Assert.Equal("我,爱,北京,天安门", words);
        Assert.Equal("r,v,LOC,LOC", labels);
        Assert.Equal("Pronoun,Verb,LocationName,LocationName", tags);
    }

    [Fact]
    public void LongTextCheck()
    {
        string input = """
            很久以前，在一个遥远的森林中，生活着三只小猪兄弟。他们决定离开妈妈的怀抱，各自建立自己的家。三只小猪性格迥异，选择的建筑材料也体现了他们的不同。

            最大的那只小猪认为速度至上，于是他用稻草迅速搭建了一座房子。这座房子虽然建造迅速，但在结构上显得非常脆弱，很容易被风吹垮。中间的那只小猪倾向于选择稳定性较好的材料，他选择了木头，费了一番功夫，建了一座比较稳固的木屋。最小的那只小猪最为谨慎，他决定使用坚固耐用的砖头，虽然花费了最多的时间和劳力，但建立起了最坚不可摧的砖石房屋。

            一天，一只凶猛的大灰狼闯进了小猪们的村子，它饥饿难耐，盯上了三只小猪为目标。它首先找到了用稻草建的房子。在这座脆弱的房子面前，它用力一吹，稻草房子应声倒塌。第一只小猪惊慌失措，幸亏及时逃到了第二只小猪的木屋中。大灰狼不甘心，紧接着来到了木屋面前。它运用了更大的力量，不断地撞击，终于，木头房子也抵挡不住，嘎吱一声，倒在了大灰狼的脚下。两只小猪忐忑不安，互相依偎，紧急逃到了第三只小猪的砖房里。

            大灰狼紧追不舍，但当它来到砖房面前时，遭遇了前所未有的挑战。无论它如何用力呼吸，如何猛烈冲击，那座砖房就像一座坚固的堡垒，纹丝不动。愤怒至极的大灰狼，企图从烟囱处潜入房内。但小猪们早已做好了准备，他们在火炉中燃起了旺火。大灰狼一头扎进去，被火焰烫伤，疼痛难忍，只得叫着逃走了。

            这次经历让三只小猪深刻认识到，面对困难和危险，不仅要有预见性的准备，还需要团结互助。他们在砖房里过上了幸福安稳的生活，再也不用担心大灰狼的威胁。岁月流转，这个故事成为了村庄中流传的佳话，提醒着每一代小猪和其他小动物，只有合作和智慧，才能使生活更美好，将危险拒之门外。
            """;
        using ChineseSegmenter segmenter = new();
        WordAndTag[] result = segmenter.Tagging(input);
    }
}

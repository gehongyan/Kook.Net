using Xunit;
using KaiHeiLa.Rest;

namespace KaiHeiLa;

/// <summary>
///     Tests for <see cref="MessageHelper"/> parsing.
/// </summary>
public class MessageHelperTests
{
    /// <summary>
    ///     Tests that no tags are parsed while in code blocks
    ///     or inline code.
    /// </summary>
    [Theory]
    [InlineData("`c`")]
    [InlineData("`(met)1896684851(met)`")]
    [InlineData("```(met)all(met)```")]
    [InlineData("```cs \n (met)all(met)```")]
    [InlineData("```cs (rol)1896684851(rol) ```")]
    [InlineData("``` test ``` ```cs (rol)1896684851(rol) ```")]
    [InlineData("`(emj)test(emj)[1990044438283387/aIVQrtPv4z10b10b]`")]
    [InlineData("``` (met)all(met)  `")] // KaiHeiLa client doesn't handles this
    [InlineData("``` (met)all(met)  ``")]
    [InlineData("` (met)here(met) `")]
    [InlineData("` (met)all(met) (met)here(met) (met)1896684851(met) (rol)1896684851(rol) (chn)1896684851(chn) (emj)test(emj)[1990044438283387/aIVQrtPv4z10b10b] `")]
    public void ParseTagsInCode(string testData)
    {
        var result = MessageHelper.ParseTags(testData, null, null, null, TagMode.KMarkdown);
        Assert.Empty(result);
    }

    /// <summary> Tests parsing tags that surround inline code or a code block. </summary>
    [Theory]
    [InlineData("`` (rol)1896684851(rol)")]
    [InlineData("``` code block 1 ``` ``` code block 2 ``` (rol)1896684851(rol)")]
    [InlineData("` code block 1 ``` ` code block 2 ``` (rol)1896684851(rol)")]
    [InlineData("(rol)1896684851(rol) ``` code block 1 ```")]
    [InlineData("``` code ``` ``` code ``` (met)here(met) ``` code ``` ``` more ```")]
    [InlineData("``` code ``` (met)here(met) ``` more ```")]
    public void ParseTagsAroundCode(string testData)
    {
        var result = MessageHelper.ParseTags(testData, null, null, null, TagMode.KMarkdown);
        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData(@"\` (met)all(met) \`")]
    [InlineData(@"\`\`\` (met)all(met) \`\`\`")]
    [InlineData(@"hey\`\`\`(met)all(met)\`\`\`!!")]
    public void IgnoreEscapedCodeBlocks(string testData)
    {
        var result = MessageHelper.ParseTags(testData, null, null, null, TagMode.KMarkdown);
        Assert.NotEmpty(result);
    }

    // cannot test parsing a user, as it uses the ReadOnlyCollection<IUser> arg.
    // this could be done if mocked entities are merged in PR #1290

    /// <summary> Tests parsing a mention of a role. </summary>
    [Theory]
    [InlineData("(rol)1896684851(rol)")]
    [InlineData("**(rol)1896684851(rol)**")]
    [InlineData("__(rol)1896684851(rol)__")]
    [InlineData("<>(rol)1896684851(rol)")]
    public void ParseRole(string roleTag)
    {
        var result = MessageHelper.ParseTags(roleTag, null, null, null, TagMode.KMarkdown);
        Assert.Contains(result, x => x.Type == TagType.RoleMention);
    }

    /// <summary> Tests parsing a channel. </summary>
    [Theory]
    [InlineData("(chn)1896684851(chn)")]
    [InlineData("**(chn)1896684851(chn)**")]
    [InlineData("<>(chn)1896684851(chn)")]
    public void ParseChannel(string channelTag)
    {
        var result = MessageHelper.ParseTags(channelTag, null, null, null, TagMode.KMarkdown);
        Assert.Contains(result, x => x.Type == TagType.ChannelMention);
    }

    /// <summary> Tests parsing an emoji. </summary>
    [Theory]
    [InlineData("(emj)test(emj)[1990044438283387/aIVQrtPv4z10b10b]")]
    [InlineData("**(emj)test(emj)[1990044438283387/aIVQrtPv4z10b10b]**")]
    [InlineData("<>(emj)test(emj)[1990044438283387/aIVQrtPv4z10b10b]")]
    public void ParseEmoji(string emoji)
    {
        var result = MessageHelper.ParseTags(emoji, null, null, null, TagMode.KMarkdown);
        Assert.Contains(result, x => x.Type == TagType.Emoji);
    }

    /// <summary> Tests parsing a mention of @everyone. </summary>
    [Theory]
    [InlineData("(met)all(met)")]
    [InlineData("**(met)all(met)**")]
    public void ParseEveryone(string everyone)
    {
        var result = MessageHelper.ParseTags(everyone, null, null, null, TagMode.KMarkdown);
        Assert.Contains(result, x => x.Type == TagType.EveryoneMention);
    }

    /// <summary> Tests parsing a mention of @here. </summary>
    [Theory]
    [InlineData("(met)here(met)")]
    [InlineData("**(met)here(met)**")]
    public void ParseHere(string here)
    {
        var result = MessageHelper.ParseTags(here, null, null, null, TagMode.KMarkdown);
        Assert.Contains(result, x => x.Type == TagType.HereMention);
    }
}
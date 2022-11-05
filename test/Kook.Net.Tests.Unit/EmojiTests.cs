using System;
using Xunit;

namespace Kook;

[Trait("Category", "Unit")]
public class EmojiTests
{
    [Theory]
    [InlineData("👍")]
    [InlineData("\ud83d\udc4d")]
    [InlineData(":+1:")]
    public void Test_Emoji_Parse(string input)
    {
        Assert.True(Emoji.TryParse(input, out Emoji emoji));
        Assert.NotNull(emoji);
        Assert.Equal(emoji.Name, emoji.Id);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("\u0000\u0000")]
    [InlineData("+1")]
    public void Test_Invalid_Emoji_Parse(string input)
    {
        Assert.False(Emoji.TryParse(input, out _));
        Assert.Throws<FormatException>(() => Emoji.Parse(input));
    }
    
    [Theory]
    [InlineData("[:test:1990044438283387/aIVQrtPv4z10b10b]", "test", "1990044438283387/aIVQrtPv4z10b10b", TagMode.PlainText)]
    [InlineData("(emj)test(emj)[1990044438283387/aIVQrtPv4z10b10b]", "test", "1990044438283387/aIVQrtPv4z10b10b", TagMode.KMarkdown)]
    public void Test_Emote_Parse(string input, string name, string id, TagMode tagMode)
    {
        Assert.True(Emote.TryParse(input, out Emote emote, tagMode));
        Assert.NotNull(emote);
        Assert.Equal(name, emote.Name);
        Assert.Equal(id, emote.Id);
    }

    [Theory]
    [InlineData("invalid", TagMode.PlainText)]
    [InlineData("invalid", TagMode.KMarkdown)]
    [InlineData("[:test:not_a_number/aIVQrtPv4z10b10b]", TagMode.PlainText)]
    [InlineData("(emj)test(emj)[not_a_number/aIVQrtPv4z10b10b]", TagMode.KMarkdown)]
    public void Test_Invalid_Emote_Parse(string input, TagMode tagMode)
    {
        Assert.False(Emote.TryParse(input, out _, tagMode));
        Assert.Throws<ArgumentException>(() => Emote.Parse(input, tagMode));
    }
}
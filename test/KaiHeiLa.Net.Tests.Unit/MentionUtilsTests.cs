using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace KaiHeiLa;

/// <summary>
///     Tests the methods provided in <see cref="MentionUtils"/>.
/// </summary>
public class MentionUtilsTests
{
    /// <summary>
    ///     Tests <see cref="MentionUtils.PlainTextMentionUser(string,string)"/>
    ///     and <see cref="MentionUtils.KMarkdownMentionUser(string)"/>
    /// </summary>
    [Fact]
    public void MentionUser()
    {
        Assert.Equal("@test_username#123", MentionUtils.PlainTextMentionUser("test_username", 123u));
        Assert.Equal("(met)123(met)", MentionUtils.KMarkdownMentionUser(123u));
        Assert.Equal("@test_username#123", MentionUtils.PlainTextMentionUser("test_username", "123"));
        Assert.Equal("(met)123(met)", MentionUtils.KMarkdownMentionUser("123"));
    }
    /// <summary>
    ///     Tests <see cref="MentionUtils.PlainTextMentionChannel(string)"/>
    ///     and <see cref="MentionUtils.KMarkdownMentionChannel(string)"/>
    /// </summary>
    [Fact]
    public void MentionChannel()
    {
        Assert.Equal("#channel:123;", MentionUtils.PlainTextMentionChannel(123u));
        Assert.Equal("(chn)123(chn)", MentionUtils.KMarkdownMentionChannel(123u));
        Assert.Equal("#channel:123;", MentionUtils.PlainTextMentionChannel("123"));
        Assert.Equal("(chn)123(chn)", MentionUtils.KMarkdownMentionChannel("123"));
    }
    /// <summary>
    ///     Tests <see cref="MentionUtils.PlainTextMentionRole(string)"/>
    ///     and <see cref="MentionUtils.KMarkdownMentionRole(string)"/>
    /// </summary>
    [Fact]
    public void MentionRole()
    {
        Assert.Equal("@role:123;", MentionUtils.PlainTextMentionRole(123u));
        Assert.Equal("(rol)123(rol)", MentionUtils.KMarkdownMentionRole(123u));
        Assert.Equal("@role:123;", MentionUtils.PlainTextMentionRole("123"));
        Assert.Equal("(rol)123(rol)", MentionUtils.KMarkdownMentionRole("123"));
    }
    [Theory]
    [InlineData("@test_username#123", TagMode.PlainText, 123)]
    [InlineData("(met)123(met)", TagMode.KMarkdown, 123)]
    public void ParseUser_Pass(string user, TagMode tagMode, ulong id)
    {
        var parsed = MentionUtils.ParseUser(user, tagMode);
        Assert.Equal(id, parsed);

        Assert.True(MentionUtils.TryParseUser(user, out ulong result, tagMode));
        Assert.Equal(id, result);
    }
    [Theory]
    [InlineData(" ", TagMode.PlainText)]
    [InlineData(" ", TagMode.KMarkdown)]
    [InlineData("invalid", TagMode.PlainText)]
    [InlineData("invalid", TagMode.KMarkdown)]
    [InlineData("@#", TagMode.PlainText)]
    [InlineData("(met)(met)", TagMode.KMarkdown)]
    [InlineData("@test_username#not_a_number", TagMode.PlainText)]
    [InlineData("(met)not_a_number(met)", TagMode.KMarkdown)]
    public void ParseUser_Fail(string user, TagMode tagMode)
    {
        Assert.Throws<ArgumentException>(() => MentionUtils.ParseUser(user, tagMode));
        Assert.False(MentionUtils.TryParseUser(user, out _, tagMode));
    }
    [Fact]
    public void ParseUser_Null()
    {
        Assert.Throws<ArgumentNullException>(() => MentionUtils.ParseUser(null, TagMode.PlainText));
        Assert.Throws<ArgumentNullException>(() => MentionUtils.ParseUser(null, TagMode.KMarkdown));
        Assert.Throws<ArgumentNullException>(() => MentionUtils.TryParseUser(null, out _, TagMode.PlainText));
        Assert.Throws<ArgumentNullException>(() => MentionUtils.TryParseUser(null, out _, TagMode.KMarkdown));
    }
    [Theory]
    [InlineData("#channel:123;", TagMode.PlainText, 123)]
    [InlineData("(chn)123(chn)", TagMode.KMarkdown, 123)]
    public void ParseChannel_Pass(string channel, TagMode tagMode, ulong id)
    {
        var parsed = MentionUtils.ParseChannel(channel, tagMode);
        Assert.Equal(id, parsed);

        Assert.True(MentionUtils.TryParseChannel(channel, out ulong result, tagMode));
        Assert.Equal(id, result);
    }
    [Theory]
    [InlineData(" ", TagMode.PlainText)]
    [InlineData(" ", TagMode.KMarkdown)]
    [InlineData("invalid", TagMode.PlainText)]
    [InlineData("invalid", TagMode.KMarkdown)]
    [InlineData("#channel:;", TagMode.PlainText)]
    [InlineData("(chn)(chn)", TagMode.KMarkdown)]
    [InlineData("#channel:not_a_number;", TagMode.PlainText)]
    [InlineData("(chn)not_a_number(chn)", TagMode.KMarkdown)]
    public void ParseChannel_Fail(string channel, TagMode tagMode)
    {
        Assert.Throws<ArgumentException>(() => MentionUtils.ParseChannel(channel, tagMode));
        Assert.False(MentionUtils.TryParseChannel(channel, out _, tagMode));
    }
    [Fact]
    public void ParseChannel_Null()
    {
        Assert.Throws<ArgumentNullException>(() => MentionUtils.ParseChannel(null, TagMode.PlainText));
        Assert.Throws<ArgumentNullException>(() => MentionUtils.ParseChannel(null, TagMode.KMarkdown));
        Assert.Throws<ArgumentNullException>(() => MentionUtils.TryParseChannel(null, out _, TagMode.PlainText));
        Assert.Throws<ArgumentNullException>(() => MentionUtils.TryParseChannel(null, out _, TagMode.KMarkdown));
    }
    [Theory]
    [InlineData("@role:123;", TagMode.PlainText, 123)]
    [InlineData("(rol)123(rol)", TagMode.KMarkdown, 123)]
    public void ParseRole_Pass(string role, TagMode tagMode, uint id)
    {
        var parsed = MentionUtils.ParseRole(role, tagMode);
        Assert.Equal(id, parsed);

        Assert.True(MentionUtils.TryParseRole(role, out uint result, tagMode));
        Assert.Equal(id, result);
    }
    [Theory]
    [InlineData(" ", TagMode.PlainText)]
    [InlineData(" ", TagMode.KMarkdown)]
    [InlineData("invalid", TagMode.PlainText)]
    [InlineData("invalid", TagMode.KMarkdown)]
    [InlineData("@role:;", TagMode.PlainText)]
    [InlineData("(rol)(rol)", TagMode.KMarkdown)]
    [InlineData("@role:not_a_number;", TagMode.PlainText)]
    [InlineData("(rol)not_a_number(rol)", TagMode.KMarkdown)]
    public void ParseRole_Fail(string role, TagMode tagMode)
    {
        Assert.Throws<ArgumentException>(() => MentionUtils.ParseRole(role, tagMode));
        Assert.False(MentionUtils.TryParseRole(role, out _, tagMode));
    }
    [Fact]
    public void ParseRole_Null()
    {
        Assert.Throws<ArgumentNullException>(() => MentionUtils.ParseRole(null, TagMode.PlainText));
        Assert.Throws<ArgumentNullException>(() => MentionUtils.ParseRole(null, TagMode.KMarkdown));
        Assert.Throws<ArgumentNullException>(() => MentionUtils.TryParseRole(null, out _, TagMode.PlainText));
        Assert.Throws<ArgumentNullException>(() => MentionUtils.TryParseRole(null, out _, TagMode.KMarkdown));
    }
}
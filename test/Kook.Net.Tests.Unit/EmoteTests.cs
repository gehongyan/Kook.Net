using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Kook;

[Trait("Category", "Unit")]
public class EmoteTests
{
    [Fact]
    public void Test_Emote_Parse()
    {
        Assert.True(Emote.TryParse("[:test:1990044438283387/aIVQrtPv4z10b10b]", out Emote emote, TagMode.PlainText));
        Assert.NotNull(emote);
        Assert.Equal("test", emote.Name);
        Assert.Equal("1990044438283387/aIVQrtPv4z10b10b", emote.Id);
        Assert.True(Emote.TryParse("(emj)test(emj)[1990044438283387/aIVQrtPv4z10b10b]", out emote, TagMode.KMarkdown));
        Assert.NotNull(emote);
        Assert.Equal("test", emote.Name);
        Assert.Equal("1990044438283387/aIVQrtPv4z10b10b", emote.Id);
    }
    [Fact]
    public void Test_Invalid_Emote_Parse()
    {
        Assert.False(Emote.TryParse("invalid", out _, TagMode.PlainText));
        Assert.False(Emote.TryParse("invalid", out _, TagMode.KMarkdown));
        Assert.False(Emote.TryParse("[:test:not_a_number/aIVQrtPv4z10b10b]", out _, TagMode.PlainText));
        Assert.False(Emote.TryParse("(emj)test(emj)[not_a_number/aIVQrtPv4z10b10b]", out _, TagMode.KMarkdown));
        Assert.Throws<ArgumentException>(() => Emote.Parse("invalid", TagMode.PlainText));
        Assert.Throws<ArgumentException>(() => Emote.Parse("invalid", TagMode.KMarkdown));
    }

    // [Fact]
    // public void ExportAllEmotes()
    // {
    //     var emotes = Emoji.NamesAndUnicodes;
    //     var sb = new StringBuilder();
    //     foreach (KeyValuePair<string,string> keyValuePair in emotes)
    //     {
    //         string unicode = Convert.ToHexString(Encoding.Unicode.GetBytes(keyValuePair.Value)).Chunk(2).Select(x => $"{x[0]}{x[1]}")
    //             .Chunk(2).Select(x => x.Reverse().ToList()).Select(x => $"{x[0]}{x[1]}")
    //             .Select(x => $"\\u{x.ToLowerInvariant()}").Aggregate((x, y) => $"{x}{y}");
    //         sb.AppendLine($"|{keyValuePair.Value}|{unicode}|{keyValuePair.Key}|");
    //     }
    //     File.WriteAllText("emotes.txt", sb.ToString());
    // }
}
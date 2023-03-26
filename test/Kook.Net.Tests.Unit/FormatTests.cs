using Xunit;

namespace Kook;

[Trait("Category", "Unit")]
public class FormatTests
{
    [Theory]
    [InlineData("@everyone", "@everyone")]
    [InlineData(@"\", @"\\")]
    [InlineData(@"*text*", @"\*text\*")]
    [InlineData(@"~text~", @"\~text\~")]
    [InlineData(@"`text`", @"\`text\`")]
    [InlineData(@"> text", @"\> text")]
    public void Sanitize(string input, string expected) => Assert.Equal(expected, Format.Sanitize(input));

    [Fact]
    public void Code()
    {
        // no language
        Assert.Equal("`test`", Format.Code("test"));
        Assert.Equal("```\nanother\none\n```", Format.Code("another\none"));
        // language specified
        Assert.Equal("```cs\ntest\n```", Format.Code("test", "cs"));
        Assert.Equal("```cs\nanother\none\n```", Format.Code("another\none", "cs"));
    }

    [Fact]
    public void QuoteNullString() => Assert.Null(Format.Quote(null));

    [Theory]
    [InlineData("", "")]
    [InlineData("\n", "\n")]
    [InlineData("foo\n\nbar", "> foo\n\n\n> bar\n")]
    [InlineData("input", "> input\n")] // single line
    // should work with CR or CRLF
    [InlineData("inb4\ngreentext", "> inb4\ngreentext\n")]
    [InlineData("inb4\r\ngreentext", "> inb4\r\ngreentext\r\n")]
    public void Quote(string input, string expected) => Assert.Equal(expected, Format.Quote(input));

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("\n", "\n")]
    [InlineData("foo\n\nbar", "> foo\n\u200d\nbar")]
    [InlineData("input", "> input")] // single line
    // should work with CR or CRLF
    [InlineData("inb4\ngreentext", "> inb4\ngreentext")]
    [InlineData("inb4\n\ngreentext", "> inb4\n\u200d\ngreentext")]
    [InlineData("inb4\rgreentext", "> inb4\rgreentext")]
    [InlineData("inb4\r\rgreentext", "> inb4\r\u200d\rgreentext")]
    [InlineData("inb4\r\ngreentext", "> inb4\r\ngreentext")]
    [InlineData("inb4\r\n\r\ngreentext", "> inb4\r\n\u200d\r\ngreentext")]
    public void BlockQuote(string input, string expected) => Assert.Equal(expected, Format.BlockQuote(input));

    [Theory]
    [InlineData("", "")]
    [InlineData("\n", "\n")]
    [InlineData("**hi**", "hi")]
    [InlineData(">>uwu", "uwu")]
    [InlineData("```uwu```", "uwu")]
    [InlineData("~uwu~", "uwu")]
    [InlineData("(ins)uwu(ins)", "uwu")]
    [InlineData("(spl)uwu(spl)", "uwu")]
    [InlineData("berries and *Cream**, I'm a little lad who loves berries and cream",
        "berries and Cream, I'm a little lad who loves berries and cream")]
    public void StripMarkdown(string input, string expected)
    {
        string test = Format.StripMarkDown(input);
        Assert.Equal(expected, test);
    }
}

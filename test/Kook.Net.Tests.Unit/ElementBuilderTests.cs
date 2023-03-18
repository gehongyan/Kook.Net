using System;
using System.Collections.Generic;
using Xunit;

namespace Kook;

[Trait("Category", "Unit")]
public class ElementBuilderTests
{
    private const string Name = "Kook.Net";
    private const string Icon = "https://kaiheila.net/logo.png";
    private const string Url = "https://kaiheila.net/";

    [Fact]
    public void PlainTextElementBuilder_Constructor()
    {
        PlainTextElementBuilder builder = new PlainTextElementBuilder().WithContent("content");
        Assert.Equal(ElementType.PlainText, builder.Type);
        Assert.Equal(ElementType.PlainText, builder.Build().Type);
        Assert.Equal("content", builder.Content);
        Assert.Equal("content", builder.Build().Content);
        Assert.True(builder.Emoji);
        Assert.True(builder.Build().Emoji);
        builder = new PlainTextElementBuilder { Content = "text", Emoji = false };
        Assert.Equal(ElementType.PlainText, builder.Type);
        Assert.Equal(ElementType.PlainText, builder.Build().Type);
        Assert.Equal("text", builder.Content);
        Assert.Equal("text", builder.Build().Content);
        Assert.False(builder.Emoji);
        Assert.False(builder.Build().Emoji);
    }

    [Fact]
    public void PlainTextElementBuilder_ValidContent()
    {
        IEnumerable<string> GetValidContent()
        {
            yield return string.Empty;
            yield return " ";
            yield return new string('a', 2000);
        }

        foreach (string content in GetValidContent())
        {
            PlainTextElementBuilder builder = new PlainTextElementBuilder().WithContent(content);
            Assert.Equal(content, builder.Content);
            Assert.Equal(content, builder.Build().Content);
        }
    }

    [Fact]
    public void PlainTextElementBuilder_InvalidContent()
    {
        IEnumerable<string> GetInvalidContent()
        {
            yield return null;
            yield return new string('a', 2001);
        }

        foreach (string content in GetInvalidContent()) Assert.Throws<ArgumentException>(() => new PlainTextElementBuilder().WithContent(content));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("content")]
    public void PlainTextElementBuilder_ImplicitConversion(string content)
    {
        PlainTextElementBuilder builder = content;
        Assert.Equal(content, builder.Content);
        Assert.Equal(content, builder.Build().Content);
    }

    [Fact]
    public void KMarkdownElementBuilder_Constructor()
    {
        KMarkdownElementBuilder builder = new KMarkdownElementBuilder().WithContent("content");
        Assert.Equal(ElementType.KMarkdown, builder.Type);
        Assert.Equal(ElementType.KMarkdown, builder.Build().Type);
        Assert.Equal("content", builder.Content);
        Assert.Equal("content", builder.Build().Content);
        builder = new KMarkdownElementBuilder { Content = "text" };
        Assert.Equal(ElementType.KMarkdown, builder.Type);
        Assert.Equal(ElementType.KMarkdown, builder.Build().Type);
        Assert.Equal("text", builder.Content);
        Assert.Equal("text", builder.Build().Content);
    }

    [Fact]
    public void KMarkdownElementBuilder_ValidContent()
    {
        IEnumerable<string> GetValidContent()
        {
            yield return string.Empty;
            yield return " ";
            yield return new string('a', 5000);
        }

        foreach (string content in GetValidContent())
        {
            KMarkdownElementBuilder builder = new KMarkdownElementBuilder().WithContent(content);
            Assert.Equal(content, builder.Content);
            Assert.Equal(content, builder.Build().Content);
        }
    }

    [Fact]
    public void KMarkdownElementBuilder_InvalidContent()
    {
        IEnumerable<string> GetInvalidContent()
        {
            yield return null;
            yield return new string('a', 5001);
        }

        foreach (string content in GetInvalidContent()) Assert.Throws<ArgumentException>(() => new KMarkdownElementBuilder().WithContent(content));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("content")]
    public void KMarkdownElementBuilder_ImplicitConversion(string content)
    {
        KMarkdownElementBuilder builder = content;
        Assert.Equal(content, builder.Content);
        Assert.Equal(content, builder.Build().Content);
    }

    [Fact]
    public void ImageElementBuilder_Constructor()
    {
        ImageElementBuilder builder = new ImageElementBuilder()
            .WithSource(Icon)
            .WithAlternative(Name)
            .WithSize(ImageSize.Small)
            .WithCircle(true);
        Assert.Equal(ElementType.Image, builder.Type);
        Assert.Equal(ElementType.Image, builder.Build().Type);
        Assert.Equal(Icon, builder.Source);
        Assert.Equal(Icon, builder.Build().Source);
        Assert.Equal(Name, builder.Alternative);
        Assert.Equal(Name, builder.Build().Alternative);
        Assert.Equal(ImageSize.Small, builder.Size);
        Assert.Equal(ImageSize.Small, builder.Build().Size);
        ;
        Assert.Equal(true, builder.Circle);
        Assert.Equal(true, builder.Build().Circle);
        builder = new ImageElementBuilder { Source = Icon, Alternative = Name, Size = ImageSize.Large, Circle = false };
        Assert.Equal(ElementType.Image, builder.Type);
        Assert.Equal(ElementType.Image, builder.Build().Type);
        Assert.Equal(Icon, builder.Source);
        Assert.Equal(Icon, builder.Build().Source);
        Assert.Equal(Name, builder.Alternative);
        Assert.Equal(Name, builder.Build().Alternative);
        Assert.Equal(ImageSize.Large, builder.Size);
        Assert.Equal(ImageSize.Large, builder.Build().Size);
        ;
        Assert.Equal(false, builder.Circle);
        Assert.Equal(false, builder.Build().Circle);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("alternative")]
    // 20 characters
    [InlineData("abcdefghijklmnopqrst")]
    public void ImageElementBuilder_ValidAlternative(string alternative)
    {
        ImageElementBuilder builder = new ImageElementBuilder().WithAlternative(alternative);
        Assert.Equal(alternative, builder.Alternative);
        Assert.Equal(alternative, builder.Build().Alternative);
    }

    [Theory]
    // 21 characters
    [InlineData("abcdefghijklmnopqrstu")]
    public void ImageElementBuilder_InvalidAlternative(string alternative) =>
        Assert.Throws<ArgumentException>(() => new ImageElementBuilder().WithAlternative(alternative));

    [Fact]
    public void ImageElementBuilder_WithSize()
    {
        foreach (ImageSize size in (ImageSize[])Enum.GetValues(typeof(ImageSize)))
        {
            ImageElementBuilder builder = new ImageElementBuilder().WithSize(size);
            Assert.Equal(size, builder.Size);
            Assert.Equal(size, builder.Build().Size);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("http://kaiheila.net")]
    [InlineData("https://kaiheila.net")]
    public void ImageElementBuilder_AcceptedUrl(string source)
    {
        ImageElementBuilder builder = new ImageElementBuilder().WithSource(source);
        Assert.Equal(source, builder.Source);
        Assert.Equal(source, builder.Build().Source);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("kaiheila.net")]
    [InlineData("steam://run/123456/")]
    public void ImageElementBuilder_InvalidUrl(string source)
    {
        ImageElementBuilder builder = new ImageElementBuilder().WithSource(source);
        Assert.Equal(source, builder.Source);
        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Theory]
    [InlineData(Icon)]
    public void ImageElementBuilder_ImplicitConversion(string source)
    {
        ImageElementBuilder builder = source;
        Assert.Equal(source, builder.Source);
        Assert.Equal(source, builder.Build().Source);
    }

    [Fact]
    public void ButtonElementBuilder_Constructor()
    {
        ButtonElementBuilder builder = new ButtonElementBuilder()
            .WithTheme(ButtonTheme.Primary)
            .WithText(Name, true)
            .WithClick(ButtonClickEventType.None);
        Assert.Equal(ElementType.Button, builder.Type);
        Assert.Equal(ElementType.Button, builder.Build().Type);
        Assert.Equal(ButtonTheme.Primary, builder.Theme);
        Assert.Equal(ButtonTheme.Primary, builder.Build().Theme);
        Assert.Equal(Name, ((KMarkdownElementBuilder)builder.Text).Content);
        Assert.Equal(Name, ((KMarkdownElement)builder.Build().Text).Content);
        Assert.Equal(ButtonClickEventType.None, builder.Click);
        Assert.Equal(ButtonClickEventType.None, builder.Build().Click);
        builder = new ButtonElementBuilder
        {
            Theme = ButtonTheme.Success,
            Text = new PlainTextElementBuilder().WithContent("text"),
            Click = ButtonClickEventType.ReturnValue,
            Value = "value"
        };
        Assert.Equal(ElementType.Button, builder.Type);
        Assert.Equal(ElementType.Button, builder.Build().Type);
        Assert.Equal(ButtonTheme.Success, builder.Theme);
        Assert.Equal(ButtonTheme.Success, builder.Build().Theme);
        Assert.Equal("text", ((PlainTextElementBuilder)builder.Text).Content);
        Assert.Equal("text", ((PlainTextElement)builder.Build().Text).Content);
        Assert.Equal(ButtonClickEventType.ReturnValue, builder.Click);
        Assert.Equal(ButtonClickEventType.ReturnValue, builder.Build().Click);
        Assert.Equal("value", builder.Value);
        Assert.Equal("value", builder.Build().Value);
    }

    [Fact]
    public void ButtonElementBuilder_ButtonTextAction()
    {
        ButtonElementBuilder builder = new ButtonElementBuilder()
            .WithText<PlainTextElementBuilder>(b => b.WithContent("text"));
        Assert.IsType<PlainTextElementBuilder>(builder.Text);
        Assert.IsType<PlainTextElement>(builder.Build().Text);
        Assert.Equal("text", ((PlainTextElementBuilder)builder.Text).Content);
        Assert.Equal("text", ((PlainTextElement)builder.Build().Text).Content);
    }

    [Fact]
    public void ButtonElementBuilder_ButtonTheme()
    {
        foreach (ButtonTheme theme in (ButtonTheme[])Enum.GetValues(typeof(ButtonTheme)))
        {
            ButtonElementBuilder builder = new ButtonElementBuilder().WithTheme(theme);
            Assert.Equal(theme, builder.Theme);
            Assert.Equal(theme, builder.Build().Theme);
        }
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("text")]
    // 40 characters
    [InlineData("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMN")]
    public void ButtonElementBuilder_ValidText(string text)
    {
        ButtonElementBuilder builder = new ButtonElementBuilder().WithText(text);
        Assert.Equal(text, ((PlainTextElementBuilder)builder.Text).Content);
        Assert.Equal(text, ((PlainTextElement)builder.Build().Text).Content);
    }

    [Theory]
    // 41 characters
    [InlineData("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNO")]
    public void ButtonElementBuilder_InvalidText(string text) => Assert.Throws<ArgumentException>(() => new ButtonElementBuilder().WithText(text));

    [Theory]
    [InlineData("http://kaiheila.net")]
    [InlineData("https://kaiheila.net")]
    public void ButtonElementBuilder_AcceptedUrl(string source)
    {
        ButtonElementBuilder builder = new ButtonElementBuilder().WithClick(ButtonClickEventType.Link).WithValue(source);
        Assert.Equal(ButtonClickEventType.Link, builder.Click);
        Assert.Equal(ButtonClickEventType.Link, builder.Build().Click);
        Assert.Equal(source, builder.Value);
        Assert.Equal(source, builder.Build().Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("kaiheila.net")]
    [InlineData("steam://run/123456/")]
    public void ButtonElementBuilder_InvalidUrl(string source)
    {
        ButtonElementBuilder builder = new ButtonElementBuilder().WithClick(ButtonClickEventType.Link).WithValue(source);
        Assert.Equal(ButtonClickEventType.Link, builder.Click);
        Assert.Equal(source, builder.Value);
        Assert.ThrowsAny<Exception>(() => builder.Build());
    }

    [Fact]
    public void ParagraphStructBuilder_Constructor()
    {
        ParagraphStructBuilder builder = new ParagraphStructBuilder()
            .WithColumnCount(1)
            .AddField(new KMarkdownElementBuilder().WithContent("text"));
        Assert.Equal(ElementType.Paragraph, builder.Type);
        Assert.Equal(ElementType.Paragraph, builder.Build().Type);
        Assert.Equal(1, builder.ColumnCount);
        Assert.Equal(1, builder.Build().ColumnCount);
        Assert.Equal("text", ((KMarkdownElementBuilder)builder.Fields[0]).Content);
        Assert.Equal("text", ((KMarkdownElement)builder.Build().Fields[0]).Content);
        builder = new ParagraphStructBuilder
        {
            ColumnCount = 3,
            Fields = new List<IElementBuilder>
            {
                new KMarkdownElementBuilder().WithContent("KMarkdown"), new PlainTextElementBuilder().WithContent("PlainText")
            }
        };
        Assert.Equal(ElementType.Paragraph, builder.Type);
        Assert.Equal(ElementType.Paragraph, builder.Build().Type);
        Assert.Equal(3, builder.ColumnCount);
        Assert.Equal(3, builder.Build().ColumnCount);
        Assert.Equal("PlainText", ((PlainTextElementBuilder)builder.Fields[1]).Content);
        Assert.Equal("PlainText", ((PlainTextElement)builder.Build().Fields[1]).Content);
    }

    [Fact]
    public void ParagraphStructBuilder_AddFieldAction()
    {
        ParagraphStructBuilder builder = new();
        builder.AddField<KMarkdownElementBuilder>(b => b.WithContent("text"));
        Assert.Equal("text", ((KMarkdownElementBuilder)builder.Fields[0]).Content);
        Assert.Equal("text", ((KMarkdownElement)builder.Build().Fields[0]).Content);
        builder = new ParagraphStructBuilder();
        builder.AddField<PlainTextElementBuilder>(b => b.WithContent("text"));
        Assert.Equal("text", ((PlainTextElementBuilder)builder.Fields[0]).Content);
        Assert.Equal("text", ((PlainTextElement)builder.Build().Fields[0]).Content);
    }

    [Fact]
    public void ParagraphStructBuilder_InvalidField()
    {
        ParagraphStructBuilder builder = new();
        Assert.Throws<ArgumentException>(() => builder.AddField<ImageElementBuilder>(b => b.WithSource(Icon)));
        Assert.Throws<ArgumentException>(() => builder.AddField<ButtonElementBuilder>(b => b.WithText("button")));
        Assert.Throws<ArgumentException>(() => builder = new ParagraphStructBuilder
        {
            Fields = new List<IElementBuilder> { new ImageElementBuilder().WithSource(Icon) }
        });
        Assert.Throws<ArgumentException>(() => builder = new ParagraphStructBuilder
        {
            Fields = new List<IElementBuilder> { new ButtonElementBuilder().WithText("button") }
        });
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(32)]
    public void ParagraphStructBuilder_InvalidColumnCount(int columnCount) =>
        Assert.Throws<ArgumentException>(() => new ParagraphStructBuilder().WithColumnCount(columnCount));
}

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Kook;

[Trait("Category", "Unit")]
public class ModuleBuilderTests
{
    private const string Name = "Kook.Net";
    private const string Icon = "https://kaiheila.net/logo.png";
    private const string Url = "https://kaiheila.net/";

    [Fact]
    public void HeaderModuleBuilder_Constructor()
    {
        var builder = new HeaderModuleBuilder()
            .WithText(new PlainTextElementBuilder().WithContent("text"));
        Assert.Equal(ModuleType.Header, builder.Type);
        Assert.Equal(ModuleType.Header, builder.Build().Type);
        Assert.Equal("text", builder.Text.Content);
        Assert.Equal("text", builder.Build().Text.Content);
        builder = new HeaderModuleBuilder
        {
            Text = new PlainTextElementBuilder().WithContent("content")
        };
        Assert.Equal(ModuleType.Header, builder.Type);
        Assert.Equal(ModuleType.Header, builder.Build().Type);
        Assert.Equal("content", builder.Text.Content);
        Assert.Equal("content", builder.Build().Text.Content);
    }

    [Fact]
    public void HeaderModuleBuilder_WithTextAction()
    {
        var builder = new HeaderModuleBuilder()
            .WithText(b => b.WithContent("text"));
        Assert.IsType<PlainTextElementBuilder>(builder.Text);
        Assert.IsType<PlainTextElement>(builder.Build().Text);
        Assert.Equal("text", builder.Text.Content);
        Assert.Equal("text", builder.Build().Text.Content);
    }

    [Fact]
    public void HeaderModuleBuilder_ValidText()
    {
        IEnumerable<string> GetValidContent()
        {
            yield return "";
            yield return " ";
            yield return "text";
            yield return new string('a', 100);
        }
        foreach (string text in GetValidContent())
        {
            var builder = new HeaderModuleBuilder()
                .WithText(new PlainTextElementBuilder().WithContent(text));
            Assert.Equal(text, builder.Text.Content);
            Assert.Equal(text, builder.Build().Text.Content);
        }
    }

    [Fact]
    public void HeaderModuleBuilder_InvalidText()
    {
        IEnumerable<string> GetInvalidContent()
        {
            yield return null;
            yield return new string('a', 101);
        }
        foreach (string text in GetInvalidContent())
        {
            Assert.Throws<ArgumentException>(() => new HeaderModuleBuilder()
                .WithText(b => b.WithContent(text)));
        }
    }

    [Theory]
    [InlineData("text")]
    public void HeaderModuleBuilder_ImplicitConversion(string text)
    {
        HeaderModuleBuilder builder = text;
        Assert.Equal(text, builder.Text.Content);
        Assert.Equal(text, builder.Build().Text.Content);
    }

    [Fact]
    public void SectionModuleBuilder_Constructor()
    {
        var builder = new SectionModuleBuilder()
            .WithMode(SectionAccessoryMode.Left)
            .WithText(new PlainTextElementBuilder().WithContent("text"))
            .WithAccessory(new ImageElementBuilder().WithSource(Icon));
        Assert.Equal(ModuleType.Section, builder.Type);
        Assert.Equal(ModuleType.Section, builder.Build().Type);
        Assert.Equal("text", ((PlainTextElementBuilder) builder.Text).Content);
        Assert.Equal("text", ((PlainTextElement) builder.Build().Text).Content);
        Assert.Equal(Icon, ((ImageElementBuilder) builder.Accessory).Source);
        Assert.Equal(Icon, ((ImageElement) builder.Build().Accessory).Source);
        builder = new SectionModuleBuilder
        {
            Mode = SectionAccessoryMode.Right,
            Text = new KMarkdownElementBuilder().WithContent("content"),
            Accessory = new ButtonElementBuilder().WithText("button", true)
        };
        Assert.Equal(ModuleType.Section, builder.Type);
        Assert.Equal(ModuleType.Section, builder.Build().Type);
        Assert.Equal("content", ((KMarkdownElementBuilder) builder.Text).Content);
        Assert.Equal("content", ((KMarkdownElement) builder.Build().Text).Content);
        Assert.Equal("button", ((KMarkdownElementBuilder) ((ButtonElementBuilder) builder.Accessory).Text).Content);
        Assert.Equal("button", ((KMarkdownElement) ((ButtonElement) builder.Build().Accessory).Text).Content);
    }

    [Fact]
    public void SectionModuleBuilder_WithTextAction()
    {
        var builder = new SectionModuleBuilder()
            .WithText<PlainTextElementBuilder>(b => b.WithContent("text"));
        Assert.IsType<PlainTextElementBuilder>(builder.Text);
        Assert.IsType<PlainTextElement>(builder.Build().Text);
        Assert.Equal("text", ((PlainTextElementBuilder) builder.Text).Content);
        Assert.Equal("text", ((PlainTextElement) builder.Build().Text).Content);
    }

    [Fact]
    public void SectionModuleBuilder_WithAccessoryAction()
    {
        var builder = new SectionModuleBuilder()
            .WithAccessory<ImageElementBuilder>(b => b.WithSource(Icon));
        Assert.IsType<ImageElementBuilder>(builder.Accessory);
        Assert.IsType<ImageElement>(builder.Build().Accessory);
        Assert.Equal(Icon, ((ImageElementBuilder) builder.Accessory).Source);
        Assert.Equal(Icon, ((ImageElement) builder.Build().Accessory).Source);
    }
    
    [Fact]
    public void SectionModuleBuilder_InvalidText()
    {
        var builder = new SectionModuleBuilder();
        Assert.Throws<ArgumentException>(() => builder.WithText<ImageElementBuilder>(b => b.WithSource(Icon)));
        Assert.Throws<ArgumentException>(() => builder.WithText<ButtonElementBuilder>(b => b.WithText("button")));
        Assert.Throws<ArgumentException>(() => builder = new SectionModuleBuilder
        {
            Text = new ImageElementBuilder().WithSource(Icon)
        });
        Assert.Throws<ArgumentException>(() => builder = new SectionModuleBuilder
        {
            Text = new ButtonElementBuilder().WithText("button")
        });
    }

    [Fact]
    public void SectionModuleBuilder_InvalidAccessory()
    {
        var builder = new SectionModuleBuilder();
        Assert.Throws<ArgumentException>(() => builder.WithAccessory<PlainTextElementBuilder>(b => b.WithContent(Name)));
        Assert.Throws<ArgumentException>(() => builder.WithAccessory<KMarkdownElementBuilder>(b => b.WithContent(Name)));
        Assert.Throws<ArgumentException>(() => builder = new SectionModuleBuilder
        {
            Accessory = new PlainTextElementBuilder().WithContent(Name)
        });
        Assert.Throws<ArgumentException>(() => builder = new SectionModuleBuilder
        {
            Accessory = new KMarkdownElementBuilder().WithContent(Name)
        });
    }

    [Fact]
    public void SectionModuleBuilder_InvalidButtonPosition()
    {
        var builder = new SectionModuleBuilder();
        builder.WithMode(SectionAccessoryMode.Left)
            .WithAccessory<ButtonElementBuilder>(b => b.WithText("button"));
        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }
    
    [Fact]
    public void ImageGroupBuilder_Constructor()
    {
        var builder = new ImageGroupModuleBuilder()
            .AddElement(b => b.WithSource(Icon));
        Assert.Equal(ModuleType.ImageGroup, builder.Type);
        Assert.Equal(ModuleType.ImageGroup, builder.Build().Type);
        Assert.Equal(Icon, builder.Elements[0].Source);
        Assert.Equal(Icon, builder.Build().Elements[0].Source);
        builder = new ImageGroupModuleBuilder
        {
            Elements = new List<ImageElementBuilder>
            {
                new ImageElementBuilder().WithSource(Icon)
            }
        };
        Assert.Equal(ModuleType.ImageGroup, builder.Type);
        Assert.Equal(ModuleType.ImageGroup, builder.Build().Type);
        Assert.Equal(Icon, builder.Elements[0].Source);
        Assert.Equal(Icon, builder.Build().Elements[0].Source);
    }

    [Fact]
    public void ImageGroupBuilder_AddElementAction()
    {
        var builder = new ImageGroupModuleBuilder()
            .AddElement(b => b.WithSource(Icon));
        Assert.IsType<ImageElementBuilder>(builder.Elements[0]);
        Assert.IsType<ImageElement>(builder.Build().Elements[0]);
        Assert.Equal(Icon, builder.Elements[0].Source);
        Assert.Equal(Icon, builder.Build().Elements[0].Source);
    }

    [Fact]
    public void ImageGroupBuilder_ValidElements()
    {
        var builder = new ImageGroupModuleBuilder();
        for (int i = 0; i < 9; i++)
            builder.AddElement(b => b.WithSource(Icon));
        Assert.Equal(9, builder.Elements.Count);
        builder = new ImageGroupModuleBuilder
        {
            Elements = Enumerable.Repeat(new ImageElementBuilder().WithSource(Icon), 9).ToList()
        };
        Assert.Equal(9, builder.Elements.Count);
    }

    [Fact]
    public void ImageGroupBuilder_InvalidElements()
    {
        var builder = new ImageGroupModuleBuilder();
        for (int i = 0; i < 9; i++)
            builder.AddElement(b => b.WithSource(Icon));
        Assert.Throws<ArgumentException>(() => builder.AddElement(b => b.WithSource(Icon)));
        Assert.Throws<ArgumentException>(() => builder = new ImageGroupModuleBuilder
        {
            Elements = Enumerable.Repeat(new ImageElementBuilder().WithSource(Icon), 10).ToList()
        });
    }
    
    [Fact]
    public void ContainerBuilder_Constructor()
    {
        var builder = new ContainerModuleBuilder()
            .AddElement(b => b.WithSource(Icon));
        Assert.Equal(ModuleType.Container, builder.Type);
        Assert.Equal(ModuleType.Container, builder.Build().Type);
        Assert.Equal(Icon, builder.Elements[0].Source);
        Assert.Equal(Icon, builder.Build().Elements[0].Source);
        builder = new ContainerModuleBuilder
        {
            Elements = new List<ImageElementBuilder>
            {
                new ImageElementBuilder().WithSource(Icon)
            }
        };
        Assert.Equal(ModuleType.Container, builder.Type);
        Assert.Equal(ModuleType.Container, builder.Build().Type);
        Assert.Equal(Icon, builder.Elements[0].Source);
        Assert.Equal(Icon, builder.Build().Elements[0].Source);
    }

    [Fact]
    public void ContainerBuilder_AddElementAction()
    {
        var builder = new ContainerModuleBuilder()
            .AddElement(b => b.WithSource(Icon));
        Assert.IsType<ImageElementBuilder>(builder.Elements[0]);
        Assert.IsType<ImageElement>(builder.Build().Elements[0]);
        Assert.Equal(Icon, builder.Elements[0].Source);
        Assert.Equal(Icon, builder.Build().Elements[0].Source);
    }

    [Fact]
    public void ContainerBuilder_ValidElements()
    {
        var builder = new ContainerModuleBuilder();
        for (int i = 0; i < 9; i++)
            builder.AddElement(b => b.WithSource(Icon));
        Assert.Equal(9, builder.Elements.Count);
        builder = new ContainerModuleBuilder
        {
            Elements = Enumerable.Repeat(new ImageElementBuilder().WithSource(Icon), 9).ToList()
        };
        Assert.Equal(9, builder.Elements.Count);
    }

    [Fact]
    public void ContainerBuilder_InvalidElements()
    {
        var builder = new ContainerModuleBuilder();
        for (int i = 0; i < 9; i++)
            builder.AddElement(b => b.WithSource(Icon));
        Assert.Throws<ArgumentException>(() => builder.AddElement(b => b.WithSource(Icon)));
        Assert.Throws<ArgumentException>(() => builder = new ContainerModuleBuilder
        {
            Elements = Enumerable.Repeat(new ImageElementBuilder().WithSource(Icon), 10).ToList()
        });
    }
    
    [Fact]
    public void ActionGroupBuilder_Constructor()
    {
        var builder = new ActionGroupModuleBuilder()
            .AddElement(b => b.WithText(Name));
        Assert.Equal(ModuleType.ActionGroup, builder.Type);
        Assert.Equal(ModuleType.ActionGroup, builder.Build().Type);
        Assert.Equal(Name, ((PlainTextElementBuilder)builder.Elements[0].Text).Content);
        Assert.Equal(Name, ((PlainTextElement)builder.Build().Elements[0].Text).Content);
        builder = new ActionGroupModuleBuilder
        {
            Elements = new List<ButtonElementBuilder>
            {
                new ButtonElementBuilder().WithText(Name, true)
            }
        };
        Assert.Equal(ModuleType.ActionGroup, builder.Type);
        Assert.Equal(ModuleType.ActionGroup, builder.Build().Type);
        Assert.Equal(Name, ((KMarkdownElementBuilder)builder.Elements[0].Text).Content);
        Assert.Equal(Name, ((KMarkdownElement)builder.Build().Elements[0].Text).Content);
    }

    [Fact]
    public void ActionGroupBuilder_AddElementAction()
    {
        var builder = new ActionGroupModuleBuilder()
            .AddElement(b => b.WithText(Name));
        Assert.IsType<ButtonElementBuilder>(builder.Elements[0]);
        Assert.IsType<ButtonElement>(builder.Build().Elements[0]);
        Assert.Equal(Name, ((PlainTextElementBuilder)builder.Elements[0].Text).Content);
        Assert.Equal(Name, ((PlainTextElement)builder.Build().Elements[0].Text).Content);
    }

    [Fact]
    public void ActionGroupBuilder_ValidElements()
    {
        var builder = new ActionGroupModuleBuilder();
        for (int i = 0; i < 4; i++)
            builder.AddElement(b => b.WithText(Name));
        Assert.Equal(4, builder.Elements.Count);
        builder = new ActionGroupModuleBuilder
        {
            Elements = Enumerable.Repeat(new ButtonElementBuilder().WithText(Name), 4).ToList()
        };
        Assert.Equal(4, builder.Elements.Count);
    }

    [Fact]
    public void ActionGroupBuilder_InvalidElements()
    {
        var builder = new ActionGroupModuleBuilder();
        for (int i = 0; i < 4; i++)
            builder.AddElement(b => b.WithText(Name));
        Assert.Throws<ArgumentException>(() => builder.AddElement(b => b.WithText(Name)));
        Assert.Throws<ArgumentException>(() => builder = new ActionGroupModuleBuilder
        {
            Elements = Enumerable.Repeat(new ButtonElementBuilder().WithText(Name), 5).ToList()
        });
    }

    [Fact]
    public void ContextBuilder_Constructor()
    {
        var builder = new ContextModuleBuilder()
            .AddElement<PlainTextElementBuilder>(b => b.WithContent(Name));
        Assert.Equal(ModuleType.Context, builder.Type);
        Assert.Equal(ModuleType.Context, builder.Build().Type);
        Assert.Equal(Name, ((PlainTextElementBuilder)builder.Elements[0]).Content);
        Assert.Equal(Name, ((PlainTextElement)builder.Build().Elements[0]).Content);
        builder = new ContextModuleBuilder
        {
            Elements = new List<IElementBuilder>
            {
                new KMarkdownElementBuilder().WithContent(Name)
            }
        };
        Assert.Equal(ModuleType.Context, builder.Type);
        Assert.Equal(ModuleType.Context, builder.Build().Type);
        Assert.Equal(Name, ((KMarkdownElementBuilder)builder.Elements[0]).Content);
        Assert.Equal(Name, ((KMarkdownElement)builder.Build().Elements[0]).Content);
    }

    [Fact]
    public void ContextBuilder_AddElementAction()
    {
        var builder = new ContextModuleBuilder()
            .AddElement<ImageElementBuilder>(b => b.WithSource(Icon));
        Assert.IsType<ImageElementBuilder>(builder.Elements[0]);
        Assert.IsType<ImageElement>(builder.Build().Elements[0]);
        Assert.Equal(Icon, ((ImageElementBuilder)builder.Elements[0]).Source);
        Assert.Equal(Icon, ((ImageElement)builder.Build().Elements[0]).Source);
    }

    [Fact]
    public void ContextBuilder_ValidElements()
    {
        var builder = new ContextModuleBuilder();
        for (int i = 0; i < 10; i++)
            builder.AddElement<PlainTextElementBuilder>(b => b.WithContent(Name));
        Assert.Equal(10, builder.Elements.Count);
        builder = new ContextModuleBuilder
        {
            Elements = Enumerable.Repeat(new PlainTextElementBuilder().WithContent(Name) as IElementBuilder, 10).ToList()
        };
        Assert.Equal(10, builder.Elements.Count);
    }

    [Fact]
    public void ContextBuilder_InvalidElements()
    {
        var builder = new ContextModuleBuilder();
        Assert.Throws<ArgumentException>(() => builder.AddElement<ParagraphStructBuilder>(b => b.WithColumnCount(2)));
        Assert.Throws<ArgumentException>(() => builder.AddElement<ButtonElementBuilder>(b => b.WithText("button")));
        for (int i = 0; i < 10; i++)
            builder.AddElement<PlainTextElementBuilder>(b => b.WithContent(Name));
        Assert.Throws<ArgumentException>(() => builder.AddElement<PlainTextElementBuilder>(b => b.WithContent(Name)));
        Assert.Throws<ArgumentException>(() => builder = new ContextModuleBuilder
        {
            Elements = Enumerable.Repeat(new ButtonElementBuilder().WithText(Name) as IElementBuilder, 11).ToList()
        });
    }
        
    [Fact]
    public void DividerModuleBuilder_Constructor()
    {
        var builder = new DividerModuleBuilder();
        Assert.Equal(ModuleType.Divider, builder.Type);
        Assert.Equal(ModuleType.Divider, builder.Build().Type);
    }

    [Fact]
    public void FileModuleBuilder_Constructor()
    {
        var builder = new FileModuleBuilder()
            .WithTitle(Name)
            .WithSource(Icon);
        Assert.Equal(ModuleType.File, builder.Type);
        Assert.Equal(ModuleType.File, builder.Build().Type);
        Assert.Equal(Name, builder.Title);
        Assert.Equal(Name, builder.Build().Title);
        Assert.Equal(Icon, builder.Source);
        Assert.Equal(Icon, builder.Build().Source);
        builder = new FileModuleBuilder
        {
            Title = Name,
            Source = Icon
        };
        Assert.Equal(ModuleType.File, builder.Type);
        Assert.Equal(ModuleType.File, builder.Build().Type);
        Assert.Equal(Name, builder.Title);
        Assert.Equal(Name, builder.Build().Title);
        Assert.Equal(Icon, builder.Source);
        Assert.Equal(Icon, builder.Build().Source);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("kaiheila.net")]
    [InlineData("steam://run/123456/")]
    public void FileModuleBuilder_InvalidUrl(string source)
    {
        var builder = new FileModuleBuilder().WithSource(source);
        Assert.Equal(source, builder.Source);
        Assert.ThrowsAny<Exception>(() => builder.Build());
    }

    [Fact]
    public void VideoModuleBuilder_Constructor()
    {
        var builder = new VideoModuleBuilder()
            .WithTitle(Name)
            .WithSource(Icon);
        Assert.Equal(ModuleType.Video, builder.Type);
        Assert.Equal(ModuleType.Video, builder.Build().Type);
        Assert.Equal(Name, builder.Title);
        Assert.Equal(Name, builder.Build().Title);
        Assert.Equal(Icon, builder.Source);
        Assert.Equal(Icon, builder.Build().Source);
        builder = new VideoModuleBuilder
        {
            Title = Name,
            Source = Icon
        };
        Assert.Equal(ModuleType.Video, builder.Type);
        Assert.Equal(ModuleType.Video, builder.Build().Type);
        Assert.Equal(Name, builder.Title);
        Assert.Equal(Name, builder.Build().Title);
        Assert.Equal(Icon, builder.Source);
        Assert.Equal(Icon, builder.Build().Source);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("kaiheila.net")]
    [InlineData("steam://run/123456/")]
    public void VideoModuleBuilder_InvalidUrl(string source)
    {
        var builder = new VideoModuleBuilder().WithSource(source);
        Assert.Equal(source, builder.Source);
        Assert.ThrowsAny<Exception>(() => builder.Build());
    }

    [Fact]
    public void AudioModuleBuilder_Constructor()
    {
        var builder = new AudioModuleBuilder()
            .WithTitle(Name)
            .WithSource(Url)
            .WithCover(Icon);
        Assert.Equal(ModuleType.Audio, builder.Type);
        Assert.Equal(ModuleType.Audio, builder.Build().Type);
        Assert.Equal(Name, builder.Title);
        Assert.Equal(Name, builder.Build().Title);
        Assert.Equal(Url, builder.Source);
        Assert.Equal(Url, builder.Build().Source);
        Assert.Equal(Icon, builder.Cover);
        Assert.Equal(Icon, builder.Build().Cover);
        builder = new AudioModuleBuilder
        {
            Title = Name,
            Source = Url,
            Cover = Icon
        };
        Assert.Equal(ModuleType.Audio, builder.Type);
        Assert.Equal(ModuleType.Audio, builder.Build().Type);
        Assert.Equal(Name, builder.Title);
        Assert.Equal(Name, builder.Build().Title);
        Assert.Equal(Url, builder.Source);
        Assert.Equal(Url, builder.Build().Source);
        Assert.Equal(Icon, builder.Cover);
        Assert.Equal(Icon, builder.Build().Cover);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("kaiheila.net")]
    [InlineData("steam://run/123456/")]
    public void AudioModuleBuilder_InvalidSource(string source)
    {
        var builder = new AudioModuleBuilder().WithSource(Url).WithCover(Icon);
        Assert.Equal(Url, builder.Source);
        Assert.Equal(Url, builder.Build().Source);
        
        builder = new AudioModuleBuilder().WithSource(source).WithCover(Icon);
        Assert.Equal(source, builder.Source);
        Assert.ThrowsAny<Exception>(() => builder.Build());
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("kaiheila.net")]
    [InlineData("steam://run/123456/")]
    public void AudioModuleBuilder_InvalidCover(string source)
    {
        var builder = new AudioModuleBuilder().WithSource(Url).WithCover(Icon);
        Assert.Equal(Icon, builder.Cover);
        Assert.Equal(Icon, builder.Build().Cover);
        
        builder = new AudioModuleBuilder().WithSource(Url).WithCover(source);
        Assert.Equal(source, builder.Cover);
        Assert.ThrowsAny<Exception>(() => builder.Build());
    }

    [Fact]
    public void CountdownModuleBuilder_Constructor()
    {
        var builder = new CountdownModuleBuilder()
            .WithMode(CountdownMode.Day)
            .WithEndTime(DateTimeOffset.Now.AddDays(1));
        Assert.Equal(ModuleType.Countdown, builder.Type);
        Assert.Equal(ModuleType.Countdown, builder.Build().Type);
        Assert.Equal(DateTimeOffset.Now.AddDays(1).DateTime, builder.EndTime.DateTime, TimeSpan.FromSeconds(1));
        Assert.Equal(DateTimeOffset.Now.AddDays(1).DateTime, builder.Build().EndTime.DateTime, TimeSpan.FromSeconds(1));
        builder = new CountdownModuleBuilder
        {
            Mode = CountdownMode.Second,
            EndTime = DateTimeOffset.Now.AddHours(1),
            StartTime = DateTimeOffset.Now
        };
        Assert.Equal(ModuleType.Countdown, builder.Type);
        Assert.Equal(ModuleType.Countdown, builder.Build().Type);
        Assert.Equal(DateTimeOffset.Now.AddHours(1).DateTime, builder.EndTime.DateTime, TimeSpan.FromSeconds(1));
        Assert.Equal(DateTimeOffset.Now.AddHours(1).DateTime, builder.Build().EndTime.DateTime, TimeSpan.FromSeconds(1));
        Assert.Equal(DateTimeOffset.Now.DateTime, builder.StartTime.Value.DateTime, TimeSpan.FromSeconds(1));
        Assert.Equal(DateTimeOffset.Now.DateTime, builder.Build().StartTime.Value.DateTime, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CountdownModuleBuilder_ValidTimes()
    {
        var builder = new CountdownModuleBuilder()
            .WithMode(CountdownMode.Day)
            .WithEndTime(DateTimeOffset.Now.AddDays(1));
        Assert.Equal(DateTimeOffset.Now.AddDays(1).DateTime, builder.EndTime.DateTime, TimeSpan.FromSeconds(1));
        Assert.Equal(DateTimeOffset.Now.AddDays(1).DateTime, builder.Build().EndTime.DateTime, TimeSpan.FromSeconds(1));
        builder = new CountdownModuleBuilder()
            .WithMode(CountdownMode.Hour)
            .WithEndTime(DateTimeOffset.Now.AddHours(1));
        Assert.Equal(DateTimeOffset.Now.AddHours(1).DateTime, builder.EndTime.DateTime, TimeSpan.FromSeconds(1));
        Assert.Equal(DateTimeOffset.Now.AddHours(1).DateTime, builder.Build().EndTime.DateTime, TimeSpan.FromSeconds(1));
        builder = new CountdownModuleBuilder()
            .WithMode(CountdownMode.Second)
            .WithEndTime(DateTimeOffset.Now.AddMinutes(1))
            .WithStartTime(DateTimeOffset.Now.AddMinutes(-1));
        Assert.Equal(DateTimeOffset.Now.AddMinutes(1).DateTime, builder.EndTime.DateTime, TimeSpan.FromSeconds(1));
        Assert.Equal(DateTimeOffset.Now.AddMinutes(1).DateTime, builder.Build().EndTime.DateTime, TimeSpan.FromSeconds(1));
        Assert.Equal(DateTimeOffset.Now.AddMinutes(-1).DateTime, builder.StartTime.Value.DateTime, TimeSpan.FromSeconds(1));
        Assert.Equal(DateTimeOffset.Now.AddMinutes(-1).DateTime, builder.Build().StartTime.Value.DateTime, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CountdownModuleBuilder_InvalidTimes()
    {
        IEnumerable<(CountdownMode countdownMode, DateTimeOffset endTime, DateTimeOffset? startTime)> GetInvalidTimes()
        {
            // StartTime is not allowed for CountdownMode.Day and CountdownMode.Hour
            yield return (CountdownMode.Day, DateTimeOffset.Now.AddMinutes(1), DateTimeOffset.Now.AddMinutes(-1));
            yield return (CountdownMode.Hour, DateTimeOffset.Now.AddMinutes(1), DateTimeOffset.Now.AddMinutes(-1));
            // EndTime must be later than current time
            yield return (CountdownMode.Day, DateTimeOffset.Now.AddDays(-1), null);
            yield return (CountdownMode.Hour, DateTimeOffset.Now.AddDays(-1), null);
            // StartTime must be equal or later than Unix Epoch
            yield return (CountdownMode.Second, DateTimeOffset.Now.AddMinutes(1), DateTimeOffset.UnixEpoch.AddTicks(-1));
            // EndTime must be later than StartTime
            yield return (CountdownMode.Second, DateTimeOffset.Now.AddMinutes(1), DateTimeOffset.Now.AddMinutes(1));
            yield return (CountdownMode.Second, DateTimeOffset.Now.AddMinutes(1), DateTimeOffset.Now.AddMinutes(2));
        }

        foreach ((CountdownMode countdownMode, DateTimeOffset endTime, DateTimeOffset? startTime) in GetInvalidTimes())
        {
            var builder = new CountdownModuleBuilder()
                .WithMode(countdownMode)
                .WithEndTime(endTime);
            if (startTime.HasValue)
                builder.WithStartTime(startTime.Value);
            Assert.ThrowsAny<Exception>(() => builder.Build());
        }
    }
    
    [Fact]
    public void InviteModuleBuilder_Constructor()
    {
        var builder = new InviteModuleBuilder()
            .WithCode("abcdef");
        Assert.Equal(ModuleType.Invite, builder.Type);
        Assert.Equal(ModuleType.Invite, builder.Build().Type);
        Assert.Equal("abcdef", builder.Code);
        Assert.Equal("abcdef", builder.Build().Code);
        builder = new InviteModuleBuilder
        {
            Code = "ghijkl"
        };
        Assert.Equal(ModuleType.Invite, builder.Type);
        Assert.Equal(ModuleType.Invite, builder.Build().Type);
        Assert.Equal("ghijkl", builder.Code);
        Assert.Equal("ghijkl", builder.Build().Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123456")]
    [InlineData("text")]
    [InlineData("hUsYbZ")]
    [InlineData("https://kook.top/hUsYbZ")]
    public void InviteModuleBuilder_ImplicitConversion(string text)
    {
        InviteModuleBuilder builder = text;
        Assert.Equal(text, builder.Code);
        Assert.Equal(text, builder.Build().Code);
    }
}
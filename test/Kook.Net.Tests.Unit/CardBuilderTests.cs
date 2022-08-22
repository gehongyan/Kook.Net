using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kook;

/// <summary>
///     Tests the <see cref="Kook.CardBuilder"/> class.
/// </summary>
[Trait("Category", "Unit")]
public class CardBuilderTests
{
    private const string Name = "Kook.Net";
    private const string Icon = "https://kaiheila.net/logo.png";
    private const string Url = "https://kaiheila.net/";

    [Fact]
    public void Constructor()
    {
        var builder = new CardBuilder()
            .WithTheme(CardTheme.Info)
            .WithSize(CardSize.Small)
            .WithColor(Color.Blue)
            .AddModule<SectionModuleBuilder>(b =>
                b.WithText<KMarkdownElementBuilder>(t => 
                    t.WithContent("text")));
        Assert.Equal(CardTheme.Info, builder.Theme);
        Assert.Equal(CardTheme.Info, builder.Build().Theme);
        Assert.Equal(CardSize.Small, builder.Size);
        Assert.Equal(CardSize.Small, builder.Build().Size);
        Assert.Equal(Color.Blue, builder.Color);
        Assert.Equal(Color.Blue, builder.Build().Color);
        Assert.Equal("text", ((KMarkdownElementBuilder) ((SectionModuleBuilder) builder.Modules[0]).Text).Content);
        Assert.Equal("text", ((KMarkdownElement) ((SectionModule) builder.Build().Modules[0]).Text).Content);
        builder = new CardBuilder
        {
            Theme = CardTheme.Danger,
            Size = CardSize.Large,
            Color = Color.Red,
            Modules = new List<IModuleBuilder>
            {
                new SectionModuleBuilder
                {
                    Text = new PlainTextElementBuilder
                    {
                        Content = "content"
                    }
                }
            }
        };
        Assert.Equal(CardTheme.Danger, builder.Theme);
        Assert.Equal(CardTheme.Danger, builder.Build().Theme);
        Assert.Equal(CardSize.Large, builder.Size);
        Assert.Equal(CardSize.Large, builder.Build().Size);
        Assert.Equal(Color.Red, builder.Color);
        Assert.Equal(Color.Red, builder.Build().Color);
        Assert.Equal("content", ((PlainTextElementBuilder) ((SectionModuleBuilder) builder.Modules[0]).Text).Content);
        Assert.Equal("content", ((PlainTextElement) ((SectionModule) builder.Build().Modules[0]).Text).Content);
    }
    
    /// <summary>
    ///     Tests the behavior of <see cref="CardBuilder.WithTheme(CardTheme)"/>.
    /// </summary>
    [Fact]
    public void WithTheme()
    {
        foreach (CardTheme theme in Enum.GetValues<CardTheme>())
        {
            var builder = new CardBuilder().WithTheme(theme);
            Assert.Equal(theme, builder.Theme);
            Assert.Equal(theme, builder.Build().Theme);
        }
    }

    /// <summary>
    ///     Tests the behavior of <see cref="CardBuilder.WithColor(Color)"/>.
    /// </summary>
    [Fact]
    public void WithColor()
    {
        var builder = new CardBuilder().WithColor(Color.Red);
        Assert.Equal(Color.Red.RawValue, builder.Color.Value.RawValue);
        Assert.Equal(Color.Red.RawValue, builder.Build().Color.Value.RawValue);
    }

    /// <summary>
    ///     Tests the behavior of <see cref="CardBuilder.WithSize(CardSize)"/>.
    /// </summary>
    [Fact]
    public void WithSize()
    {
        foreach (CardSize size in Enum.GetValues<CardSize>())
        {
            var builder = new CardBuilder().WithSize(size);
            Assert.Equal(size, builder.Size);
            Assert.Equal(size, builder.Build().Size);
        }
    }

    /// <summary>
    ///     Tests the behavior of <see cref="CardBuilder.AddModule(IModuleBuilder)"/>.
    /// </summary>
    [Fact]
    public void AddModuleAction()
    {
        var builder = new CardBuilder()
            .WithTheme(CardTheme.Secondary)
            .WithSize(CardSize.Small)
            .WithColor(Color.Magenta)
            .AddModule<HeaderModuleBuilder>(b => b
                .WithText(text => text
                    .WithContent("Header")
                    .WithEmoji(false)))
            .AddModule<SectionModuleBuilder>(b => b
                .WithText<KMarkdownElementBuilder>(text => text
                    .WithContent("Section")))
            .AddModule<ImageGroupModuleBuilder>(b => b
                .AddElement(image => image
                    .WithSource(Icon)))
            .AddModule<ContainerModuleBuilder>(b => b
                .AddElement(element => element
                    .WithSource(Icon)))
            .AddModule<ActionGroupModuleBuilder>(b => b.
                AddElement(button => button
                    .WithTheme(ButtonTheme.Info)
                    .WithText("Button")))
            .AddModule<ContextModuleBuilder>(b => b
                .AddElement<ImageElementBuilder>(element => element
                    .WithSource(Icon))
                .AddElement<PlainTextElementBuilder>(element => element
                    .WithContent(Icon)))
            .AddModule<DividerModuleBuilder>()
            .AddModule<FileModuleBuilder>(b => b
                .WithTitle(Name)
                .WithSource(Icon))
            .AddModule<VideoModuleBuilder>(b => b
                .WithTitle(Name)
                .WithSource(Icon))
            .AddModule<AudioModuleBuilder>(b => b
                .WithTitle(Name)
                .WithSource(Url)
                .WithCover(Icon))
            .AddModule<CountdownModuleBuilder>(b => b
                .WithMode(CountdownMode.Second)
                .WithEndTime(DateTimeOffset.Now.AddMinutes(1).AddHours(1))
                .WithStartTime(DateTimeOffset.Now.AddMinutes(1)))
            .AddModule<InviteModuleBuilder>(b => b
                .WithCode("sIISdv"));
        Card card = builder.Build();
        Assert.Equal(CardTheme.Secondary, builder.Theme);
        Assert.Equal(CardTheme.Secondary, card.Theme);
        Assert.Equal(CardSize.Small, builder.Size);
        Assert.Equal(CardSize.Small, card.Size);
        Assert.Equal(Color.Magenta, builder.Color);
        Assert.Equal(Color.Magenta, card.Color);
        Assert.Equal(builder.Modules.Count, card.Modules.Length);
        for (int index = 0; index < builder.Modules.Count; index++)
        {
            IModuleBuilder moduleBuilder = builder.Modules[index];
            Assert.Equal(moduleBuilder.Type, card.Modules[index].Type);
            Assert.Equal(moduleBuilder.Build().GetType(), card.Modules[index].GetType());
        }
    }

}
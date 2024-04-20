using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Provides extension methods for <see cref="IElement"/>, <see cref="IModule"/> and <see cref="ICard"/>.
/// </summary>
public static class CardExtensions
{
    #region Elements

    /// <summary>
    ///     Converts the <see cref="IElement"/> to a <see cref="IElementBuilder"/> with the same properties.
    /// </summary>
    public static IElementBuilder ToBuilder(this IElement entity)
    {
        return entity switch
        {
            PlainTextElement { Type: ElementType.PlainText } plainTextElement => plainTextElement.ToBuilder(),
            KMarkdownElement { Type: ElementType.KMarkdown } kMarkdownElement => kMarkdownElement.ToBuilder(),
            ImageElement { Type: ElementType.Image } imageElement => imageElement.ToBuilder(),
            ButtonElement { Type: ElementType.Button } buttonElement => buttonElement.ToBuilder(),
            ParagraphStruct { Type: ElementType.Paragraph } paragraphStruct => paragraphStruct.ToBuilder(),
            _ => throw new ArgumentOutOfRangeException(nameof(entity))
        };
    }

    /// <summary>
    ///     Converts the <see cref="PlainTextElement"/> to a <see cref="PlainTextElementBuilder"/> with the same properties.
    /// </summary>
    public static PlainTextElementBuilder ToBuilder(this PlainTextElement entity)
    {
        return new PlainTextElementBuilder
        {
            Content = entity.Content,
            Emoji = entity.Emoji
        };
    }

    /// <summary>
    ///     Converts the <see cref="KMarkdownElement"/> to a <see cref="KMarkdownElementBuilder"/> with the same properties.
    /// </summary>
    public static KMarkdownElementBuilder ToBuilder(this KMarkdownElement entity)
    {
        return new KMarkdownElementBuilder { Content = entity.Content };
    }

    /// <summary>
    ///     Converts the <see cref="ImageElement"/> to an <see cref="ImageElementBuilder"/> with the same properties.
    /// </summary>
    public static ImageElementBuilder ToBuilder(this ImageElement entity)
    {
        return new ImageElementBuilder
        {
            Source = entity.Source,
            Alternative = entity.Alternative,
            Size = entity.Size,
            Circle = entity.Circle
        };
    }

    /// <summary>
    ///     Converts the <see cref="ButtonElement"/> to a <see cref="ButtonElementBuilder"/> with the same properties.
    /// </summary>
    public static ButtonElementBuilder ToBuilder(this ButtonElement entity)
    {
        return new ButtonElementBuilder
        {
            Theme = entity.Theme,
            Click = entity.Click,
            Value = entity.Value,
            Text = entity.Text.ToBuilder()
        };
    }

    /// <summary>
    ///     Converts the <see cref="ParagraphStruct"/> to a <see cref="ParagraphStructBuilder"/> with the same properties.
    /// </summary>
    public static ParagraphStructBuilder ToBuilder(this ParagraphStruct entity)
    {
        return new ParagraphStructBuilder
        {
            ColumnCount = entity.ColumnCount,
            Fields = entity.Fields.Select(x => x.ToBuilder()).ToList()
        };
    }

    #endregion

    #region Modules

    /// <summary>
    ///     Converts the <see cref="IModule"/> to a <see cref="IModuleBuilder"/> with the same properties.
    /// </summary>
    public static IModuleBuilder ToBuilder(this IModule entity)
    {
        return entity switch
        {
            HeaderModule { Type: ModuleType.Header } headerModule => headerModule.ToBuilder(),
            SectionModule { Type: ModuleType.Section } sectionModule => sectionModule.ToBuilder(),
            ImageGroupModule { Type: ModuleType.ImageGroup } imageGroupModule => imageGroupModule.ToBuilder(),
            ContainerModule { Type: ModuleType.Container } containerModule => containerModule.ToBuilder(),
            ActionGroupModule { Type: ModuleType.ActionGroup } actionGroupModule => actionGroupModule.ToBuilder(),
            ContextModule { Type: ModuleType.Context } contextModule => contextModule.ToBuilder(),
            DividerModule { Type: ModuleType.Divider } dividerModule => dividerModule.ToBuilder(),
            FileModule { Type: ModuleType.File } fileModule => fileModule.ToBuilder(),
            AudioModule { Type: ModuleType.Audio } audioModule => audioModule.ToBuilder(),
            VideoModule { Type: ModuleType.Video } videoModule => videoModule.ToBuilder(),
            CountdownModule { Type: ModuleType.Countdown } countdownModule => countdownModule.ToBuilder(),
            InviteModule { Type: ModuleType.Invite } inviteModule => inviteModule.ToBuilder(),
            _ => throw new ArgumentOutOfRangeException(nameof(entity))
        };
    }

    /// <summary>
    ///     Converts the <see cref="HeaderModule"/> to a <see cref="HeaderModuleBuilder"/> with the same properties.
    /// </summary>
    public static HeaderModuleBuilder ToBuilder(this HeaderModule entity)
    {
        return new HeaderModuleBuilder
        {
            Text = entity.Text.ToBuilder()
        };
    }

    /// <summary>
    ///     Converts the <see cref="SectionModule"/> to a <see cref="SectionModuleBuilder"/> with the same properties.
    /// </summary>
    public static SectionModuleBuilder ToBuilder(this SectionModule entity)
    {
        return new SectionModuleBuilder
        {
            Mode = entity.Mode,
            Text = entity.Text?.ToBuilder(),
            Accessory = entity.Accessory?.ToBuilder()
        };
    }

    /// <summary>
    ///     Converts the <see cref="ImageGroupModule"/> to a <see cref="ImageGroupModuleBuilder"/> with the same properties.
    /// </summary>
    public static ImageGroupModuleBuilder ToBuilder(this ImageGroupModule entity)
    {
        return new ImageGroupModuleBuilder
        {
            Elements = entity.Elements.Select(x => x.ToBuilder()).ToList()
        };
    }

    /// <summary>
    ///     Converts the <see cref="ContainerModule"/> to a <see cref="ContainerModuleBuilder"/> with the same properties.
    /// </summary>
    public static ContainerModuleBuilder ToBuilder(this ContainerModule entity)
    {
        return new ContainerModuleBuilder
        {
            Elements = entity.Elements.Select(x => x.ToBuilder()).ToList()
        };
    }

    /// <summary>
    ///     Converts the <see cref="ActionGroupModule"/> to a <see cref="ActionGroupModuleBuilder"/> with the same properties.
    /// </summary>
    public static ActionGroupModuleBuilder ToBuilder(this ActionGroupModule entity)
    {
        return new ActionGroupModuleBuilder
        {
            Elements = entity.Elements.Select(x => x.ToBuilder()).ToList()
        };
    }

    /// <summary>
    ///     Converts the <see cref="ContextModule"/> to a <see cref="ContextModuleBuilder"/> with the same properties.
    /// </summary>
    public static ContextModuleBuilder ToBuilder(this ContextModule entity)
    {
        return new ContextModuleBuilder
        {
            Elements = entity.Elements.Select(e => e.ToBuilder()).ToList()
        };
    }

    /// <summary>
    ///     Converts the <see cref="DividerModule"/> to a <see cref="DividerModuleBuilder"/> with the same properties.
    /// </summary>
    public static DividerModuleBuilder ToBuilder(this DividerModule _)
    {
        return new DividerModuleBuilder();
    }

    /// <summary>
    ///     Converts the <see cref="FileModule"/> to a <see cref="FileModuleBuilder"/> with the same properties.
    /// </summary>
    public static FileModuleBuilder ToBuilder(this FileModule entity)
    {
        return new FileModuleBuilder
        {
            Source = entity.Source,
            Title = entity.Title
        };
    }

    /// <summary>
    ///     Converts the <see cref="AudioModule"/> to an <see cref="AudioModuleBuilder"/> with the same properties.
    /// </summary>
    public static AudioModuleBuilder ToBuilder(this AudioModule entity)
    {
        return new AudioModuleBuilder
        {
            Source = entity.Source,
            Title = entity.Title,
            Cover = entity.Cover
        };
    }

    /// <summary>
    ///     Converts the <see cref="VideoModule"/> to a <see cref="VideoModuleBuilder"/> with the same properties.
    /// </summary>
    public static VideoModuleBuilder ToBuilder(this VideoModule entity)
    {
        return new VideoModuleBuilder
        {
            Source = entity.Source,
            Title = entity.Title
        };
    }

    /// <summary>
    ///     Converts the <see cref="CountdownModule"/> to a <see cref="CountdownModuleBuilder"/> with the same properties.
    /// </summary>
    public static CountdownModuleBuilder ToBuilder(this CountdownModule entity)
    {
        return new CountdownModuleBuilder
        {
            Mode = entity.Mode,
            EndTime = entity.EndTime,
            StartTime = entity.StartTime
        };
    }

    /// <summary>
    ///     Converts the <see cref="InviteModule"/> to an <see cref="InviteModuleBuilder"/> with the same properties.
    /// </summary>
    public static InviteModuleBuilder ToBuilder(this InviteModule entity)
    {
        return new InviteModuleBuilder
        {
            Code = entity.Code
        };
    }

    #endregion

    #region Cards

    /// <summary>
    ///     Converts the <see cref="ICard"/> to an <see cref="ICardBuilder"/> with the same properties.
    /// </summary>
    public static ICardBuilder ToBuilder(this ICard entity)
    {
        return entity switch
        {
            Card { Type: CardType.Card } card => card.ToBuilder(),
            _ => throw new ArgumentOutOfRangeException(nameof(entity))
        };
    }

    /// <summary>
    ///     Converts the <see cref="Card"/> to a <see cref="CardBuilder"/> with the same properties.
    /// </summary>
    public static CardBuilder ToBuilder(this Card entity)
    {
        return new CardBuilder
        {
            Theme = entity.Theme,
            Size = entity.Size,
            Color = entity.Color,
            Modules = entity.Modules.Select(m => m.ToBuilder()).ToList()
        };
    }

    #endregion
}

namespace Kook;

/// <summary>
///     提供用于 <see cref="IElement"/>、<see cref="IModule"/> 和 <see cref="ICard"/> 等卡片相关对象的扩展方法。
/// </summary>
public static class CardExtensions
{
    #region Elements

    /// <summary>
    ///     将 <see cref="IElement"/> 实体转换为具有相同属性的 <see cref="IElementBuilder"/> 实体构建器。
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
    ///     将 <see cref="PlainTextElement"/> 实体转换为具有相同属性的 <see cref="PlainTextElementBuilder"/> 实体构建器。
    /// </summary>
    public static PlainTextElementBuilder ToBuilder(this PlainTextElement entity)
    {
        return new PlainTextElementBuilder
        {
            Content = entity.Content,
            Emoji = entity.Emoji ?? true
        };
    }

    /// <summary>
    ///     将 <see cref="KMarkdownElement"/> 实体转换为具有相同属性的 <see cref="KMarkdownElementBuilder"/> 实体构建器。
    /// </summary>
    public static KMarkdownElementBuilder ToBuilder(this KMarkdownElement entity)
    {
        return new KMarkdownElementBuilder { Content = entity.Content };
    }

    /// <summary>
    ///     将 <see cref="ImageElement"/> 实体转换为具有相同属性的 <see cref="ImageElementBuilder"/> 实体构建器。
    /// </summary>
    public static ImageElementBuilder ToBuilder(this ImageElement entity)
    {
        return new ImageElementBuilder
        {
            Source = entity.Source,
            Alternative = entity.Alternative,
            Size = entity.Size,
            Circle = entity.Circle,
            FallbackUrl = entity.FallbackUrl
        };
    }

    /// <summary>
    ///     将 <see cref="ButtonElement"/> 实体转换为具有相同属性的 <see cref="ButtonElementBuilder"/> 实体构建器。
    /// </summary>
    public static ButtonElementBuilder ToBuilder(this ButtonElement entity)
    {
        return new ButtonElementBuilder
        {
            Theme = entity.Theme ?? ButtonTheme.Primary,
            Click = entity.Click ?? ButtonClickEventType.None,
            Value = entity.Value,
            Text = entity.Text.ToBuilder()
        };
    }

    /// <summary>
    ///     将 <see cref="ParagraphStruct"/> 实体转换为具有相同属性的 <see cref="ParagraphStructBuilder"/> 实体构建器。
    /// </summary>
    public static ParagraphStructBuilder ToBuilder(this ParagraphStruct entity)
    {
        return new ParagraphStructBuilder
        {
            ColumnCount = entity.ColumnCount ?? 1,
            Fields = entity.Fields.Select(x => x.ToBuilder()).ToList()
        };
    }

    #endregion

    #region Modules

    /// <summary>
    ///     将 <see cref="IModule"/> 实体转换为具有相同属性的 <see cref="IModuleBuilder"/> 实体构建器。
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
    ///     将 <see cref="HeaderModule"/> 实体转换为具有相同属性的 <see cref="HeaderModuleBuilder"/> 实体构建器。
    /// </summary>
    public static HeaderModuleBuilder ToBuilder(this HeaderModule entity)
    {
        return new HeaderModuleBuilder
        {
            Text = entity.Text?.ToBuilder()
        };
    }

    /// <summary>
    ///     将 <see cref="SectionModule"/> 实体转换为具有相同属性的 <see cref="SectionModuleBuilder"/> 实体构建器。
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
    ///     将 <see cref="ImageGroupModule"/> 实体转换为具有相同属性的 <see cref="ImageGroupModuleBuilder"/> 实体构建器。
    /// </summary>
    public static ImageGroupModuleBuilder ToBuilder(this ImageGroupModule entity)
    {
        return new ImageGroupModuleBuilder
        {
            Elements = entity.Elements.Select(x => x.ToBuilder()).ToList()
        };
    }

    /// <summary>
    ///     将 <see cref="ContainerModule"/> 实体转换为具有相同属性的 <see cref="ContainerModuleBuilder"/> 实体构建器。
    /// </summary>
    public static ContainerModuleBuilder ToBuilder(this ContainerModule entity)
    {
        return new ContainerModuleBuilder
        {
            Elements = entity.Elements.Select(x => x.ToBuilder()).ToList()
        };
    }

    /// <summary>
    ///     将 <see cref="ActionGroupModule"/> 实体转换为具有相同属性的 <see cref="ActionGroupModuleBuilder"/> 实体构建器。
    /// </summary>
    public static ActionGroupModuleBuilder ToBuilder(this ActionGroupModule entity)
    {
        return new ActionGroupModuleBuilder
        {
            Elements = entity.Elements.Select(x => x.ToBuilder()).ToList()
        };
    }

    /// <summary>
    ///     将 <see cref="ContextModule"/> 实体转换为具有相同属性的 <see cref="ContextModuleBuilder"/> 实体构建器。
    /// </summary>
    public static ContextModuleBuilder ToBuilder(this ContextModule entity)
    {
        return new ContextModuleBuilder
        {
            Elements = entity.Elements.Select(e => e.ToBuilder()).ToList()
        };
    }

    /// <summary>
    ///     将 <see cref="DividerModule"/> 实体转换为具有相同属性的 <see cref="DividerModuleBuilder"/> 实体构建器。
    /// </summary>
    public static DividerModuleBuilder ToBuilder(this DividerModule _)
    {
        return new DividerModuleBuilder();
    }

    /// <summary>
    ///     将 <see cref="FileModule"/> 实体转换为具有相同属性的 <see cref="FileModuleBuilder"/> 实体构建器。
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
    ///     将 <see cref="AudioModule"/> 实体转换为具有相同属性的 <see cref="AudioModuleBuilder"/> 实体构建器。
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
    ///     将 <see cref="VideoModule"/> 实体转换为具有相同属性的 <see cref="VideoModuleBuilder"/> 实体构建器。
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
    ///     将 <see cref="CountdownModule"/> 实体转换为具有相同属性的 <see cref="CountdownModuleBuilder"/> 实体构建器。
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
    ///     将 <see cref="InviteModule"/> 实体转换为具有相同属性的 <see cref="InviteModuleBuilder"/> 实体构建器。
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
    ///     将 <see cref="ICard"/> 实体转换为具有相同属性的 <see cref="ICardBuilder"/> 实体构建器。
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
    ///     将 <see cref="Card"/> 实体转换为具有相同属性的 <see cref="CardBuilder"/> 实体构建器。
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

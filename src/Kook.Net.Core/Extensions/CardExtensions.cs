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
    public static IElementBuilder ToBuilder(this IElement element)
    {
        if (element is null) return null;
        return element.Type switch
        {
            ElementType.PlainText => (element as PlainTextElement).ToBuilder(),
            ElementType.KMarkdown => (element as KMarkdownElement).ToBuilder(),
            ElementType.Image => (element as ImageElement).ToBuilder(),
            ElementType.Button => (element as ButtonElement).ToBuilder(),
            ElementType.Paragraph => (element as ParagraphStruct).ToBuilder(),
            _ => throw new ArgumentOutOfRangeException(nameof(element))
        };
    }

    /// <summary>
    ///     Converts the <see cref="PlainTextElement"/> to a <see cref="PlainTextElementBuilder"/> with the same properties.
    /// </summary>
    public static PlainTextElementBuilder ToBuilder(this PlainTextElement entity)
    {
        if (entity is null) return null;
        return new PlainTextElementBuilder { Content = entity.Content, Emoji = entity.Emoji };
    }

    /// <summary>
    ///     Converts the <see cref="KMarkdownElement"/> to a <see cref="KMarkdownElementBuilder"/> with the same properties.
    /// </summary>
    public static KMarkdownElementBuilder ToBuilder(this KMarkdownElement entity)
    {
        if (entity is null) return null;
        return new KMarkdownElementBuilder { Content = entity.Content };
    }

    /// <summary>
    ///     Converts the <see cref="ImageElement"/> to a <see cref="ImageElementBuilder"/> with the same properties.
    /// </summary>
    public static ImageElementBuilder ToBuilder(this ImageElement entity)
    {
        if (entity is null) return null;
        return new ImageElementBuilder
        {
            Source = entity.Source, Alternative = entity.Alternative, Size = entity.Size, Circle = entity.Circle
        };
    }

    /// <summary>
    ///     Converts the <see cref="ButtonElement"/> to a <see cref="ButtonElementBuilder"/> with the same properties.
    /// </summary>
    public static ButtonElementBuilder ToBuilder(this ButtonElement entity)
    {
        if (entity is null) return null;
        return new ButtonElementBuilder
        {
            Theme = entity.Theme, Click = entity.Click, Value = entity.Value, Text = entity.Text.ToBuilder()
        };
    }

    /// <summary>
    ///     Converts the <see cref="ParagraphStruct"/> to a <see cref="ParagraphStructBuilder"/> with the same properties.
    /// </summary>
    public static ParagraphStructBuilder ToBuilder(this ParagraphStruct entity)
    {
        if (entity is null) return null;
        return new ParagraphStructBuilder
        {
            ColumnCount = entity.ColumnCount, Fields = entity.Fields.Select(x => x.ToBuilder()).ToList()
        };
    }

    #endregion

    #region Modules

    /// <summary>
    ///     Converts the <see cref="IModule"/> to a <see cref="IModuleBuilder"/> with the same properties.
    /// </summary>
    public static IModuleBuilder ToBuilder(this IModule entity)
    {
        if (entity is null) return null;
        return entity.Type switch
        {
            ModuleType.Header => (entity as HeaderModule).ToBuilder(),
            ModuleType.Section => (entity as SectionModule).ToBuilder(),
            ModuleType.ImageGroup => (entity as ImageGroupModule).ToBuilder(),
            ModuleType.Container => (entity as ContainerModule).ToBuilder(),
            ModuleType.ActionGroup => (entity as ActionGroupModule).ToBuilder(),
            ModuleType.Context => (entity as ContextModule).ToBuilder(),
            ModuleType.Divider => (entity as DividerModule).ToBuilder(),
            ModuleType.File => (entity as FileModule).ToBuilder(),
            ModuleType.Audio => (entity as AudioModule).ToBuilder(),
            ModuleType.Video => (entity as VideoModule).ToBuilder(),
            ModuleType.Countdown => (entity as CountdownModule).ToBuilder(),
            ModuleType.Invite => (entity as InviteModule).ToBuilder(),
            _ => throw new ArgumentOutOfRangeException(nameof(entity))
        };
    }

    /// <summary>
    ///     Converts the <see cref="HeaderModule"/> to a <see cref="HeaderModuleBuilder"/> with the same properties.
    /// </summary>
    public static HeaderModuleBuilder ToBuilder(this HeaderModule entity)
    {
        if (entity is null) return null;
        return new HeaderModuleBuilder { Text = entity.Text.ToBuilder() };
    }

    /// <summary>
    ///     Converts the <see cref="SectionModule"/> to a <see cref="SectionModuleBuilder"/> with the same properties.
    /// </summary>
    public static SectionModuleBuilder ToBuilder(this SectionModule entity)
    {
        if (entity is null) return null;
        return new SectionModuleBuilder
        {
            Mode = entity.Mode,
            Text = entity.Text.ToBuilder(),
            Accessory = entity.Accessory.ToBuilder()
        };
    }

    /// <summary>
    ///     Converts the <see cref="ImageGroupModule"/> to a <see cref="ImageGroupModuleBuilder"/> with the same properties.
    /// </summary>
    public static ImageGroupModuleBuilder ToBuilder(this ImageGroupModule entity)
    {
        if (entity is null) return null;
        return new ImageGroupModuleBuilder { Elements = entity.Elements.Select(x => x.ToBuilder()).ToList() };
    }

    /// <summary>
    ///     Converts the <see cref="ContainerModule"/> to a <see cref="ContainerModuleBuilder"/> with the same properties.
    /// </summary>
    public static ContainerModuleBuilder ToBuilder(this ContainerModule entity)
    {
        if (entity is null) return null;
        return new ContainerModuleBuilder { Elements = entity.Elements.Select(x => x.ToBuilder()).ToList() };
    }

    /// <summary>
    ///     Converts the <see cref="ActionGroupModule"/> to a <see cref="ActionGroupModuleBuilder"/> with the same properties.
    /// </summary>
    public static ActionGroupModuleBuilder ToBuilder(this ActionGroupModule entity)
    {
        if (entity is null) return null;
        return new ActionGroupModuleBuilder { Elements = entity.Elements.Select(x => x.ToBuilder()).ToList() };
    }

    /// <summary>
    ///     Converts the <see cref="ContextModule"/> to a <see cref="ContextModuleBuilder"/> with the same properties.
    /// </summary>
    public static ContextModuleBuilder ToBuilder(this ContextModule entity)
    {
        if (entity is null) return null;
        return new ContextModuleBuilder { Elements = entity.Elements.Select(e => e.ToBuilder()).ToList() };
    }

    /// <summary>
    ///     Converts the <see cref="DividerModule"/> to a <see cref="DividerModuleBuilder"/> with the same properties.
    /// </summary>
    public static DividerModuleBuilder ToBuilder(this DividerModule entity)
    {
        if (entity is null) return null;
        return new DividerModuleBuilder();
    }

    /// <summary>
    ///     Converts the <see cref="FileModule"/> to a <see cref="FileModuleBuilder"/> with the same properties.
    /// </summary>
    public static FileModuleBuilder ToBuilder(this FileModule entity)
    {
        if (entity is null) return null;
        return new FileModuleBuilder { Source = entity.Source, Title = entity.Title };
    }

    /// <summary>
    ///     Converts the <see cref="AudioModule"/> to a <see cref="AudioModuleBuilder"/> with the same properties.
    /// </summary>
    public static AudioModuleBuilder ToBuilder(this AudioModule entity)
    {
        if (entity is null) return null;
        return new AudioModuleBuilder { Source = entity.Source, Title = entity.Title, Cover = entity.Cover };
    }

    /// <summary>
    ///     Converts the <see cref="VideoModule"/> to a <see cref="VideoModuleBuilder"/> with the same properties.
    /// </summary>
    public static VideoModuleBuilder ToBuilder(this VideoModule entity)
    {
        if (entity is null) return null;
        return new VideoModuleBuilder { Source = entity.Source, Title = entity.Title };
    }

    /// <summary>
    ///     Converts the <see cref="CountdownModule"/> to a <see cref="CountdownModuleBuilder"/> with the same properties.
    /// </summary>
    public static CountdownModuleBuilder ToBuilder(this CountdownModule entity)
    {
        if (entity is null) return null;
        return new CountdownModuleBuilder
        {
            Mode = entity.Mode, EndTime = entity.EndTime, StartTime = entity.StartTime
        };
    }

    /// <summary>
    ///     Converts the <see cref="InviteModule"/> to a <see cref="InviteModuleBuilder"/> with the same properties.
    /// </summary>
    public static InviteModuleBuilder ToBuilder(this InviteModule entity)
    {
        if (entity is null) return null;
        return new InviteModuleBuilder { Code = entity.Code };
    }

    #endregion

    #region Cards

    /// <summary>
    ///     Converts the <see cref="ICard"/> to a <see cref="ICardBuilder"/> with the same properties.
    /// </summary>
    public static ICardBuilder ToBuilder(this ICard builder)
    {
        if (builder is null) return null;

        return builder.Type switch
        {
            CardType.Card => (builder as Card).ToBuilder(),
            _ => throw new ArgumentOutOfRangeException(nameof(builder))
        };
    }

    /// <summary>
    ///     Converts the <see cref="Card"/> to a <see cref="CardBuilder"/> with the same properties.
    /// </summary>
    public static CardBuilder ToBuilder(this Card builder)
    {
        if (builder is null) return null;

        return new CardBuilder
        {
            Theme = builder.Theme,
            Size = builder.Size,
            Color = builder.Color,
            Modules = builder.Modules.Select(m => m.ToBuilder()).ToList()
        };
    }

    #endregion
}

using System.Collections.Immutable;
using KaiHeiLa.Utils;

namespace KaiHeiLa;

/// <summary>
///     Represents a header module builder for creating a <see cref="HeaderModule"/>.
/// </summary>
public class HeaderModuleBuilder : IModuleBuilder
{
    private PlainTextElementBuilder _text;

    /// <summary>
    ///     Gets the maximum content length for header allowed by KaiHeiLa.
    /// </summary>
    public const int MaxTitleContentLength = 100;

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Header;

    /// <summary>
    ///     Gets or sets the text of the header.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="text"/> is greater than <see cref="MaxTitleContentLength"/>.",
    /// </exception>
    /// <returns>
    ///     A <see cref="PlainTextElementBuilder"/> representing the text of the header.
    /// </returns>
    public PlainTextElementBuilder Text
    {
        get => _text;
        set
        {
            if (value.Content.Length > MaxTitleContentLength)
                throw new ArgumentException(
                    message: $"Header content length must be less than or equal to {MaxTitleContentLength}.",
                    paramName: nameof(Text));
            _text = value;
        }
    }

    /// <summary>
    ///     Sets the text of the header.
    /// </summary>
    /// <param name="text">
    ///     The text to be set for the header.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="text"/> is greater than <see cref="MaxTitleContentLength"/>.",
    /// </exception>
    public HeaderModuleBuilder WithText(PlainTextElementBuilder text)
    {
        Text = text;
        return this;
    }

    /// <summary>
    ///     Sets the text of the header.
    /// </summary>
    /// <param name="action">
    ///     The action to set the text of the header.
    /// </param>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of result of <paramref name="action"/> is greater than <see cref="MaxTitleContentLength"/>.",
    /// </exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public HeaderModuleBuilder WithText(Action<PlainTextElementBuilder> action)
    {
        PlainTextElementBuilder text = new();
        action(text);
        Text = text;
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="HeaderModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="HeaderModule"/> representing the built header module object.
    /// </returns>
    public HeaderModule Build() => new(Text?.Build());

    /// <inheritdoc />   /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}

/// <summary>
///     Represents a section module builder for creating a <see cref="SectionModule"/>.
/// </summary>
public class SectionModuleBuilder : IModuleBuilder
{
    private IElementBuilder _text;
    private IElementBuilder _accessory;

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Section;

    /// <summary>
    ///     Gets or sets how the <see cref="Accessory"/> is positioned relative to the <see cref="Text"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="SectionAccessoryMode"/> representing
    ///     how the <see cref="Accessory"/> is positioned relative to the <see cref="Text"/>.
    /// </returns>
    public SectionAccessoryMode Mode { get; set; }
    
    /// <summary>
    ///     Gets or sets the text of the section.
    /// </summary>
    /// <returns>
    ///     An <see cref="IElementBuilder"/> representing the text of the section.
    /// </returns>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="value"/> is not any form of text element,
    ///     including <see cref="PlainTextElementBuilder"/>, <see cref="KMarkdownElementBuilder"/>, and <see cref="ParagraphStructBuilder"/>.
    /// </exception>
    public IElementBuilder Text
    {
        get => _text;
        set
        {
            if (value.Type != ElementType.PlainText && value.Type != ElementType.KMarkdown && value.Type != ElementType.Paragraph)
                throw new ArgumentException(
                    message: "Section text must be a PlainText element, a KMarkdown element or a Paragraph struct.",
                    paramName: nameof(value));
            _text = value;
        }
    }

    /// <summary>
    ///     Gets or sets the accessory of the section.
    /// </summary>
    /// <returns>
    ///     An <see cref="IElementBuilder"/> representing the accessory of the section.
    /// </returns>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="value"/> is neither an <see cref="ImageElementBuilder"/>, <see cref="ButtonElementBuilder"/>,
    /// </exception>
    public IElementBuilder Accessory
    {
        get => _accessory;
        set
        {
            if (value.Type != ElementType.Image && value.Type != ElementType.Button)
                throw new ArgumentException(
                    message: "Section text must be a Image element or a Button element..",
                    paramName: nameof(value));
            _accessory = value;
        }
    }
    
    /// <summary>
    ///     Sets the text of the section.
    /// </summary>
    /// <param name="text">
    ///     The text to be set for the section.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SectionModuleBuilder WithText(PlainTextElementBuilder text)
    {
        Text = text;
        return this;
    }

    /// <summary>
    ///     Sets the text of the section.
    /// </summary>
    /// <param name="text">
    ///     The text to be set for the section.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SectionModuleBuilder WithText(KMarkdownElementBuilder text)
    {
        Text = text;
        return this;
    }

    /// <summary>
    ///     Sets the text of the section.
    /// </summary>
    /// <param name="text">The text to be set for the section.</param>
    /// <param name="isKMarkdown">
    ///     A bool indicating whether the text is in KMarkdown format;
    ///     if <c>true</c>, the text will be set as a <see cref="KMarkdownElement"/>;
    ///     if <c>false</c>, the text will be set as a <see cref="PlainTextElement"/>.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SectionModuleBuilder WithText(string text, bool isKMarkdown = false)
    {
        Text = isKMarkdown switch
        {
            false => new PlainTextElementBuilder().WithContent(text),
            true => new KMarkdownElementBuilder().WithContent(text)
        };
        return this;
    }

    /// <summary>
    ///     Sets the text of the section.
    /// </summary>
    /// <param name="text">
    ///     The text to be set for the section.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SectionModuleBuilder WithText(ParagraphStructBuilder text)
    {
        Text = text;
        return this;
    }

    /// <summary>
    ///     Sets the accessory of the section.
    /// </summary>
    /// <param name="accessory">
    ///     The accessory to be set for the section.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SectionModuleBuilder WithAccessory(ImageElementBuilder accessory)
    {
        Accessory = accessory;
        return this;
    }

    /// <summary>
    ///     Sets the accessory of the section.
    /// </summary>
    /// <param name="accessory">
    ///     The accessory to be set for the section.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SectionModuleBuilder WithAccessory(ButtonElementBuilder accessory)
    {
        Accessory = accessory;
        return this;
    }

    /// <summary>
    ///     Sets how the <see cref="Accessory"/> is positioned relative to the <see cref="Text"/>.
    /// </summary>
    /// <param name="mode">
    ///     How the <see cref="Accessory"/> is positioned relative to the <see cref="Text"/>.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SectionModuleBuilder WithMode(SectionAccessoryMode mode)
    {
        Mode = mode;
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="SectionModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="SectionModule"/> representing the built section module object.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     The <see cref="ButtonElement"/> was positioned to the left of the <see cref="Text"/>,
    ///     which is not allowed.
    /// </exception>
    public SectionModule Build()
    {
        if (Mode == SectionAccessoryMode.Left && Accessory is ButtonElementBuilder)
            throw new InvalidOperationException(message: "Button must be placed on the right");
        return new SectionModule(Mode, Text?.Build(), Accessory?.Build());
    }

    /// <inheritdoc />   /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}

/// <summary>
///     Representing an image group module builder for create an <see cref="ImageGroupModule"/>.
/// </summary>
public class ImageGroupModuleBuilder : IModuleBuilder
{
    private List<ImageElementBuilder> _elements;

    /// <summary>
    ///     Returns the maximum number of elements allowed by KaiHeiLa.
    /// </summary>
    public const int MaxElementCount = 9;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageGroupModuleBuilder"/> class.
    /// </summary>
    public ImageGroupModuleBuilder()
    {
        Elements = new List<ImageElementBuilder>();
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.ImageGroup;

    /// <summary>
    ///     Gets or sets the elements of the image group.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    /// <returns>
    ///     An <see cref="ImageElementBuilder"/> containing the elements of the image group.
    /// </returns>
    public List<ImageElementBuilder> Elements
    {
        get => _elements;
        set
        {
            if (value.Count > MaxElementCount)
                throw new ArgumentException(
                    message: $"Element count must be less than or equal to {MaxElementCount}.",
                    paramName: nameof(Elements));
            _elements = value;
        }
    }

    /// <summary>
    ///     Adds an image element to the image group.
    /// </summary>
    /// <param name="field">
    ///     The image element to add.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ImageGroupModuleBuilder AddElement(ImageElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount)
            throw new ArgumentException(
                message: $"Element count must be less than or equal to {MaxElementCount}.",
                paramName: nameof(field));
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds an image element to the image group.
    /// </summary>
    /// <param name="action">
    ///     The action to add an image element to the image group.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ImageGroupModuleBuilder AddElement(Action<ImageElementBuilder> action)
    {
        ImageElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    }

    /// <summary>
    ///     Builds this builder into an <see cref="ImageGroupModule"/>.
    /// </summary>
    /// <returns>
    ///     An <see cref="ImageGroupModule"/> representing the built image group module object.
    /// </returns>
    public ImageGroupModule Build() =>
        new(Elements.Select(e => e.Build()).ToImmutableArray());

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}

/// <summary>
///     Represents a container module builder for creating a <see cref="ContainerModule"/>.
/// </summary>
public class ContainerModuleBuilder : IModuleBuilder
{
    private List<ImageElementBuilder> _elements;

    /// <summary>
    ///     Returns the maximum number of elements allowed by KaiHeiLa.
    /// </summary>
    public const int MaxElementCount = 9;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContainerModuleBuilder"/> class.
    /// </summary>
    public ContainerModuleBuilder()
    {
        Elements = new List<ImageElementBuilder>();
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Container;

    /// <summary>
    ///     Gets or sets the image elements in the container module.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The number of <paramref name="value"/> is greater than <see cref="MaxElementCount"/>.
    /// </exception>
    /// <returns>
    ///     A <see cref="List{ImageElementBuilder}"/> containing the image elements in this image container module.
    /// </returns>
    public List<ImageElementBuilder> Elements
    {
        get => _elements;
        set
        {
            if (value.Count > MaxElementCount)
                throw new ArgumentException(
                    message: $"Element count must be less than or equal to {MaxElementCount}.",
                    paramName: nameof(Elements));
            _elements = value;
        }
    }

    /// <summary>
    ///     Adds an image element to the container module.
    /// </summary>
    /// <param name="field">
    ///     The image element to add.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ContainerModuleBuilder AddElement(ImageElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount)
            throw new ArgumentException(
                message: $"Element count must be less than or equal to {MaxElementCount}.",
                paramName: nameof(field));
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds an image element to the container module.
    /// </summary>
    /// <param name="action">
    ///     The action to add an image element to the container module.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ContainerModuleBuilder AddElement(Action<ImageElementBuilder> action)
    {
        ImageElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="ContainerModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ContainerModule"/> representing the built container module object.
    /// </returns>
    public ContainerModule Build() =>
        new(Elements.Select(e => e.Build()).ToImmutableArray());

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}

/// <summary>
///     Represents a action group module builder for creating an <see cref="ActionGroupModule"/>.
/// </summary>
public class ActionGroupModuleBuilder : IModuleBuilder
{
    private List<ButtonElementBuilder> _elements;

    /// <summary>
    ///     Returns the maximum number of elements allowed by KaiHeiLa.
    /// </summary>
    public const int MaxElementCount = 4;

    public ActionGroupModuleBuilder()
    {
        Elements = new List<ButtonElementBuilder>();
    }


    /// <inheritdoc />
    public ModuleType Type => ModuleType.Container;

    /// <summary>
    ///     Gets or sets the button elements of the action group module.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    /// <returns>
    ///     A <see cref="List{ButtonElementBuilder}"/> containing the button elements of the action group module.
    /// </returns>
    public List<ButtonElementBuilder> Elements
    {
        get => _elements;
        set
        {
            if (value.Count > MaxElementCount)
                throw new ArgumentException(
                    message: $"Element count must be less than or equal to {MaxElementCount}.",
                    paramName: nameof(Elements));
            _elements = value;
        }
    }

    /// <summary>
    ///     Adds a button element to the action group module.
    /// </summary>
    /// <param name="field">
    ///     The button element to add.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ActionGroupModuleBuilder AddElement(ButtonElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount)
            throw new ArgumentException(
                message: $"Element count must be less than or equal to {MaxElementCount}.",
                paramName: nameof(field));
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds a button element to the action group module.
    /// </summary>
    /// <param name="action">
    ///     The action to add a button element to the action group module.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ActionGroupModuleBuilder AddElement(Action<ButtonElementBuilder> action)
    {
        ButtonElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    }

    /// <summary>
    ///     Builds this builder into an <see cref="ActionGroupModule"/>.
    /// </summary>
    /// <returns>
    ///     An <see cref="ActionGroupModule"/> representing the built action group module object.
    /// </returns>
    public ActionGroupModule Build() =>
        new(Elements.Select(e => e.Build()).ToImmutableArray());

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}

/// <summary>
///     Represents a context module builder for creating a <see cref="ContextModule"/>.
/// </summary>
public class ContextModuleBuilder : IModuleBuilder
{
    private List<IElementBuilder> _elements;

    /// <summary>
    ///     Returns the maximum number of elements allowed by KaiHeiLa.
    /// </summary>
    public const int MaxElementCount = 10;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContextModuleBuilder"/> class.
    /// </summary>
    public ContextModuleBuilder()
    {
        Elements = new List<IElementBuilder>();
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Container;

    /// <summary>
    ///     Gets or sets the elements of the context module.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="value"/> contains disallowed type of element builder. Allowed element builders are
    ///     <see cref="PlainTextElementBuilder"/>, <see cref="KMarkdownElementBuilder"/>, and <see cref="ImageElementBuilder"/>.
    /// </exception>
    public List<IElementBuilder> Elements
    {
        get => _elements;
        set
        {
            if (value.Count > MaxElementCount)
                throw new ArgumentException(
                    message: $"Element count must be less than or equal to {MaxElementCount}.",
                    paramName: nameof(Elements));
            if (value.Any(e => e.Type != ElementType.PlainText
                               && e.Type != ElementType.KMarkdown
                               && e.Type != ElementType.Image))
                throw new ArgumentException(
                    message: "Elements must be of type PlainText, KMarkdown or Image.",
                    paramName: nameof(Elements));
            _elements = value;
        }
    }

    /// <summary>
    ///     Adds a PlainText element to the context module.
    /// </summary>
    /// <param name="field">
    ///     The PlainText element to add.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ContextModuleBuilder AddElement(PlainTextElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount)
            throw new ArgumentException(
                message: $"Element count must be less than or equal to {MaxElementCount}.",
                paramName: nameof(field));
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds a PlainText element to the context module.
    /// </summary>
    /// <param name="action">
    ///     The action to add a PlainText element to the context module.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ContextModuleBuilder AddElement(Action<PlainTextElementBuilder> action)
    {
        PlainTextElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    }

    /// <summary>
    ///     Adds a KMarkdown element to the context module.
    /// </summary>
    /// <param name="field">
    ///     The KMarkdown element to add.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ContextModuleBuilder AddElement(KMarkdownElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount)
            throw new ArgumentException(
                message: $"Element count must be less than or equal to {MaxElementCount}.",
                paramName: nameof(field));
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds a KMarkdown element to the context module.
    /// </summary>
    /// <param name="action">
    ///     The action to add a KMarkdown element to the context module.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ContextModuleBuilder AddElement(Action<KMarkdownElementBuilder> action)
    {
        KMarkdownElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    }

    /// <summary>
    ///     Adds an image element to the context module.
    /// </summary>
    /// <param name="field">
    ///     The image element to add.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ContextModuleBuilder AddElement(ImageElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount)
            throw new ArgumentException(
                message: $"Element count must be less than or equal to {MaxElementCount}.",
                paramName: nameof(field));
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds a image element to the context module.
    /// </summary>
    /// <param name="action">
    ///     The action to add a image element to the context module.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ContextModuleBuilder AddElement(Action<ImageElementBuilder> action)
    {
        ImageElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="ContextModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ContextModule"/> representing the built context module object.
    /// </returns>
    public ContextModule Build() =>
        new(Elements.Select(e => e.Build()).ToImmutableArray());

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}

/// <summary>
///     Represents a divider module builder for creating a <see cref="DividerModule"/>.
/// </summary>
public class DividerModuleBuilder : IModuleBuilder
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Divider;

    /// <summary>
    ///     Builds this builder into a <see cref="DividerModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="DividerModule"/> representing the built divider module object.
    /// </returns>
    public DividerModule Build() => new();

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}

/// <summary>
///     Represents a file module builder for creating a <see cref="FileModule"/>.
/// </summary>
public class FileModuleBuilder : IModuleBuilder
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.File;

    /// <summary>
    ///     Gets or sets the source URL of the file.
    /// </summary>
    /// <returns>
    ///     The source URL of the file.
    /// </returns>
    public string Source { get; set; }
    
    /// <summary>
    ///     Gets or sets the title of the file.
    /// </summary>
    /// <returns>
    ///     The title of the file.
    /// </returns>
    public string Title { get; set; }

    /// <summary>
    ///     Sets the source URL of the file.
    /// </summary>
    /// <param name="source">
    ///     The source URL of the file to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public FileModuleBuilder WithSource(string source)
    {
        Source = source;
        return this;
    }

    /// <summary>
    ///     Sets the title of the file.
    /// </summary>
    /// <param name="title">
    ///     The title of the file to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public FileModuleBuilder WithTitle(string title)
    {
        Title = title;
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="FileModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="FileModule"/> representing the built file module object.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     <see cref="Source"/> does not include a protocol (neither HTTP nor HTTPS)
    /// </exception>
    public FileModule Build()
    {
        if (!string.IsNullOrEmpty(Source))
            UrlValidation.Validate(Source);
        return new FileModule(Source, Title);
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}

/// <summary>
///     Represents a video module builder for creating a <see cref="VideoModule"/>.
/// </summary>
public class VideoModuleBuilder : IModuleBuilder
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Video;

    /// <summary>
    ///     Gets or sets the source URL of the video.
    /// </summary>
    /// <returns>
    ///     The source URL of the video.
    /// </returns>
    public string Source { get; set; }
    
    /// <summary>
    ///     Gets or sets the title of the video.
    /// </summary>
    /// <returns>
    ///     The title of the video.
    /// </returns>
    public string Title { get; set; }

    /// <summary>
    ///     Sets the source URL of the video.
    /// </summary>
    /// <param name="source">
    ///     The source URL of the video to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public VideoModuleBuilder WithSource(string source)
    {
        Source = source;
        return this;
    }

    /// <summary>
    ///     Sets the title of the video.
    /// </summary>
    /// <param name="title">
    ///     The title of the video to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public VideoModuleBuilder WithTitle(string title)
    {
        Title = title;
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="VideoModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="VideoModule"/> representing the built video module object.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     <see cref="Source"/> does not include a protocol (neither HTTP nor HTTPS)
    /// </exception>
    public VideoModule Build()
    {
        if (!string.IsNullOrEmpty(Source))
            UrlValidation.Validate(Source);
        return new(Source, Title);
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}

/// <summary>
///     Represents a audio module builder for creating an <see cref="AudioModule"/>.
/// </summary>
public class AudioModuleBuilder : IModuleBuilder
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Audio;

    /// <summary>
    ///     Gets or sets the source URL of the video.
    /// </summary>
    /// <returns>
    ///     The source URL of the video.
    /// </returns>
    public string Source { get; set; }
    
    /// <summary>
    ///     Gets or sets the cover URL of the video.
    /// </summary>
    /// <returns>
    ///     The cover URL of the video.
    /// </returns>
    public string Cover { get; set; }
    
    /// <summary>
    ///     Gets or sets the title of the video.
    /// </summary>
    /// <returns>
    ///     The title of the video.
    /// </returns>
    public string Title { get; set; }

    /// <summary>
    ///     Sets the source URL of the video.
    /// </summary>
    /// <param name="source">
    ///     The source URL of the video to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public AudioModuleBuilder WithSource(string source)
    {
        Source = source;
        return this;
    }

    /// <summary>
    ///     Sets the cover URL of the video.
    /// </summary>
    /// <param name="cover">
    ///     The cover URL of the video to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public AudioModuleBuilder WithCover(string cover)
    {
        Cover = cover;
        return this;
    }

    /// <summary>
    ///     Sets the title of the video.
    /// </summary>
    /// <param name="title">
    ///     The title of the video to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public AudioModuleBuilder WithTitle(string title)
    {
        Title = title;
        return this;
    }

    /// <summary>
    ///     Builds this builder into an <see cref="AudioModule"/>.
    /// </summary>
    /// <returns>
    ///     An <see cref="AudioModule"/> representing the built audio module object.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     <see cref="Source"/> does not include a protocol (neither HTTP nor HTTPS)
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     <see cref="Cover"/> does not include a protocol (neither HTTP nor HTTPS)
    /// </exception>
    public AudioModule Build()
    {
        if (!string.IsNullOrEmpty(Source))
            UrlValidation.Validate(Source);
        if (!string.IsNullOrEmpty(Cover))
            UrlValidation.Validate(Cover);
        return new(Source, Title, Cover);
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}

/// <summary>
///     Represents a countdown module builder for creating a <see cref="CountdownModule"/>.
/// </summary>
public class CountdownModuleBuilder : IModuleBuilder
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Countdown;

    /// <summary>
    ///  Gets or sets the ending time of the countdown.
    /// </summary>
    /// <returns>
    ///     The time at which the countdown ends.
    /// </returns>
    public DateTimeOffset EndTime { get; set; }
    /// <summary>
    ///     Gets or sets the beginning time of the countdown.
    /// </summary>
    /// <returns>
    ///     The time at which the countdown begins.
    /// </returns>
    public DateTimeOffset? StartTime { get; set; }
    /// <summary>
    ///     Gets or sets how the countdown should be displayed.
    /// </summary>
    /// <returns>
    ///     A <see cref="CountdownMode"/> representing how the countdown should be displayed.
    /// </returns>
    public CountdownMode Mode { get; set; }

    /// <summary>
    ///     Sets how the countdown should be displayed.
    /// </summary>
    /// <param name="mode">
    ///     A <see cref="CountdownMode"/> representing how the countdown should be displayed.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public CountdownModuleBuilder WithMode(CountdownMode mode)
    {
        Mode = mode;
        return this;
    }

    /// <summary>
    ///     Sets the beginning time of the countdown.
    /// </summary>
    /// <param name="endTime">
    ///     The time at which the countdown ends.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public CountdownModuleBuilder WithEndTime(DateTimeOffset endTime)
    {
        EndTime = endTime;
        return this;
    }

    /// <summary>
    ///     Sets the beginning time of the countdown.
    /// </summary>
    /// <param name="startTime">
    ///     The time at which the countdown begins.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public CountdownModuleBuilder WithStartTime(DateTimeOffset startTime)
    {
        StartTime = startTime;
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="CountdownModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="CountdownModule"/> representing the built countdown module object.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="EndTime"/> is before the current time.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="StartTime"/> is before the current time.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="EndTime"/> is before <see cref="StartTime"/>
    /// </exception>
    public CountdownModule Build()
    {
        if (Mode != CountdownMode.Second && StartTime is not null)
            throw new InvalidOperationException(
                "Only when the countdown is displayed as second mode can the start time be set.");
        if (EndTime < DateTimeOffset.Now)
            throw new ArgumentOutOfRangeException(
                message: $"{nameof(EndTime)} must be later than current timestamp.",
                paramName: nameof(EndTime));
        if (StartTime is not null && StartTime < DateTimeOffset.Now)
            throw new ArgumentOutOfRangeException(
                message: $"{nameof(StartTime)} must be later than current timestamp.",
                paramName: nameof(StartTime));
        if (StartTime is not null && StartTime >= EndTime)
            throw new ArgumentOutOfRangeException(
                message: $"{nameof(StartTime)} must be later than {nameof(EndTime)}.",
                paramName: nameof(StartTime));
        return new CountdownModule(Mode, EndTime, StartTime);
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}

/// <summary>
///     Represents a invite module builder for creating an <see cref="InviteModule"/>.
/// </summary>
public class InviteModuleBuilder : IModuleBuilder
{

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Invite;

    /// <summary>
    ///     Gets or sets the code of the invite.
    /// </summary>
    /// <returns>
    ///     A <see langword="string"/> representing the code of the invite.
    /// </returns>
    public string Code { get; set; }

    /// <summary>
    ///     Sets the code of the invite.
    /// </summary>
    /// <param name="code">
    ///     The code of the invite to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public InviteModuleBuilder WithCode(string code)
    {
        Code = code;
        return this;
    }

    /// <summary>
    ///     Builds this builder into an <see cref="InviteModule"/>.
    /// </summary>
    /// <returns>
    ///     An <see cref="InviteModule"/> representing the built invite module object.
    /// </returns>
    public InviteModule Build() => new(Code);

    
    /// <summary>
    ///     Initialized a new instance of the <see cref="InviteModuleBuilder"/> class
    ///     with the specified <paramref name="code"/>.
    /// </summary>
    /// <param name="code">
    ///     The code representing the invite.
    /// </param>
    /// <returns>
    ///     An <see cref="InviteModuleBuilder"/> object that is initialized with the specified <paramref name="code"/>.
    /// </returns>
    public static implicit operator InviteModuleBuilder(string code) => new InviteModuleBuilder()
        .WithCode(code);

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}
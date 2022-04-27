using System.Collections.Immutable;
using KaiHeiLa.Utils;

namespace KaiHeiLa;

/// <summary>
///     A element builder to build a <see cref="PlainTextElement"/>.
/// </summary>
public class PlainTextElementBuilder : IElementBuilder
{
    private string _content;

    /// <summary>
    ///     Gets the maximum plain text length allowed by KaiHeiLa.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> that represents the maximum plain text length allowed by KaiHeiLa.
    /// </returns>
    public const int MaxPlainTextLength = 2000;

    /// <summary>
    ///     Gets the type of the element that this builder builds.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> that represents the type of element that this builder builds.
    /// </returns>
    public ElementType Type => ElementType.PlainText;
    
    /// <summary>
    ///     Gets or sets the content of an <see cref="PlainTextElement"/>.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="value"/> is greater than <see cref="MaxPlainTextLength"/>.
    /// </exception>
    /// <returns>
    ///     The content of the <see cref="PlainTextElement"/>.
    /// </returns>
    public string Content
    {
        get => _content;
        set
        {
            if (value?.Length > MaxPlainTextLength)
                throw new ArgumentException(
                    message: $"Plain text length must be less than or equal to {MaxPlainTextLength}.",
                    paramName: nameof(Content));
            _content = value;
        }
    }

    /// <summary>
    ///     // TODO: To be documented.
    /// </summary>
    public bool Emoji { get; set; } = true;

    /// <summary>
    ///     Sets the content of an <see cref="PlainTextElement"/>.
    /// </summary>
    /// <param name="content">The text to be set as the content.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public PlainTextElementBuilder WithContent(string content)
    {
        Content = content;
        return this;
    }
    /// <summary>
    ///     // TODO: To be documented.
    /// </summary>
    /// <param name="emoji">
    ///     // TODO: To be documented.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public PlainTextElementBuilder WithEmoji(bool emoji)
    {
        Emoji = emoji;
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="PlainTextElementBuilder"/> into an <see cref="PlainTextElement"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="PlainTextElement"/> represents the built element object.
    /// </returns>
    public PlainTextElement Build() 
        => new PlainTextElement(Content, Emoji);

    /// <summary>
    ///     Initialized a new instance of the <see cref="PlainTextElementBuilder"/> class
    ///     with the specified content.
    /// </summary>
    /// <param name="content">
    ///     The content of the <see cref="PlainTextElement"/>.
    /// </param>
    /// <returns>
    ///     A <see cref="PlainTextElementBuilder"/> object that is initialized with the specified content.
    /// </returns>
    public static implicit operator PlainTextElementBuilder(string content) => 
        new PlainTextElementBuilder().WithContent(content);
    
    /// <inheritdoc />
    IElement IElementBuilder.Build() => Build();
}

/// <summary>
///     A element builder to build a <see cref="KMarkdownElement"/>.
/// </summary>
public class KMarkdownElementBuilder : IElementBuilder
{
    private string _content;

    /// <summary>
    ///     Gets the maximum KMarkdown length allowed by KaiHeiLa.
    /// </summary>
    public const int MaxKMarkdownLength = 5000;

    /// <summary>
    ///     Gets the type of the element that this builder builds.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> that represents the type of element that this builder builds.
    /// </returns>
    public ElementType Type => ElementType.KMarkdown;
    /// <summary>
    ///     Gets or sets the content of an <see cref="KMarkdownElementBuilder"/>.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="value"/> is greater than <see cref="MaxKMarkdownLength"/>.
    /// </exception>
    /// <returns>
    ///     The content of the <see cref="KMarkdownElementBuilder"/>.
    /// </returns>
    public string Content
    {
        get => _content;
        set
        {
            if (value?.Length > MaxKMarkdownLength)
                throw new ArgumentException(
                    message: $"KMarkdown length must be less than or equal to {MaxKMarkdownLength}.",
                    paramName: nameof(Content));
            _content = value;
        }
    }

    /// <summary>
    ///     Sets the content of an <see cref="KMarkdownElementBuilder"/>.
    /// </summary>
    /// <param name="content">The text to be set as the content.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public KMarkdownElementBuilder WithContent(string content)
    {
        Content = content;
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="KMarkdownElementBuilder"/> into an <see cref="KMarkdownElement"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="KMarkdownElement"/> represents the built element object.
    /// </returns>
    public KMarkdownElement Build() 
        => new KMarkdownElement(Content);

    /// <summary>
    ///     Initialized a new instance of the <see cref="KMarkdownElementBuilder"/> class
    ///     with the specified content.
    /// </summary>
    /// <param name="content">
    ///     The content of the <see cref="KMarkdownElement"/>.
    /// </param>
    /// <returns>
    ///     A <see cref="KMarkdownElementBuilder"/> object that is initialized with the specified content.
    /// </returns>
    public static implicit operator KMarkdownElementBuilder(string content) => 
        new KMarkdownElementBuilder().WithContent(content);
    
    /// <inheritdoc />
    IElement IElementBuilder.Build() => Build();
}

/// <summary>
///     A element builder to build an <see cref="ImageElement"/>.
/// </summary>
public class ImageElementBuilder : IElementBuilder
{
    private string _alternative;

    /// <summary>
    ///     Gets the maximum image alternative text length allowed by KaiHeiLa.
    /// </summary>
    public const int MaxAlternativeLength = 20;
    
    /// <summary>
    ///     Gets the type of the element that this builder builds.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> that represents the type of element that this builder builds.
    /// </returns>
    public ElementType Type => ElementType.Image;
    /// <summary>
    ///     Gets or sets the source of an <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="string"/> that represents the source of the <see cref="ImageElementBuilder"/>.
    /// </returns>
    public string Source { get; set; }

    /// <summary>
    ///     Gets or sets the alternative text of an <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="value"/> is greater than <see cref="MaxAlternativeLength"/>.
    /// </exception>
    /// <returns>
    ///     A <see cref="string"/> that represents the alternative text of the <see cref="ImageElementBuilder"/>.
    /// </returns>
    public string Alternative
    {
        get => _alternative;
        set
        {
            if (value?.Length > MaxAlternativeLength)
                throw new ArgumentException(
                    message: $"Image alternative length must be less than or equal to {MaxAlternativeLength}.",
                    paramName: nameof(Alternative));
            _alternative = value;
        }
    }

    /// <summary>
    ///     Gets or sets the size of the image of an <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ImageSize"/> that represents the size of the image of the <see cref="ImageElementBuilder"/>;
    ///     <c>null</c> if the size is not specified.
    /// </returns>
    public ImageSize? Size { get; set; }
    /// <summary>
    ///     Gets or sets whether the image should be rendered as a circle.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the image should be rendered as a circle; otherwise, <c>false</c>;
    ///     or <c>null</c> if whether the image should be rendered as a circle is not specified.
    /// </returns>
    public bool? Circle { get; set; }
    
    /// <summary>
    ///     Sets the source of an <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <param name="source">
    ///     The source to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ImageElementBuilder WithSource(string source)
    {
        Source = source;
        return this;
    }
    /// <summary>
    ///     Sets the alternative text of an <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <param name="alternative">
    ///     The alternative text to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ImageElementBuilder WithAlternative(string alternative)
    {
        Alternative = alternative;
        return this;
    }
    /// <summary>
    ///     Sets the size of the image of an <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <param name="size">
    ///     The size to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ImageElementBuilder WithSize(ImageSize size)
    {
        Size = size;
        return this;
    }
    /// <summary>
    ///     Sets whether the image should be rendered as a circle.
    /// </summary>
    /// <param name="circle">
    ///     <c>true</c> if the image should be rendered as a circle; otherwise, <c>false</c>.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ImageElementBuilder WithCircle(bool circle)
    {
        Circle = circle;
        return this;
    }
    
    /// <summary>
    ///     Builds the <see cref="ImageElementBuilder"/> into an <see cref="ImageElement"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ImageElement"/> represents the built element object.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     The source url does not include a protocol (either HTTP or HTTPS).
    /// </exception>
    public ImageElement Build()
    {
        if (!string.IsNullOrEmpty(Source))
            UrlValidation.Validate(Source);
        return new ImageElement(Source, Alternative, Size, Circle);
    }
    
    /// <summary>
    ///     Initialized a new instance of the <see cref="ImageElementBuilder"/> class
    ///     with the specified content.
    /// </summary>
    /// <param name="source">
    ///     The content of the <see cref="ImageElement"/>.
    /// </param>
    /// <returns>
    ///     A <see cref="ImageElementBuilder"/> object that is initialized with the specified image source.
    /// </returns>
    public static implicit operator ImageElementBuilder(string source) => new ImageElementBuilder()
        .WithSource(source);
    
    /// <inheritdoc />
    IElement IElementBuilder.Build() => Build();
}

/// <summary>
///     A element builder to build an <see cref="ButtonElement"/>.
/// </summary>
public class ButtonElementBuilder : IElementBuilder
{
    private IElementBuilder _text;

    /// <summary>
    ///     Gets the type of the element that this builder builds.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> that represents the type of element that this builder builds.
    /// </returns>
    public ElementType Type => ElementType.Button;
    
    /// <summary>
    ///     Gets or sets the theme of the button.
    /// </summary>
    /// <returns>
    ///     A <see cref="ButtonTheme"/> that represents the theme of the button.
    /// </returns>
    public ButtonTheme Theme { get; set; }
    
    /// <summary>
    ///     Gets or sets the value of the button.
    /// </summary>
    /// <returns>
    ///     A <see cref="string"/> that represents the value of the button.
    /// </returns>
    /// <remarks>
    ///     If the <see cref="Click"/> is set to <see cref="ButtonClickEventType.ReturnValue"/>,
    ///     the value of the property will be returned when the button is clicked.
    /// </remarks>
    public string Value { get; set; }
    
    /// <summary>
    ///     Gets or sets the type of the click event.
    /// </summary>
    /// <returns>
    ///     A <see cref="ButtonClickEventType"/> that represents the type of the click event.
    /// </returns>
    public ButtonClickEventType Click { get; set; }

    /// <summary>
    ///     Gets or sets the text element of the button.
    /// </summary>
    /// <returns>
    ///     An <see cref="IElementBuilder"/> that represents the text of the button.
    /// </returns>
    /// <exception cref="InvalidOperationException" accessor="set">
    ///     The <paramref name="value"/> is neither a <see cref="PlainTextElementBuilder"/> nor a <see cref="KMarkdownElementBuilder"/>.
    /// </exception>
    /// <remarks>
    ///     This property only takes a <see cref="PlainTextElementBuilder"/> or a <see cref="KMarkdownElementBuilder"/>.
    /// </remarks>
    public IElementBuilder Text
    {
        get => _text;
        set
        {
            if (value is not PlainTextElementBuilder && value is not KMarkdownElementBuilder)
                throw new InvalidOperationException("The text of a button must be a PlainTextElementBuilder or a KMarkdownElementBuilder.");
            _text = value;
        }
    }

    /// <summary>
    ///     Sets the theme of an <see cref="ButtonElement"/>.
    /// </summary>
    /// <param name="theme">The theme to be set.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ButtonElementBuilder WithTheme(ButtonTheme theme)
    {
        Theme = theme;
        return this;
    }
    /// <summary>
    ///     Sets the value of an <see cref="ButtonElement"/>.
    /// </summary>
    /// <param name="value">The value to be set.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ButtonElementBuilder WithValue(string value)
    {
        Value = value;
        return this;
    }
    /// <summary>
    ///     Sets the type of the event to be fired when the button is clicked in an <see cref="ButtonElement"/>.
    /// </summary>
    /// <param name="click">
    ///     The type of the event to be fired when the button is clicked.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ButtonElementBuilder WithClick(ButtonClickEventType click)
    {
        Click = click;
        return this;
    }
    /// <summary>
    ///     Sets the text of an <see cref="ButtonElement"/>.
    /// </summary>
    /// <param name="text">
    ///     The builder of a <see cref="PlainTextElement"/>, which will be set as the text of the button.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ButtonElementBuilder WithText(PlainTextElementBuilder text)
    {
        Text = text;
        return this;
    }
    /// <summary>
    ///     Sets the text of an <see cref="ButtonElement"/>.
    /// </summary>
    /// <param name="action">
    ///     The action to create a builder of a <see cref="PlainTextElement"/>, which will be set as the text of the button.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ButtonElementBuilder WithText(Action<PlainTextElementBuilder> action)
    {
        PlainTextElementBuilder text = new();
        action(text);
        Text = text;
        return this;
    }
    /// <summary>
    ///     Sets the text of an <see cref="ButtonElement"/>.
    /// </summary>
    /// <param name="text">
    ///     The builder of a <see cref="KMarkdownElement"/>, which will be set as the text of the button.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ButtonElementBuilder WithText(KMarkdownElementBuilder text)
    {
        Text = text;
        return this;
    }
    /// <summary>
    ///     Sets the text of an <see cref="ButtonElement"/>.
    /// </summary>
    /// <param name="action">
    ///     The action to create a builder of a <see cref="KMarkdownElement"/>, which will be set as the text of the button.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ButtonElementBuilder WithText(Action<KMarkdownElementBuilder> action)
    {
        KMarkdownElementBuilder text = new();
        action(text);
        Text = text;
        return this;
    }
    /// <summary>
    ///     Sets the text of an <see cref="ButtonElement"/>.
    /// </summary>
    /// <param name="text">
    ///     A <see cref="string"/> to be set as the text of the button.
    /// </param>
    /// <param name="isKMarkdown">
    ///     A <see cref="bool"/> indicating whether the text is in KMarkdown format;
    ///     if <c>true</c>, the text will be set as a <see cref="KMarkdownElement"/>;
    ///     if <c>false</c>, the text will be set as a <see cref="PlainTextElement"/>.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ButtonElementBuilder WithText(string text, bool isKMarkdown = false)
    {
        Text = isKMarkdown switch
        {
            false => new PlainTextElementBuilder().WithContent(text),
            true => new KMarkdownElementBuilder().WithContent(text)
        };
        return this;
    }
    
    /// <summary>
    ///     Builds the <see cref="ButtonElementBuilder"/> into an <see cref="ButtonElement"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ButtonElement"/> represents the built element object.
    /// </returns>
    public ButtonElement Build() 
        => new ButtonElement(Theme, Value, Click, Text?.Build());
    
    /// <inheritdoc />
    IElement IElementBuilder.Build() => Build();
}

/// <summary>
///     A element builder to build a <see cref="ParagraphStruct"/>.
/// </summary>
public class ParagraphStructBuilder : IElementBuilder
{
    private int _columnCount;
    private List<IElementBuilder> _fields;

    /// <summary>
    ///     Returns the maximum number of fields allowed by KaiHeiLa.
    /// </summary>
    public const int MaxFieldCount = 50;
    /// <summary>
    ///     Returns the minimum number of columns allowed by KaiHeiLa.
    /// </summary>
    public const int MinColumnCount = 1;
    /// <summary>
    ///     Returns the maximum number of columns allowed by KaiHeiLa.
    /// </summary>
    public const int MaxColumnCount = 3;

    /// <summary>
    ///     Initializes a new <see cref="ParagraphStructBuilder"/> class.
    /// </summary>
    public ParagraphStructBuilder()
    {
        Fields = new List<IElementBuilder>();
    }
    
    /// <summary>
    ///     Gets the type of the element that this builder builds.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> that represents the type of element that this builder builds.
    /// </returns>
    public ElementType Type => ElementType.Paragraph;
    
    /// <summary>
    ///     Gets or sets the number of columns of the paragraph.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="value"/> is less than <see cref="MinColumnCount"/> or greater than <see cref="MaxColumnCount"/>.
    /// </exception>
    /// <returns>
    ///     An <see cref="int"/> that represents the number of columns of the paragraph.
    /// </returns>
    public int ColumnCount
    {
        get => _columnCount;
        set
        {
            _columnCount = value switch
            {
                < MinColumnCount => throw new ArgumentException(
                    message: $"Column must be more than or equal to {MinColumnCount}.", paramName: nameof(ColumnCount)),
                > MaxColumnCount => throw new ArgumentException(
                    message: $"Column must be less than or equal to {MaxColumnCount}.", paramName: nameof(ColumnCount)),
                _ => value
            };
        }
    }
    /// <summary>
    ///     Gets or sets the fields of the paragraph.
    /// </summary>
    /// <exception cref="ArgumentNullException" accessor="set">
    ///     The <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="value"/> contains more than <see cref="MaxFieldCount"/> elements.
    /// </exception>
    /// <exception cref="InvalidOperationException" accessor="set">
    ///     The <paramref name="value"/> contains an element that is neither a <see cref="PlainTextElementBuilder"/> nor a <see cref="KMarkdownElementBuilder"/>.
    /// </exception>
    /// <returns>
    ///     A <see cref="List{IElementBuilder}"/> that represents the fields of the paragraph.
    /// </returns>
    public List<IElementBuilder> Fields
    {
        get => _fields;
        set
        {
            if (value == null) throw new ArgumentNullException(
                    paramName: nameof(Fields),
                    message: "Cannot set an paragraph struct builder's fields collection to null.");
            if (value.Count > MaxFieldCount) throw new ArgumentException(
                    message: $"Field count must be less than or equal to {MaxFieldCount}.",
                    paramName: nameof(Fields));
            if (value.Any(field => field is not PlainTextElementBuilder && field is not KMarkdownElementBuilder)) throw new InvalidOperationException(
                    message: "The elements of fields in a paragraph must be PlainTextElementBuilder or KMarkdownElementBuilder.");
                
            _fields = value;
        }
    }

    /// <summary>
    ///     Sets the number of columns of the paragraph.
    /// </summary>
    /// <param name="count">
    ///     An <see cref="int"/> that represents the number of columns of the paragraph.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ParagraphStructBuilder WithColumnCount(int count)
    {
        ColumnCount = count;
        return this;
    }
    /// <summary>
    ///     Adds a field to the paragraph.
    /// </summary>
    /// <param name="field">
    ///     A <see cref="PlainTextElementBuilder"/> that represents the field to add.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     The addition operation will result in a field count greater than <see cref="MaxFieldCount"/>.
    /// </exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ParagraphStructBuilder AddField(PlainTextElementBuilder field)
    {
        if (Fields.Count >= MaxFieldCount) throw new ArgumentException(
            message: $"Field count must be less than or equal to {MaxFieldCount}.",
            paramName: nameof(field));
        Fields.Add(field);
        return this;
    }
    /// <summary>
    ///     Adds a field to the paragraph.
    /// </summary>
    /// <param name="action">
    ///     The action to create a builder of a <see cref="PlainTextElement"/>, which will be added to the paragraph.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     The addition operation will result in a field count greater than <see cref="MaxFieldCount"/>.
    /// </exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ParagraphStructBuilder AddField(Action<PlainTextElementBuilder> action)
    {
        PlainTextElementBuilder field = new();
        action(field);
        AddField(field);
        return this;
    }
    /// <summary>
    ///     Adds a field to the paragraph.
    /// </summary>
    /// <param name="field">
    ///     A <see cref="KMarkdownElementBuilder"/> that represents the field to add.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     The addition operation will result in a field count greater than <see cref="MaxFieldCount"/>.
    /// </exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ParagraphStructBuilder AddField(KMarkdownElementBuilder field)
    {
        if (Fields.Count >= MaxFieldCount) throw new ArgumentException(
            message: $"Field count must be less than or equal to {MaxFieldCount}.",
            paramName: nameof(field));
        Fields.Add(field);
        return this;
    }
    /// <summary>
    ///     Adds a field to the paragraph.
    /// </summary>
    /// <param name="action">
    ///     The action to create a builder of a <see cref="KMarkdownElement"/>, which will be added to the paragraph.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     The addition operation will result in a field count greater than <see cref="MaxFieldCount"/>.
    /// </exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ParagraphStructBuilder AddField(Action<KMarkdownElementBuilder> action)
    {
        KMarkdownElementBuilder field = new();
        action(field);
        AddField(field);
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="ParagraphStructBuilder"/> into an <see cref="ParagraphStruct"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ParagraphStruct"/> represents the built element object.
    /// </returns>
    public ParagraphStruct Build() => 
        new(ColumnCount, Fields.Select(f => f.Build()).ToImmutableArray());
    
    /// <inheritdoc />
    IElement IElementBuilder.Build() => Build();
}
using System.Collections.Immutable;
using Kook.Utils;

namespace Kook;

/// <summary>
///     An element builder to build a <see cref="PlainTextElement"/>.
/// </summary>
public class PlainTextElementBuilder : IElementBuilder, IEquatable<PlainTextElementBuilder>
{
    private string _content;

    /// <summary>
    ///     Gets the maximum plain text length allowed by Kook.
    /// </summary>
    /// <returns>
    ///     An int that represents the maximum plain text length allowed by Kook.
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
    ///     Gets or sets the content of a <see cref="PlainTextElement"/>.
    /// </summary>
    /// <returns>
    ///     The content of the <see cref="PlainTextElement"/>.
    /// </returns>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="value"/> cannot be null.
    /// </exception>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="value"/> is greater than <see cref="MaxPlainTextLength"/>.
    /// </exception>
    public string Content
    {
        get => _content;
        set
        {
            if (value is null)
                throw new ArgumentException("The content cannot be null.", nameof(value));
            if (value.Length > MaxPlainTextLength)
                throw new ArgumentException(
                    message: $"Plain text length must be less than or equal to {MaxPlainTextLength}.",
                    paramName: nameof(Content));
            _content = value;
        }
    }

    /// <summary>
    ///     Gets whether the shortcuts should be translated into emojis.
    /// </summary>
    /// <returns>
    ///     A boolean value that indicates whether the shortcuts should be translated into emojis.
    ///     <c>true</c> if the shortcuts should be translated into emojis;
    ///     <c>false</c> if the text should be displayed as is.
    /// </returns>
    public bool Emoji { get; set; } = true;

    /// <summary>
    ///     Sets the content of a <see cref="PlainTextElement"/>.
    /// </summary>
    /// <param name="content">The text to be set as the content.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="content"/> cannot be null.
    /// </exception>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="content"/> is greater than <see cref="MaxPlainTextLength"/>.
    /// </exception>
    public PlainTextElementBuilder WithContent(string content)
    {
        Content = content;
        return this;
    }
    
    /// <summary>
    ///     Sets whether the shortcuts should be translated into emojis.
    /// </summary>
    /// <param name="emoji">
    ///     A boolean value that indicates whether the shortcuts should be translated into emojis.
    ///     <c>true</c> if the shortcuts should be translated into emojis;
    ///     <c>false</c> if the text should be displayed as is.
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
    ///     Builds the <see cref="PlainTextElementBuilder"/> into a <see cref="PlainTextElement"/>.
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
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="content"/> cannot be null.
    /// </exception>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="content"/> is greater than <see cref="MaxPlainTextLength"/>.
    /// </exception>
    public static implicit operator PlainTextElementBuilder(string content) => 
        new PlainTextElementBuilder().WithContent(content);
    
    /// <inheritdoc />
    IElement IElementBuilder.Build() => Build();

    public static bool operator ==(PlainTextElementBuilder left, PlainTextElementBuilder right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(PlainTextElementBuilder left, PlainTextElementBuilder right)
        => !(left == right);

    public override bool Equals(object obj)
        => obj is PlainTextElementBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="PlainTextElementBuilder"/> is equal to the current <see cref="PlainTextElementBuilder"/>.</summary>
    /// <param name="plainTextElementBuilder">The <see cref="PlainTextElementBuilder"/> to compare with the current <see cref="PlainTextElementBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="PlainTextElementBuilder"/> is equal to the current <see cref="PlainTextElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(PlainTextElementBuilder plainTextElementBuilder)
    {
        if (plainTextElementBuilder is null)
            return false;
        
        return Type == plainTextElementBuilder.Type
            && Content == plainTextElementBuilder.Content
            && Emoji == plainTextElementBuilder.Emoji;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();
}

/// <summary>
///     An element builder to build a <see cref="KMarkdownElement"/>.
/// </summary>
public class KMarkdownElementBuilder : IElementBuilder, IEquatable<KMarkdownElementBuilder>
{
    private string _content;

    /// <summary>
    ///     Gets the maximum KMarkdown length allowed by Kook.
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
    ///     Gets or sets the content of a <see cref="KMarkdownElementBuilder"/>.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="value"/> cannot be null.
    /// </exception>
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
            if (value is null)
                throw new ArgumentException("The content cannot be null.", nameof(value));
            if (value.Length > MaxKMarkdownLength)
                throw new ArgumentException(
                    message: $"KMarkdown length must be less than or equal to {MaxKMarkdownLength}.",
                    paramName: nameof(Content));
            _content = value;
        }
    }

    /// <summary>
    ///     Sets the content of a <see cref="KMarkdownElementBuilder"/>.
    /// </summary>
    /// <param name="content">The text to be set as the content.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="content"/> cannot be null.
    /// </exception>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="content"/> is greater than <see cref="MaxKMarkdownLength"/>.
    /// </exception>
    public KMarkdownElementBuilder WithContent(string content)
    {
        Content = content;
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="KMarkdownElementBuilder"/> into a <see cref="KMarkdownElement"/>.
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
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="content"/> cannot be null.
    /// </exception>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="content"/> is greater than <see cref="MaxKMarkdownLength"/>.
    /// </exception>
    public static implicit operator KMarkdownElementBuilder(string content) => 
        new KMarkdownElementBuilder().WithContent(content);
    
    /// <inheritdoc />
    IElement IElementBuilder.Build() => Build();

    public static bool operator ==(KMarkdownElementBuilder left, KMarkdownElementBuilder right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(KMarkdownElementBuilder left, KMarkdownElementBuilder right)
        => !(left == right);

    public override bool Equals(object obj)
        => obj is KMarkdownElementBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="KMarkdownElementBuilder"/> is equal to the current <see cref="KMarkdownElementBuilder"/>.</summary>
    /// <param name="kMarkdownElementBuilder">The <see cref="KMarkdownElementBuilder"/> to compare with the current <see cref="KMarkdownElementBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="KMarkdownElementBuilder"/> is equal to the current <see cref="KMarkdownElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(KMarkdownElementBuilder kMarkdownElementBuilder)
    {
        if (kMarkdownElementBuilder is null)
            return false;
        
        return Type == kMarkdownElementBuilder.Type
               && Content == kMarkdownElementBuilder.Content;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();
}

/// <summary>
///     An element builder to build an <see cref="ImageElement"/>.
/// </summary>
public class ImageElementBuilder : IElementBuilder, IEquatable<ImageElementBuilder>
{
    private string _alternative;

    /// <summary>
    ///     Gets the maximum image alternative text length allowed by Kook.
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
    ///     A string that represents the source of the <see cref="ImageElementBuilder"/>.
    /// </returns>
    public string Source { get; set; }

    /// <summary>
    ///     Gets or sets the alternative text of an <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="value"/> is greater than <see cref="MaxAlternativeLength"/>.
    /// </exception>
    /// <returns>
    ///     A string that represents the alternative text of the <see cref="ImageElementBuilder"/>.
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
    ///     An <see cref="ImageSize"/> that represents the size of the image of the <see cref="ImageElementBuilder"/>;
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
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="alternative"/> is greater than <see cref="MaxAlternativeLength"/>.
    /// </exception>
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
    ///     An <see cref="ImageElement"/> represents the built element object.
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
    ///     An <see cref="ImageElementBuilder"/> object that is initialized with the specified image source.
    /// </returns>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="source"/> is greater than <see cref="MaxAlternativeLength"/>.
    /// </exception>
    public static implicit operator ImageElementBuilder(string source) => new ImageElementBuilder()
        .WithSource(source);
    
    /// <inheritdoc />
    IElement IElementBuilder.Build() => Build();

    public static bool operator ==(ImageElementBuilder left, ImageElementBuilder right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(ImageElementBuilder left, ImageElementBuilder right)
        => !(left == right);

    public override bool Equals(object obj)
        => obj is ImageElementBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ImageElementBuilder"/> is equal to the current <see cref="ImageElementBuilder"/>.</summary>
    /// <param name="imageElementBuilder">The <see cref="ImageElementBuilder"/> to compare with the current <see cref="ImageElementBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ImageElementBuilder"/> is equal to the current <see cref="ImageElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(ImageElementBuilder imageElementBuilder)
    {
        if (imageElementBuilder is null)
            return false;
        
        return Type == imageElementBuilder.Type
            && Source == imageElementBuilder.Source
            && Alternative == imageElementBuilder.Alternative
            && Size == imageElementBuilder.Size
            && Circle == imageElementBuilder.Circle;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();
}

/// <summary>
///     An element builder to build a <see cref="ButtonElement"/>.
/// </summary>
public class ButtonElementBuilder : IElementBuilder, IEquatable<ButtonElementBuilder>
{
    private IElementBuilder _text;

    /// <summary>
    ///     Gets the maximum button text length allowed by Kook.
    /// </summary>
    public const int MaxButtonTextLength = 40;
    
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
    ///     A string that represents the value of the button.
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
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="value"/> is neither a <see cref="PlainTextElementBuilder"/> nor a <see cref="KMarkdownElementBuilder"/>.
    /// </exception>
    /// <remarks>
    ///     This property only takes a <see cref="PlainTextElementBuilder"/> or a <see cref="KMarkdownElementBuilder"/>.
    /// </remarks>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="value"/> is greater than <see cref="MaxButtonTextLength"/>.
    /// </exception>
    public IElementBuilder Text
    {
        get => _text;
        set
        {
            string text = value switch
            {
                PlainTextElementBuilder plainText => plainText.Content,
                KMarkdownElementBuilder kMarkdown => kMarkdown.Content,
                _ => throw new ArgumentException(
                    $"The text of a button must be a {nameof(PlainTextElementBuilder)} or a {nameof(KMarkdownElementBuilder)}.",
                    nameof(value))
            };
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("The content cannot be null or empty.", nameof(value));
            if (text.Length > MaxButtonTextLength)
                throw new ArgumentException(
                    message: $"The length of button text must be less than or equal to {MaxButtonTextLength}.",
                    paramName: nameof(value));
            _text = value;
        }
    }

    /// <summary>
    ///     Sets the theme of a <see cref="ButtonElement"/>.
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
    ///     Sets the value of a <see cref="ButtonElement"/>.
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
    ///     Sets the type of the event to be fired when the button is clicked in a <see cref="ButtonElement"/>.
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
    ///     Sets the text of a <see cref="ButtonElement"/>.
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
    ///     Sets the text of a <see cref="ButtonElement"/>.
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
    ///     Sets the text of a <see cref="ButtonElement"/>.
    /// </summary>
    /// <param name="action">
    ///     The action to create a builder of an <see cref="IElementBuilder"/>,
    ///     which will be set as the text of the button.
    ///     The action must return a <see cref="PlainTextElementBuilder"/> or a <see cref="KMarkdownElementBuilder"/>.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ButtonElementBuilder WithText<T>(Action<T> action = null)
        where T : IElementBuilder, new()
    {
        T text = new();
        action?.Invoke(text);
        Text = text;
        return this;
    }
    /// <summary>
    ///     Sets the text of a <see cref="ButtonElement"/>.
    /// </summary>
    /// <param name="text">
    ///     A string to be set as the text of the button.
    /// </param>
    /// <param name="isKMarkdown">
    ///     A bool indicating whether the text is in KMarkdown format;
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
    ///     Builds the <see cref="ButtonElementBuilder"/> into a <see cref="ButtonElement"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ButtonElement"/> represents the built element object.
    /// </returns>
    public ButtonElement Build()
    {
        if (Click == ButtonClickEventType.Link && !UrlValidation.Validate(Value))
            throw new ArgumentException("The value of a button with a link event type cannot be null or empty.", nameof(Value));
        return new ButtonElement(Theme, Value, Click, Text?.Build());
    }

    /// <inheritdoc />
    IElement IElementBuilder.Build() => Build();

    public static bool operator ==(ButtonElementBuilder left, ButtonElementBuilder right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(ButtonElementBuilder left, ButtonElementBuilder right)
        => !(left == right);

    public override bool Equals(object obj)
        => obj is ButtonElementBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ButtonElementBuilder"/> is equal to the current <see cref="ButtonElementBuilder"/>.</summary>
    /// <param name="buttonElementBuilder">The <see cref="ButtonElementBuilder"/> to compare with the current <see cref="ButtonElementBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ButtonElementBuilder"/> is equal to the current <see cref="ButtonElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(ButtonElementBuilder buttonElementBuilder)
    {
        if (buttonElementBuilder is null)
            return false;
        
        return Type == buttonElementBuilder.Type
            && Theme == buttonElementBuilder.Theme
            && Value == buttonElementBuilder.Value
            && Click == buttonElementBuilder.Click
            && Text == buttonElementBuilder.Text;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();
}

/// <summary>
///     An element builder to build a <see cref="ParagraphStruct"/>.
/// </summary>
public class ParagraphStructBuilder : IElementBuilder, IEquatable<ParagraphStructBuilder>
{
    private int _columnCount;
    private List<IElementBuilder> _fields;

    /// <summary>
    ///     Returns the maximum number of fields allowed by Kook.
    /// </summary>
    public const int MaxFieldCount = 50;
    /// <summary>
    ///     Returns the minimum number of columns allowed by Kook.
    /// </summary>
    public const int MinColumnCount = 1;
    /// <summary>
    ///     Returns the maximum number of columns allowed by Kook.
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
    ///     An int that represents the number of columns of the paragraph.
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
    /// <exception cref="ArgumentException" accessor="set">
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
            if (value.Any(field => field is not PlainTextElementBuilder && field is not KMarkdownElementBuilder)) 
                throw new ArgumentException(
                    message: "The elements of fields in a paragraph must be PlainTextElementBuilder or KMarkdownElementBuilder.",
                    paramName: nameof(Fields));
                
            _fields = value;
        }
    }

    /// <summary>
    ///     Sets the number of columns of the paragraph.
    /// </summary>
    /// <param name="count">
    ///     An int that represents the number of columns of the paragraph.
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
    public ParagraphStructBuilder AddField<T>(Action<T> action = null)
        where T : IElementBuilder, new()
    {
        T field = new();
        action?.Invoke(field);
        switch (field)
        {
            case PlainTextElementBuilder plainText:
                AddField(plainText);
                break;
            case KMarkdownElementBuilder kMarkdown:
                AddField(kMarkdown);
                break;
            default:
                throw new ArgumentException(
                    message: "The elements of fields in a paragraph must be PlainTextElementBuilder or KMarkdownElementBuilder.",
                    paramName: nameof(field));
                
        }
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="ParagraphStructBuilder"/> into a <see cref="ParagraphStruct"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ParagraphStruct"/> represents the built element object.
    /// </returns>
    public ParagraphStruct Build() => 
        new(ColumnCount, Fields.Select(f => f.Build()).ToImmutableArray());
    
    /// <inheritdoc />
    IElement IElementBuilder.Build() => Build();

    public static bool operator ==(ParagraphStructBuilder left, ParagraphStructBuilder right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(ParagraphStructBuilder left, ParagraphStructBuilder right)
        => !(left == right);

    public override bool Equals(object obj)
        => obj is ParagraphStructBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ParagraphStructBuilder"/> is equal to the current <see cref="ParagraphStructBuilder"/>.</summary>
    /// <param name="paragraphStructBuilder">The <see cref="ParagraphStructBuilder"/> to compare with the current <see cref="ParagraphStructBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ParagraphStructBuilder"/> is equal to the current <see cref="ParagraphStructBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(ParagraphStructBuilder paragraphStructBuilder)
    {
        if (paragraphStructBuilder is null)
            return false;

        if (Fields.Count != paragraphStructBuilder.Fields.Count)
            return false;

        for (int i = 0; i < Fields.Count; i++)
            if (Fields[i] != paragraphStructBuilder.Fields[i])
                return false;

        return Type == paragraphStructBuilder.Type
               && ColumnCount == paragraphStructBuilder.ColumnCount;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();
}
using System.Diagnostics.CodeAnalysis;
using Kook.Utils;

namespace Kook;

/// <summary>
///     An element builder to build a <see cref="ButtonElement"/>.
/// </summary>
public sealed class ButtonElementBuilder : IElementBuilder, IEquatable<ButtonElementBuilder>
{
    private IElementBuilder? _text;

    /// <summary>
    ///     Gets the maximum button text length allowed by Kook.
    /// </summary>
    public const int MaxButtonTextLength = 40;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ButtonElementBuilder"/> class.
    /// </summary>
    public ButtonElementBuilder()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ButtonElementBuilder"/> class.
    /// </summary>
    /// <param name="text"> The text of the button.</param>
    /// <param name="theme"> The theme of the button.</param>
    /// <param name="value"> The value of the button.</param>
    /// <param name="click"> The type of the click event.</param>
    public ButtonElementBuilder(string text, ButtonTheme theme = ButtonTheme.Primary,
        string? value = null,
        ButtonClickEventType click = ButtonClickEventType.None)
    {
        Text = new PlainTextElementBuilder(text);
        Theme = theme;
        Value = value;
        Click = click;
    }

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
    public string? Value { get; set; }

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
    public IElementBuilder? Text
    {
        get => _text;
        set
        {
            string? text = value switch
            {
                PlainTextElementBuilder plainText => plainText.Content,
                KMarkdownElementBuilder kMarkdown => kMarkdown.Content,
                _ => throw new ArgumentException(
                    $"The text of a button must be a {nameof(PlainTextElementBuilder)} or a {nameof(KMarkdownElementBuilder)}.",
                    nameof(value))
            };
            if (text is null || string.IsNullOrEmpty(text))
                throw new ArgumentException("The content cannot be null or empty.", nameof(value));

            if (text.Length > MaxButtonTextLength)
            {
                throw new ArgumentException(
                    $"The length of button text must be less than or equal to {MaxButtonTextLength}.",
                    nameof(value));
            }

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
    public ButtonElementBuilder WithText<T>(Action<T> action)
        where T : IElementBuilder, new()
    {
        T text = new();
        action.Invoke(text);
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
            false => new PlainTextElementBuilder(text),
            true => new KMarkdownElementBuilder(text)
        };
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="ButtonElementBuilder"/> into a <see cref="ButtonElement"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ButtonElement"/> represents the built element object.
    /// </returns>
    [MemberNotNull(nameof(Text))]
    public ButtonElement Build()
    {
        if (Click == ButtonClickEventType.Link && (Value is null || !UrlValidation.Validate(Value)))
            throw new ArgumentException("The value of a button with a link event type cannot be null or empty.", nameof(Value));
        if (Text is null or PlainTextElementBuilder { Content: null or { Length: 0 } } or KMarkdownElementBuilder { Content: null or { Length: 0 } })
            throw new ArgumentException("The text of a button cannot be null or empty.", nameof(Text));

        return new ButtonElement(Theme, Value, Click, Text.Build());
    }

    /// <inheritdoc />
    [MemberNotNull(nameof(Text))]
    IElement IElementBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="ButtonElementBuilder"/> is equal to the current <see cref="ButtonElementBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ButtonElementBuilder"/> is equal to the current <see cref="ButtonElementBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(ButtonElementBuilder left, ButtonElementBuilder right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ButtonElementBuilder"/> is not equal to the current <see cref="ButtonElementBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ButtonElementBuilder"/> is not equal to the current <see cref="ButtonElementBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(ButtonElementBuilder left, ButtonElementBuilder right)
        => !(left == right);

    /// <summary>
    ///     Determines whether the specified <see cref="object"/> is equal to the current <see cref="ButtonElementBuilder"/>.
    /// </summary>
    /// <param name="obj"> The <see cref="object"/> to compare with the current <see cref="ButtonElementBuilder"/>. </param>
    /// <returns> <c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="ButtonElementBuilder"/>; otherwise, <c>false</c>. </returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is ButtonElementBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ButtonElementBuilder"/> is equal to the current <see cref="ButtonElementBuilder"/>.</summary>
    /// <param name="buttonElementBuilder">The <see cref="ButtonElementBuilder"/> to compare with the current <see cref="ButtonElementBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ButtonElementBuilder"/> is equal to the current <see cref="ButtonElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] ButtonElementBuilder? buttonElementBuilder)
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

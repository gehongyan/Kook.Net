namespace Kook;

/// <summary>
///     Represents a header module builder for creating a <see cref="HeaderModule"/>.
/// </summary>
public class HeaderModuleBuilder : IModuleBuilder, IEquatable<HeaderModuleBuilder>
{
    private PlainTextElementBuilder _text;

    /// <summary>
    ///     Gets the maximum content length for header allowed by Kook.
    /// </summary>
    public const int MaxTitleContentLength = 100;

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Header;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HeaderModuleBuilder"/> class.
    /// </summary>
    public HeaderModuleBuilder()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HeaderModuleBuilder"/> class.
    /// </summary>
    /// <param name="text"> The text to be set for the header. </param>
    public HeaderModuleBuilder(PlainTextElementBuilder text) => WithText(text);

    /// <summary>
    ///     Initializes a new instance of the <see cref="HeaderModuleBuilder"/> class.
    /// </summary>
    /// <param name="text"> The text to be set for the header. </param>
    public HeaderModuleBuilder(string text) => WithText(text);

    /// <summary>
    ///     Gets or sets the text of the header.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="value"/> is greater than <see cref="MaxTitleContentLength"/>.",
    /// </exception>
    /// <returns>
    ///     A <see cref="PlainTextElementBuilder"/> representing the text of the header.
    /// </returns>
    public PlainTextElementBuilder Text
    {
        get => _text;
        set
        {
            if (value is not null && value.Content is null)
                throw new ArgumentException(
                    "Header content cannot be null.",
                    nameof(Text));

            if (value is not null && value.Content.Length > MaxTitleContentLength)
                throw new ArgumentException(
                    $"Header content length must be less than or equal to {MaxTitleContentLength}.",
                    nameof(Text));

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
    /// <param name="text"> The text to be set for the header. </param>
    /// <returns> The current builder. </returns>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="text"/> is greater than <see cref="MaxTitleContentLength"/>.",
    /// </exception>
    public HeaderModuleBuilder WithText(string text)
    {
        Text = new PlainTextElementBuilder().WithContent(text);
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

    /// <summary>
    ///     Initialized a new instance of the <see cref="HeaderModuleBuilder"/> class
    ///     with the specified <paramref name="text"/>.
    /// </summary>
    /// <param name="text">
    ///     The text to be set for the header.
    /// </param>
    /// <returns>
    ///     An <see cref="HeaderModuleBuilder"/> object that is initialized with the specified <paramref name="text"/>.
    /// </returns>
    public static implicit operator HeaderModuleBuilder(string text)
        => new HeaderModuleBuilder().WithText(b => b.WithContent(text));

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="HeaderModuleBuilder"/> is equal to the current <see cref="HeaderModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="HeaderModuleBuilder"/> is equal to the current <see cref="HeaderModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(HeaderModuleBuilder left, HeaderModuleBuilder right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="HeaderModuleBuilder"/> is not equal to the current <see cref="HeaderModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="HeaderModuleBuilder"/> is not equal to the current <see cref="HeaderModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(HeaderModuleBuilder left, HeaderModuleBuilder right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="HeaderModuleBuilder"/> is equal to the current <see cref="HeaderModuleBuilder"/>.</summary>
    /// <remarks>If the object passes is an <see cref="HeaderModuleBuilder"/>, <see cref="Equals(HeaderModuleBuilder)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="HeaderModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="HeaderModuleBuilder"/> is equal to the current <see cref="HeaderModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is HeaderModuleBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="HeaderModuleBuilder"/> is equal to the current <see cref="HeaderModuleBuilder"/>.</summary>
    /// <param name="headerModuleBuilder">The <see cref="HeaderModuleBuilder"/> to compare with the current <see cref="HeaderModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="HeaderModuleBuilder"/> is equal to the current <see cref="HeaderModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(HeaderModuleBuilder headerModuleBuilder)
    {
        if (headerModuleBuilder is null) return false;

        return Type == headerModuleBuilder.Type
            && Text == headerModuleBuilder.Text;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();
}

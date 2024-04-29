using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Represents a section module builder for creating a <see cref="SectionModule"/>.
/// </summary>
public class SectionModuleBuilder : IModuleBuilder, IEquatable<SectionModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Section;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SectionModuleBuilder"/> class.
    /// </summary>
    public SectionModuleBuilder()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SectionModuleBuilder"/> class.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     The <paramref name="text"/> is not any form of text element,
    ///     including <see cref="PlainTextElementBuilder"/>, <see cref="KMarkdownElementBuilder"/>,
    ///     and <see cref="ParagraphStructBuilder"/>; or the <paramref name="accessory"/> is neither
    ///     an <see cref="ImageElementBuilder"/> nor <see cref="ButtonElementBuilder"/>.
    /// </exception>
    public SectionModuleBuilder(IElementBuilder? text,
        SectionAccessoryMode? mode = null, IElementBuilder? accessory = null)
    {
        Text = text;
        Mode = mode;
        Accessory = accessory;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SectionModuleBuilder"/> class.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     The <paramref name="text"/> is not any form of text element,
    ///     including <see cref="PlainTextElementBuilder"/>, <see cref="KMarkdownElementBuilder"/>,
    ///     and <see cref="ParagraphStructBuilder"/>; or the <paramref name="accessory"/> is neither
    ///     an <see cref="ImageElementBuilder"/> nor <see cref="ButtonElementBuilder"/>.
    /// </exception>
    public SectionModuleBuilder(string? text, bool isKMarkdown = false,
        SectionAccessoryMode? mode = null, IElementBuilder? accessory = null)
    {
        WithText(text, isKMarkdown);
        Mode = mode;
        Accessory = accessory;
    }

    /// <summary>
    ///     Gets or sets how the <see cref="Accessory"/> is positioned relative to the <see cref="Text"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="SectionAccessoryMode"/> representing
    ///     how the <see cref="Accessory"/> is positioned relative to the <see cref="Text"/>.
    /// </returns>
    public SectionAccessoryMode? Mode { get; set; }

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
    public IElementBuilder? Text { get; set; }

    /// <summary>
    ///     Gets or sets the accessory of the section.
    /// </summary>
    /// <returns>
    ///     An <see cref="IElementBuilder"/> representing the accessory of the section.
    /// </returns>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="value"/> is neither an <see cref="ImageElementBuilder"/>
    ///     nor <see cref="ButtonElementBuilder"/>.
    /// </exception>
    public IElementBuilder? Accessory { get; set; }

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
    public SectionModuleBuilder WithText(string? text, bool isKMarkdown = false)
    {
        Text = isKMarkdown switch
        {
            false => new PlainTextElementBuilder(text),
            true => new KMarkdownElementBuilder(text)
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
    ///     Sets the text of the section.
    /// </summary>
    /// <param name="action">
    ///     The action to set the text of the section.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SectionModuleBuilder WithText<T>(Action<T>? action = null)
        where T : IElementBuilder, new()
    {
        T text = new();
        action?.Invoke(text);
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
    ///     Sets the accessory of the section.
    /// </summary>
    /// <param name="action">
    ///     The action to set the accessory of the section.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SectionModuleBuilder WithAccessory<T>(Action<T>? action = null)
        where T : IElementBuilder, new()
    {
        T accessory = new();
        action?.Invoke(accessory);
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
    /// <exception cref="ArgumentException">
    ///     The <see cref="Text"/> is not any form of text element,
    ///     including <see cref="PlainTextElementBuilder"/>, <see cref="KMarkdownElementBuilder"/>,
    ///     and <see cref="ParagraphStructBuilder"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     The <see cref="Accessory"/> is neither an <see cref="ImageElementBuilder"/>
    ///     nor <see cref="ButtonElementBuilder"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     The <see cref="ButtonElement"/> was not positioned to the left of the <see cref="Text"/>,
    ///     which is not allowed.
    /// </exception>
    public SectionModule Build()
    {
        if (Text is not (null or PlainTextElementBuilder or KMarkdownElementBuilder or ParagraphStructBuilder))
            throw new ArgumentException(
                "Section text must be a PlainText element, a KMarkdown element or a Paragraph struct if set.",
                nameof(Text));

        if (Accessory is not (null or ImageElementBuilder or ButtonElementBuilder))
            throw new ArgumentException(
                "Section accessory must be an Image element or a Button element if set.",
                nameof(Accessory));

        if (Mode != SectionAccessoryMode.Right && Accessory is ButtonElementBuilder)
            throw new InvalidOperationException("Button must be placed on the right");

        return new SectionModule(Mode, Text?.Build(), Accessory?.Build());
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="SectionModuleBuilder"/> is equal to the current <see cref="SectionModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="SectionModuleBuilder"/> is equal to the current <see cref="SectionModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(SectionModuleBuilder? left, SectionModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="SectionModuleBuilder"/> is not equal to the current <see cref="SectionModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="SectionModuleBuilder"/> is not equal to the current <see cref="SectionModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(SectionModuleBuilder? left, SectionModuleBuilder? right) =>
        !(left == right);

    /// <summary>Determines whether the specified <see cref="SectionModuleBuilder"/> is equal to the current <see cref="SectionModuleBuilder"/>.</summary>
    /// <remarks>If the object passes is an <see cref="SectionModuleBuilder"/>, <see cref="Equals(SectionModuleBuilder)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="SectionModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="SectionModuleBuilder"/> is equal to the current <see cref="SectionModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is SectionModuleBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="SectionModuleBuilder"/> is equal to the current <see cref="SectionModuleBuilder"/>.</summary>
    /// <param name="sectionModuleBuilder">The <see cref="SectionModuleBuilder"/> to compare with the current <see cref="SectionModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="SectionModuleBuilder"/> is equal to the current <see cref="SectionModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] SectionModuleBuilder? sectionModuleBuilder)
    {
        if (sectionModuleBuilder is null) return false;

        return Type == sectionModuleBuilder.Type
            && Mode == sectionModuleBuilder.Mode
            && Text == sectionModuleBuilder.Text
            && Accessory == sectionModuleBuilder.Accessory;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IModuleBuilder>.Equals([NotNullWhen(true)] IModuleBuilder? moduleBuilder) =>
        Equals(moduleBuilder as SectionModuleBuilder);
}

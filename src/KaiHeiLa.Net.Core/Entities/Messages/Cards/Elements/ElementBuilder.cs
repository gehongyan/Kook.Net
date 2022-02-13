using System.Collections.Immutable;
using KaiHeiLa.Utils;

namespace KaiHeiLa;

public class PlainTextElementBuilder : IElementBuilder
{
    private string _content;

    /// <summary>
    ///     Gets the maximum plain text length allowed by KaiHeiLa.
    /// </summary>
    public const int MaxPlainTextLength = 2000;

    public ElementType Type => ElementType.PlainText;
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

    public bool Emoji { get; set; } = true;

    public PlainTextElementBuilder WithContent(string content)
    {
        Content = content;
        return this;
    }
    public PlainTextElementBuilder WithEmoji(bool emoji)
    {
        Emoji = emoji;
        return this;
    }

    public PlainTextElement Build() 
        => new PlainTextElement(Content, Emoji);

    public static implicit operator PlainTextElementBuilder(string content) => 
        new PlainTextElementBuilder().WithContent(content);
    
    IElement IElementBuilder.Build() => Build();
}

public class KMarkdownElementBuilder : IElementBuilder
{
    private string _content;

    /// <summary>
    ///     Gets the maximum kmarkdown length allowed by KaiHeiLa.
    /// </summary>
    public const int MaxKMarkdownLength = 5000;

    public ElementType Type => ElementType.KMarkdown;
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

    public KMarkdownElementBuilder WithContent(string content)
    {
        Content = content;
        return this;
    }

    public KMarkdownElement Build() 
        => new KMarkdownElement(Content);

    public static implicit operator KMarkdownElementBuilder(string content) => 
        new KMarkdownElementBuilder().WithContent(content);
    
    IElement IElementBuilder.Build() => Build();
}

public class ImageElementBuilder : IElementBuilder
{
    private string _alternative;

    /// <summary>
    ///     Gets the maximum image alternative length allowed by KaiHeiLa.
    /// </summary>
    public const int MaxAlternativeLength = 20;
    
    public ElementType Type => ElementType.Image;
    public string Source { get; set; }

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

    public ImageSize? Size { get; set; }
    public bool? Circle { get; set; }
    
    public ImageElementBuilder WithSource(string source)
    {
        Source = source;
        return this;
    }
    public ImageElementBuilder WithAlternative(string alternative)
    {
        Alternative = alternative;
        return this;
    }
    public ImageElementBuilder WithSize(ImageSize size)
    {
        Size = size;
        return this;
    }
    public ImageElementBuilder WithCircle(bool circle)
    {
        Circle = circle;
        return this;
    }
    
    public ImageElement Build()
    {
        if (!string.IsNullOrEmpty(Source))
            UrlValidation.Validate(Source);
        return new ImageElement(Source, Alternative, Size, Circle);
    }
    
    public static implicit operator ImageElementBuilder(string source) => new ImageElementBuilder()
        .WithSource(source);
    
    IElement IElementBuilder.Build() => Build();
}

public class ButtonElementBuilder : IElementBuilder
{
    public ElementType Type => ElementType.Button;
    
    public ButtonTheme Theme { get; set; }
    public string Value { get; set; }
    public ButtonClickEventType Click { get; set; }
    public IElementBuilder Text { get; set; }
    
    public ButtonElementBuilder WithTheme(ButtonTheme theme)
    {
        Theme = theme;
        return this;
    }
    public ButtonElementBuilder WithValue(string value)
    {
        Value = value;
        return this;
    }
    public ButtonElementBuilder WithClick(ButtonClickEventType click)
    {
        Click = click;
        return this;
    }
    public ButtonElementBuilder WithText(PlainTextElementBuilder text)
    {
        Text = text;
        return this;
    }
    public ButtonElementBuilder WithText(Action<PlainTextElementBuilder> action)
    {
        PlainTextElementBuilder text = new();
        action(text);
        Text = text;
        return this;
    }
    public ButtonElementBuilder WithText(KMarkdownElementBuilder text)
    {
        Text = text;
        return this;
    }
    public ButtonElementBuilder WithText(Action<KMarkdownElementBuilder> action)
    {
        KMarkdownElementBuilder text = new();
        action(text);
        Text = text;
        return this;
    }
    public ButtonElementBuilder WithText(string text, bool isKMarkdown = false)
    {
        Text = isKMarkdown switch
        {
            false => new PlainTextElementBuilder().WithContent(text),
            true => new KMarkdownElementBuilder().WithContent(text)
        };
        return this;
    }
    
    public ButtonElement Build() 
        => new ButtonElement(Theme, Value, Click, Text?.Build());
    
    IElement IElementBuilder.Build() => Build();
}

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

    public ParagraphStructBuilder()
    {
        Fields = new List<IElementBuilder>();
    }
    
    public ElementType Type => ElementType.Paragraph;
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
            _fields = value;
        }
    }

    public ParagraphStructBuilder WithColumnCount(int count)
    {
        ColumnCount = count;
        return this;
    }
    public ParagraphStructBuilder AddField(PlainTextElementBuilder field)
    {
        if (Fields.Count >= MaxFieldCount) throw new ArgumentException(
            message: $"Field count must be less than or equal to {MaxFieldCount}.",
            paramName: nameof(field));
        Fields.Add(field);
        return this;
    }
    public ParagraphStructBuilder AddField(Action<PlainTextElementBuilder> action)
    {
        PlainTextElementBuilder field = new();
        action(field);
        AddField(field);
        return this;
    }
    public ParagraphStructBuilder AddField(KMarkdownElementBuilder field)
    {
        if (Fields.Count >= MaxFieldCount) throw new ArgumentException(
            message: $"Field count must be less than or equal to {MaxFieldCount}.",
            paramName: nameof(field));
        Fields.Add(field);
        return this;
    }
    public ParagraphStructBuilder AddField(Action<KMarkdownElementBuilder> action)
    {
        KMarkdownElementBuilder field = new();
        action(field);
        AddField(field);
        return this;
    }

    public ParagraphStruct Build() => 
        new(ColumnCount, Fields.Select(f => f.Build()).ToImmutableArray());
    
    IElement IElementBuilder.Build() => Build();
}
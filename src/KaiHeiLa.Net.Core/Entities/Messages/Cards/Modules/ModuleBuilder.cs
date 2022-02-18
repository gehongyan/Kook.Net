using System.Collections.Immutable;
using KaiHeiLa.Utils;

namespace KaiHeiLa;

public class HeaderModuleBuilder : IModuleBuilder
{
    private PlainTextElementBuilder _text;

    /// <summary>
    ///     Gets the maximum content length for header allowed by KaiHeiLa.
    /// </summary>
    public const int MaxTitleContentLength = 100;
    
    public ModuleType Type => ModuleType.Header;
    public PlainTextElementBuilder Text
    {
        get => _text;
        set
        {
            if (value.Content.Length > MaxTitleContentLength) throw new ArgumentException(
                message: $"Header content length must be less than or equal to {MaxTitleContentLength}.", 
                paramName: nameof(Text));
            _text = value;
        }
    }

    public HeaderModuleBuilder WithText(PlainTextElementBuilder text)
    {
        Text = text;
        return this;
    }
    public HeaderModuleBuilder WithText(Action<PlainTextElementBuilder> action)
    {
        PlainTextElementBuilder text = new();
        action(text);
        Text = text;
        return this;
    }

    public HeaderModule Build() => new(Text?.Build());
    
    IModule IModuleBuilder.Build() => Build();
}

public class SectionModuleBuilder : IModuleBuilder
{
    public ModuleType Type => ModuleType.Section;
    public SectionAccessoryMode Mode { get; set; }
    public IElementBuilder Text { get; set; }
    public IElementBuilder Accessory { get; set; }

    public SectionModuleBuilder WithText(PlainTextElementBuilder text)
    {
        Text = text;
        return this;
    }
    public SectionModuleBuilder WithText(KMarkdownElementBuilder text)
    {
        Text = text;
        return this;
    }
    public SectionModuleBuilder WithText(string text, bool isKMarkdown = false)
    {
        Text = isKMarkdown switch
        {
            false => new PlainTextElementBuilder().WithContent(text),
            true => new KMarkdownElementBuilder().WithContent(text)
        };
        return this;
    }
    public SectionModuleBuilder WithText(ParagraphStructBuilder text)
    {
        Text = text;
        return this;
    }
    public SectionModuleBuilder WithAccessory(ImageElementBuilder accessory)
    {
        Accessory = accessory;
        return this;
    }
    public SectionModuleBuilder WithAccessory(ButtonElementBuilder accessory)
    {
        Accessory = accessory;
        return this;
    }
    public SectionModuleBuilder WithMode(SectionAccessoryMode mode)
    {
        Mode = mode;
        return this;
    }

    public SectionModule Build()
    {
        if (Mode == SectionAccessoryMode.Left && Accessory is ButtonElementBuilder)
            throw new InvalidOperationException(message: "Button must be placed on the right");
        return new SectionModule(Mode, Text?.Build(), Accessory?.Build());
    }
    
    IModule IModuleBuilder.Build() => Build();
}

public class ImageGroupModuleBuilder : IModuleBuilder
{
    private List<ImageElementBuilder> _elements;
    
    /// <summary>
    ///     Returns the maximum number of elements allowed by KaiHeiLa.
    /// </summary>
    public const int MaxElementCount = 9;

    public ImageGroupModuleBuilder()
    {
        Elements = new List<ImageElementBuilder>();
    }
    
    public ModuleType Type => ModuleType.ImageGroup;

    public List<ImageElementBuilder> Elements
    {
        get => _elements;
        set
        {
            if (value.Count > MaxElementCount) throw new ArgumentException(
                message: $"Element count must be less than or equal to {MaxElementCount}.",
                paramName: nameof(Elements));
            _elements = value;
        }
    }
    
    public ImageGroupModuleBuilder AddElement(ImageElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount) throw new ArgumentException(
            message: $"Element count must be less than or equal to {MaxElementCount}.",
            paramName: nameof(field));
        Elements.Add(field);
        return this;
    }
    public ImageGroupModuleBuilder AddElement(Action<ImageElementBuilder> action)
    {
        ImageElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    } 

    public ImageGroupModule Build() =>
        new(Elements.Select(e => e.Build()).ToImmutableArray());
    
    IModule IModuleBuilder.Build() => Build();
}

public class ContainerModuleBuilder : IModuleBuilder
{
    private List<ImageElementBuilder> _elements;
    
    /// <summary>
    ///     Returns the maximum number of elements allowed by KaiHeiLa.
    /// </summary>
    public const int MaxElementCount = 9;

    public ContainerModuleBuilder()
    {
        Elements = new List<ImageElementBuilder>();
    }
    
    public ModuleType Type => ModuleType.Container;

    public List<ImageElementBuilder> Elements
    {
        get => _elements;
        set
        {
            if (value.Count > MaxElementCount) throw new ArgumentException(
                message: $"Element count must be less than or equal to {MaxElementCount}.",
                paramName: nameof(Elements));
            _elements = value;
        }
    }
    
    public ContainerModuleBuilder AddElement(ImageElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount) throw new ArgumentException(
            message: $"Element count must be less than or equal to {MaxElementCount}.",
            paramName: nameof(field));
        Elements.Add(field);
        return this;
    }
    public ContainerModuleBuilder AddElement(Action<ImageElementBuilder> action)
    {
        ImageElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    } 

    public ContainerModule Build() =>
        new(Elements.Select(e => e.Build()).ToImmutableArray());
    
    IModule IModuleBuilder.Build() => Build();
}

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
    
    public ModuleType Type => ModuleType.Container;

    public List<ButtonElementBuilder> Elements
    {
        get => _elements;
        set
        {
            if (value.Count > MaxElementCount) throw new ArgumentException(
                message: $"Element count must be less than or equal to {MaxElementCount}.",
                paramName: nameof(Elements));
            _elements = value;
        }
    }
    
    public ActionGroupModuleBuilder AddElement(ButtonElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount) throw new ArgumentException(
            message: $"Element count must be less than or equal to {MaxElementCount}.",
            paramName: nameof(field));
        Elements.Add(field);
        return this;
    }
    public ActionGroupModuleBuilder AddElement(Action<ButtonElementBuilder> action)
    {
        ButtonElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    } 

    public ActionGroupModule Build() =>
        new(Elements.Select(e => e.Build()).ToImmutableArray());
    
    IModule IModuleBuilder.Build() => Build();
}

public class ContextModuleBuilder : IModuleBuilder
{
    private List<IElementBuilder> _elements;

    /// <summary>
    ///     Returns the maximum number of elements allowed by KaiHeiLa.
    /// </summary>
    public const int MaxElementCount = 10;

    public ContextModuleBuilder()
    {
        Elements = new List<IElementBuilder>();
    }
    
    public ModuleType Type => ModuleType.Container;

    public List<IElementBuilder> Elements
    {
        get => _elements;
        set
        {
            if (value.Count > MaxElementCount) throw new ArgumentException(
                message: $"Element count must be less than or equal to {MaxElementCount}.",
                paramName: nameof(Elements));
            _elements = value;
        }
    }
    
    public ContextModuleBuilder AddElement(PlainTextElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount) throw new ArgumentException(
            message: $"Element count must be less than or equal to {MaxElementCount}.",
            paramName: nameof(field));
        Elements.Add(field);
        return this;
    }
    public ContextModuleBuilder AddElement(Action<PlainTextElementBuilder> action)
    {
        PlainTextElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    }
    public ContextModuleBuilder AddElement(KMarkdownElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount) throw new ArgumentException(
            message: $"Element count must be less than or equal to {MaxElementCount}.",
            paramName: nameof(field));
        Elements.Add(field);
        return this;
    }
    public ContextModuleBuilder AddElement(Action<KMarkdownElementBuilder> action)
    {
        KMarkdownElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    }
    public ContextModuleBuilder AddElement(ImageElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount) throw new ArgumentException(
            message: $"Element count must be less than or equal to {MaxElementCount}.",
            paramName: nameof(field));
        Elements.Add(field);
        return this;
    }
    public ContextModuleBuilder AddElement(Action<ImageElementBuilder> action)
    {
        ImageElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    }

    public ContextModule Build() =>
        new(Elements.Select(e => e.Build()).ToImmutableArray());
    
    IModule IModuleBuilder.Build() => Build();
}

public class DividerModuleBuilder : IModuleBuilder
{
    public ModuleType Type => ModuleType.Divider;

    public DividerModule Build() => new();
    
    IModule IModuleBuilder.Build() => Build();
}

public class FileModuleBuilder : IModuleBuilder
{
    public ModuleType Type => ModuleType.File;
    
    public string Source { get; set; }
    public string Title { get; set; }
    
    public FileModuleBuilder WithSource(string source)
    {
        Source = source;
        return this;
    }
    public FileModuleBuilder WithTitle(string title)
    {
        Title = title;
        return this;
    }
    
    public FileModule Build()
    {
        if (!string.IsNullOrEmpty(Source))
            UrlValidation.Validate(Source);
        return new FileModule(Source, Title);
    }
    
    IModule IModuleBuilder.Build() => Build();
}

public class VideoModuleBuilder : IModuleBuilder
{
    public ModuleType Type => ModuleType.Video;
    
    public string Source { get; set; }
    public string Title { get; set; }
    
    public VideoModuleBuilder WithSource(string source)
    {
        Source = source;
        return this;
    }
    public VideoModuleBuilder WithTitle(string title)
    {
        Title = title;
        return this;
    }
    
    public VideoModule Build()
    {
        if (!string.IsNullOrEmpty(Source))
            UrlValidation.Validate(Source);
        return new(Source, Title);
    }
    
    IModule IModuleBuilder.Build() => Build();
}

public class AudioModuleBuilder : IModuleBuilder
{
    public ModuleType Type => ModuleType.Audio;
    
    public string Source { get; set; }
    public string Cover { get; set; }
    public string Title { get; set; }
    
    public AudioModuleBuilder WithSource(string source)
    {
        Source = source;
        return this;
    }
    public AudioModuleBuilder WithCover(string cover)
    {
        Cover = cover;
        return this;
    }
    public AudioModuleBuilder WithTitle(string title)
    {
        Title = title;
        return this;
    }
    
    public AudioModule Build()
    {
        if (!string.IsNullOrEmpty(Source))
            UrlValidation.Validate(Source);
        if (!string.IsNullOrEmpty(Cover))
            UrlValidation.Validate(Cover);
        return new(Source, Title, Cover);
    }
    
    IModule IModuleBuilder.Build() => Build();
}

public class CountdownModuleBuilder : IModuleBuilder
{
    public ModuleType Type => ModuleType.Countdown;
    
    public DateTimeOffset EndTime { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public CountdownMode Mode { get; set; }
    
    public CountdownModuleBuilder WithMode(CountdownMode mode)
    {
        Mode = mode;
        return this;
    }
    public CountdownModuleBuilder WithEndTime(DateTimeOffset endTime)
    {
        EndTime = endTime;
        return this;
    }
    public CountdownModuleBuilder WithStartTime(DateTimeOffset startTime)
    {
        StartTime = startTime;
        return this;
    }
    
    public CountdownModule Build()
    {
        if (Mode != CountdownMode.Second && StartTime is not null) throw new InvalidOperationException(
            "Only when the countdown is second mode can the start time be set.");
        if (EndTime < DateTimeOffset.Now) throw new ArgumentOutOfRangeException(
                message: $"{nameof(EndTime)} must be later than current timestamp.",
                paramName: nameof(EndTime));
        if (StartTime is not null && StartTime < DateTimeOffset.Now) throw new ArgumentOutOfRangeException(
            message: $"{nameof(StartTime)} must be later than current timestamp.",
            paramName: nameof(StartTime));
        if (StartTime is not null && StartTime >= EndTime) throw new ArgumentOutOfRangeException(
            message: $"{nameof(StartTime)} must be later than {nameof(EndTime)}.",
            paramName: nameof(StartTime));
        return new CountdownModule(Mode, EndTime, StartTime);
    }
    
    IModule IModuleBuilder.Build() => Build();
}

public class InviteModuleBuilder : IModuleBuilder
{
    public ModuleType Type => ModuleType.Invite;
    
    public string Code { get; set; }
    
    public InviteModuleBuilder WithCode(string code)
    {
        Code = code;
        return this;
    }

    public InviteModule Build() => new(Code);

    public static implicit operator InviteModuleBuilder(string code) => new InviteModuleBuilder()
        .WithCode(code);
    
    IModule IModuleBuilder.Build() => Build();
}
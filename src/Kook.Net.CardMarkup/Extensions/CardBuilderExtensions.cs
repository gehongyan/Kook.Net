using Kook.CardMarkup.Models;

namespace Kook.CardMarkup.Extensions;

internal static class CardBuilderExtensions
{
    public static IEnumerable<ICard> ToCards(this MarkupElement element)
    {
        List<ICard> cards = [];

        // Unwrap card-message element
        List<MarkupElement> cardElements = element.Children;
        foreach (MarkupElement e in cardElements)
        {
            // Get Modules
            IEnumerable<IModuleBuilder> modules = e.Children[0].Children.Select(ToModule);

            // Create Card
            Color? color = e.Attributes.GetColor();
            CardTheme theme = e.Attributes.GetCardTheme();
            CardSize size = e.Attributes.GetCardSize();

            CardBuilder builder = new CardBuilder()
                .WithTheme(theme)
                .WithSize(size);
            if (color.HasValue)
                builder.WithColor(color.Value);

            foreach (IModuleBuilder module in modules)
                builder.Modules.Add(module);

            cards.Add(builder.Build());
        }

        return cards;
    }

    #region Modules

    private static IModuleBuilder ToModule(this MarkupElement element)
    {
        return element.Name switch
        {
            "header" => element.ToHeaderModule(),
            "section" => element.ToSectionModule(),
            "images" => element.ToImagesModule(),
            "container" => element.ToContainerModule(),
            "actions" => element.ToActionsModule(),
            "context" => element.ToContextModule(),
            "divider" => element.ToDividerModule(),
            "file" => element.ToFileModule(),
            "video" => element.ToVideoModule(),
            "audio" => element.ToAudioModule(),
            "countdown" => element.ToCountdownModule(),
            "invite" => element.ToInviteModule(),
            _ => throw new ArgumentOutOfRangeException(nameof(element.Name))
        };
    }

    private static HeaderModuleBuilder ToHeaderModule(this MarkupElement element)
    {
        PlainTextElementBuilder text = element.Children[0].ToPlainTextElement();
        return new HeaderModuleBuilder(text);
    }

    private static SectionModuleBuilder ToSectionModule(this MarkupElement element)
    {
        SectionAccessoryMode? mode = element.Attributes.GetSectionAccessoryMode();

        MarkupElement textXmlElement = element.Children.First(x => x.Name == "text");
        MarkupElement? accessoryXmlElement = element.Children.Find(x => x.Name == "accessory");

        IElementBuilder text = textXmlElement.Children[0].ToElement();
        IElementBuilder? accessory = accessoryXmlElement?.Children[0].ToElement();

        return new SectionModuleBuilder(text, mode, accessory);
    }

    private static ImageGroupModuleBuilder ToImagesModule(this MarkupElement element)
    {
        List<ImageElementBuilder> images = element.Children
            .Select(ToImageElement)
            .ToList();

        return new ImageGroupModuleBuilder(images);
    }

    private static ContainerModuleBuilder ToContainerModule(this MarkupElement element)
    {
        List<ImageElementBuilder> images = element.Children
            .Select(ToImageElement)
            .ToList();

        return new ContainerModuleBuilder(images);
    }

    private static ActionGroupModuleBuilder ToActionsModule(this MarkupElement element)
    {
        List<ButtonElementBuilder> actions = element.Children
            .Select(ToButtonElement)
            .ToList();

        return new ActionGroupModuleBuilder(actions);
    }

    private static ContextModuleBuilder ToContextModule(this MarkupElement element)
    {
        List<IElementBuilder> elements = element.Children
            .Select(ToElement)
            .ToList();

        return new ContextModuleBuilder(elements);
    }

    // ReSharper disable once UnusedParameter.Local
    private static DividerModuleBuilder ToDividerModule(this MarkupElement element)
    {
        return new DividerModuleBuilder();
    }

    private static FileModuleBuilder ToFileModule(this MarkupElement element)
    {
        string src = element.Attributes.GetString("src");
        string title = element.Attributes.GetString("title", true);

        return new FileModuleBuilder(src, title);
    }

    private static VideoModuleBuilder ToVideoModule(this MarkupElement element)
    {
        string src = element.Attributes.GetString("src");
        string title = element.Attributes.GetString("title", true);

        return new VideoModuleBuilder(src, title);
    }

    private static AudioModuleBuilder ToAudioModule(this MarkupElement element)
    {
        string src = element.Attributes.GetString("src");
        string title = element.Attributes.GetString("title", true);
        string cover = element.Attributes.GetString("cover", true);

        return new AudioModuleBuilder(src, cover, title);
    }

    private static CountdownModuleBuilder ToCountdownModule(this MarkupElement element)
    {
        long? start = element.Attributes.GetLong("start", true);
        long end = element.Attributes.GetLong("end")!.Value;
        CountdownMode mode = element.Attributes.GetCountdownMode();

        DateTimeOffset endDt = DateTimeOffset.FromUnixTimeMilliseconds(end);
        DateTimeOffset? startDt = start.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds(start.Value) : null;

        return new CountdownModuleBuilder(mode, endDt, startDt);
    }

    private static InviteModuleBuilder ToInviteModule(this MarkupElement element)
    {
        string code = element.Attributes.GetString("code");
        return new InviteModuleBuilder(code);
    }

    #endregion

    #region Elements

    private static IElementBuilder ToElement(this MarkupElement element)
    {
        return element.Name switch
        {
            "plain" => element.ToPlainTextElement(),
            "kmarkdown" => element.ToKMarkdownElement(),
            "image" => element.ToImageElement(),
            "button" => element.ToButtonElement(),
            "paragraph" => element.ToParagraphStruct(),
            _ => throw new ArgumentOutOfRangeException(nameof(element.Name))
        };
    }

    private static PlainTextElementBuilder ToPlainTextElement(this MarkupElement element)
    {
        bool emoji = element.Attributes.GetBoolean("emoji", true);
        return new PlainTextElementBuilder(element.Text?.ParseText(), emoji);
    }

    private static KMarkdownElementBuilder ToKMarkdownElement(this MarkupElement element)
    {
        return new KMarkdownElementBuilder(element.Text?.ParseText());
    }

    private static ImageElementBuilder ToImageElement(this MarkupElement element)
    {
        string src = element.Attributes.GetString("src");
        string alt = element.Attributes.GetString("alt", true);
        bool circle = element.Attributes.GetBoolean("circle", false);
        ImageSize size = element.Attributes.GetImageSize();

        return new ImageElementBuilder(src, alt, size, circle);
    }

    private static ButtonElementBuilder ToButtonElement(this MarkupElement element)
    {
        IElementBuilder textElement = element.Children[0].ToElement();
        ButtonTheme theme = element.Attributes.GetButtonTheme();
        ButtonClickEventType click = element.Attributes.GetButtonClickEventType();
        string value = element.Attributes.GetString("value", true);

        return new ButtonElementBuilder { Text = textElement, Theme = theme, Click = click, Value = value };
    }

    #endregion

    #region Structure

    private static ParagraphStructBuilder ToParagraphStruct(this MarkupElement element)
    {
        int cols = element.Attributes.GetInt("cols", 1);
        List<IElementBuilder> elements = element.Children
            .Select(ToElement)
            .ToList();

        return new ParagraphStructBuilder(cols, elements);
    }

    #endregion

    #region Helpers

    private static string ParseText(this string text)
    {
        IEnumerable<string> multiLine = text
            .Split(["\r\n", "\r", "\n"], StringSplitOptions.None)
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => x.Trim());
        return string.Join("\n", multiLine);
    }

    #endregion
}

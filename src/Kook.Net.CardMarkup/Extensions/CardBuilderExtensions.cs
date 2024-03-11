using Kook.CardMarkup.Models;

namespace Kook.CardMarkup.Extensions;

internal static class CardBuilderExtensions
{
    public static IEnumerable<ICard> ToCards(this MarkupElement element)
    {
        var cards = new List<ICard>();

        // Unwrap card-message element
        var cardElements = element.Children;
        foreach (var e in cardElements)
        {
            // Get Modules
            var modules = e.Children[0].Children.Select(ToModule);

            // Create Card
            var color = e.Attributes.GetColor();
            var theme = e.Attributes.GetCardTheme();
            var size = e.Attributes.GetCardSize();

            var builder = new CardBuilder()
                .WithTheme(theme)
                .WithSize(size);
            if (color.HasValue)
            {
                builder.WithColor(color.Value);
            }

            builder.Modules.AddRange(modules);

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
        var text = element.Children[0].ToPlainTextElement();
        return new HeaderModuleBuilder(text);
    }

    private static SectionModuleBuilder ToSectionModule(this MarkupElement element)
    {
        var mode = element.Attributes.GetSectionAccessoryMode();

        var textXmlElement = element.Children.First(x => x.Name == "text");
        var accessoryXmlElement = element.Children.FirstOrDefault(x => x.Name == "accessory");

        var text = textXmlElement.Children[0].ToElement();
        var accessory = accessoryXmlElement?.Children[0].ToElement();

        return new SectionModuleBuilder(text, mode, accessory);
    }

    private static ImageGroupModuleBuilder ToImagesModule(this MarkupElement element)
    {
        var images = element.Children
            .Select(ToImageElement)
            .ToList();

        return new ImageGroupModuleBuilder(images);
    }

    private static ContainerModuleBuilder ToContainerModule(this MarkupElement element)
    {
        var images = element.Children
            .Select(ToImageElement)
            .ToList();

        return new ContainerModuleBuilder(images);
    }

    private static ActionGroupModuleBuilder ToActionsModule(this MarkupElement element)
    {
        var actions = element.Children
            .Select(ToButtonElement)
            .ToList();

        return new ActionGroupModuleBuilder(actions);
    }

    private static ContextModuleBuilder ToContextModule(this MarkupElement element)
    {
        var elements = element.Children
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
        var mode = element.Attributes.GetCountdownMode();

        var endDt = DateTimeOffset.FromUnixTimeMilliseconds(end);
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
        return new PlainTextElementBuilder(element.Text.ParseText(), emoji);
    }

    private static KMarkdownElementBuilder ToKMarkdownElement(this MarkupElement element)
    {
        return new KMarkdownElementBuilder(element.Text.ParseText());
    }

    private static ImageElementBuilder ToImageElement(this MarkupElement element)
    {
        string src = element.Attributes.GetString("src");
        string alt = element.Attributes.GetString("alt", true);
        bool circle = element.Attributes.GetBoolean("circle", false);
        var size = element.Attributes.GetImageSize();

        return new ImageElementBuilder(src, alt, size, circle);
    }

    private static ButtonElementBuilder ToButtonElement(this MarkupElement element)
    {
        var textElement = element.Children[0].ToElement();
        var theme = element.Attributes.GetButtonTheme();
        var click = element.Attributes.GetButtonClickEventType();
        string value = element.Attributes.GetString("value", true);

        return new ButtonElementBuilder { Text = textElement, Theme = theme, Click = click, Value = value };
    }

    #endregion

    #region Structure

    private static ParagraphStructBuilder ToParagraphStruct(this MarkupElement element)
    {
        int cols = element.Attributes.GetInt("cols", 1);
        var elements = element.Children
            .Select(ToElement)
            .ToList();

        return new ParagraphStructBuilder(cols, elements);
    }

    #endregion

    #region Helpers

    private static string ParseText(this string text)
    {
        var multiLine = text
            .Split(["\r\n", "\r", "\n"], StringSplitOptions.None)
            .Where(x => string.IsNullOrEmpty(x) is false)
            .Select(x => x.Trim());
        return string.Join("\n", multiLine);
    }

    #endregion
}

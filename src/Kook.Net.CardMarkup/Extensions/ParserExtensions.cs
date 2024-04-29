namespace Kook.CardMarkup.Extensions;

internal static class ParserExtensions
{
    public static CardTheme GetCardTheme(this Dictionary<string, string> dictionary)
    {
        string theme = dictionary.GetValueOrDefault("theme", "primary");
        return theme switch
        {
            "primary" => CardTheme.Primary,
            "success" => CardTheme.Success,
            "warning" => CardTheme.Warning,
            "danger" => CardTheme.Danger,
            "info" => CardTheme.Info,
            "secondary" => CardTheme.Secondary,
            "none" => CardTheme.None,
            _ => CardTheme.Primary
        };
    }

    public static CardSize GetCardSize(this Dictionary<string, string> dictionary)
    {
        string theme = dictionary.GetValueOrDefault("size", "large");
        return theme switch
        {
            "small" => CardSize.Small,
            "large" => CardSize.Large,
            _ => CardSize.Large
        };
    }

    public static Color? GetColor(this Dictionary<string, string> dictionary)
    {
        string color = dictionary.GetValueOrDefault("color", string.Empty);
        if (color.Length != 7 || color[0] != '#')
        {
            return null;
        }

        byte r = Convert.ToByte(color.Substring(1, 2), 16);
        byte g = Convert.ToByte(color.Substring(3, 2), 16);
        byte b = Convert.ToByte(color.Substring(5, 2), 16);
        return new Color(r, g, b);
    }

    public static ImageSize GetImageSize(this Dictionary<string, string> dictionary)
    {
        string theme = dictionary.GetValueOrDefault("size", "large");
        return theme switch
        {
            "small" => ImageSize.Small,
            "large" => ImageSize.Large,
            _ => ImageSize.Large
        };
    }

    public static ButtonTheme GetButtonTheme(this Dictionary<string, string> dictionary)
    {
        string theme = dictionary.GetValueOrDefault("theme", "primary");
        return theme switch
        {
            "primary" => ButtonTheme.Primary,
            "success" => ButtonTheme.Success,
            "warning" => ButtonTheme.Warning,
            "danger" => ButtonTheme.Danger,
            "info" => ButtonTheme.Info,
            "secondary" => ButtonTheme.Secondary,
            _ => ButtonTheme.Primary
        };
    }

    public static ButtonClickEventType GetButtonClickEventType(this Dictionary<string, string> dictionary)
    {
        string theme = dictionary.GetValueOrDefault("click", "none");
        return theme switch
        {
            "none" => ButtonClickEventType.None,
            "link" => ButtonClickEventType.Link,
            "return-val" => ButtonClickEventType.ReturnValue,
            _ => ButtonClickEventType.None
        };
    }

    public static SectionAccessoryMode? GetSectionAccessoryMode(this Dictionary<string, string> dictionary)
    {
        string theme = dictionary.GetValueOrDefault("mode", "unspecified");
        return theme switch
        {
            "left" => SectionAccessoryMode.Left,
            "right" => SectionAccessoryMode.Right,
            _ => null
        };
    }

    public static CountdownMode GetCountdownMode(this Dictionary<string, string> dictionary)
    {
        string theme = dictionary.GetValueOrDefault("mode", "second");
        return theme switch
        {
            "second" => CountdownMode.Second,
            "hour" => CountdownMode.Hour,
            "day" => CountdownMode.Day,
            _ => CountdownMode.Second
        };
    }

    public static bool GetBoolean(this Dictionary<string, string> dictionary, string key, bool defaultValue)
    {
        string value = dictionary.GetValueOrDefault(key, string.Empty);
        return value switch
        {
            "true" => true,
            "false" => false,
            _ => defaultValue
        };
    }

    public static int GetInt(this Dictionary<string, string> dictionary, string key, int defaultValue)
    {
        string value = dictionary.GetValueOrDefault(key, string.Empty);

        bool parsed = int.TryParse(value, out int result);

        if (parsed)
        {
            return result;
        }

        return defaultValue;
    }

    public static long? GetLong(this Dictionary<string, string> dictionary, string key, bool nullable = false)
    {
        string value = dictionary.GetValueOrDefault(key, string.Empty);

        bool parsed = long.TryParse(value, out long result);

        if (parsed)
        {
            return result;
        }

        if (nullable)
        {
            return null;
        }

        throw new ArgumentNullException(nameof(result), "The value is not a valid Int64");
    }

    public static string GetString(this Dictionary<string, string> dictionary, string key, bool allowEmpty = false)
    {
        string value = dictionary.GetValueOrDefault(key, string.Empty);

        if (string.IsNullOrEmpty(value))
        {
            if (allowEmpty)
            {
                return string.Empty;
            }

            throw new ArgumentNullException(nameof(value), "The string value is empty");
        }

        return value;
    }
}

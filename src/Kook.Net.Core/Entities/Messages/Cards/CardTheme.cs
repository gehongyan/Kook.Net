namespace Kook;

/// <summary>
///     Specifies the theme of the card.
/// </summary>
public enum CardTheme : uint
{
    /// <summary>
    ///     The card shows like a regular message.
    /// </summary>
    Primary,

    /// <summary>
    ///     The card shows like a success message.
    /// </summary>
    Success,

    /// <summary>
    ///     The card shows like a warning message.
    /// </summary>
    Warning,

    /// <summary>
    ///     The card shows like an error message.
    /// </summary>
    Danger,

    /// <summary>
    ///     The card shows like an info message.
    /// </summary>
    Info,

    /// <summary>
    ///     The card shows like a light message.
    /// </summary>
    Secondary,

    /// <summary>
    ///     The card shows like a dark message.
    /// </summary>
    None
}

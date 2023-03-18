namespace Kook;

/// <summary>
///     Represents text themes used in <see cref="Format.Colorize"/>.
/// </summary>
/// <remarks>
///     <note type="warning">
///         The text themes of KMarkdown are currently only supported in card messages.
///         KMarkdown messages theming is not supported yet.
///     </note>
///     <note type="warning">
///         The color of the text themes vary on different platforms. It is recommended to
///         test your card messages on different platforms to ensure that the text themes
///         are displayed correctly.
///     </note>
/// </remarks>
public enum TextTheme : ushort
{
    /// <summary>
    ///     Represents a primary theme.
    /// </summary>
    Primary,
    /// <summary>
    ///     Represents a success theme.
    /// </summary>
    Success,
    /// <summary>
    ///     Represents a danger theme.
    /// </summary>
    Danger,
    /// <summary>
    ///     Represents a warning theme.
    /// </summary>
    Warning,
    /// <summary>
    ///     Represents an info theme.
    /// </summary>
    Info,
    /// <summary>
    ///     Represents a secondary theme.
    /// </summary>
    Secondary,
    /// <summary>
    ///     Represents a body theme.
    /// </summary>
    Body,
    /// <summary>
    ///     Represents a tips theme.
    /// </summary>
    Tips,
    /// <summary>
    ///     Represents a pink theme.
    /// </summary>
    Pink,
    /// <summary>
    ///     Represents a purple theme.
    /// </summary>
    Purple
}

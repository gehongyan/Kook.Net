namespace Kook;

/// <summary>
///     Represents the type of a message.
/// </summary>
public enum MessageType
{
    /// <summary>
    ///     Represents that the type of the message contains plain text.
    /// </summary>
    Text = 1,
    /// <summary>
    ///     Represents that the type of the message contains an image.
    /// </summary>
    Image = 2,
    /// <summary>
    ///     Represents that the type of the message contains a video.
    /// </summary>
    Video = 3,
    /// <summary>
    ///     Represents that the type of the message contains a file.
    /// </summary>
    File = 4,
    /// <summary>
    ///     Represents that the type of the message contains a voice message.
    /// </summary>
    Audio = 8,
    /// <summary>
    ///     Represents that the type of the message contains a KMarkdown message.
    /// </summary>
    KMarkdown = 9,
    /// <summary>
    ///     Represents that the type of the message contains cards.
    /// </summary>
    Card = 10,
    /// <summary>
    ///     Represents that the type of the message contains a poke action.
    /// </summary>
    Poke = 12,
    /// <summary>
    ///     Represents that the type of the message originates from KOOK system.
    /// </summary>
    System = 255
}

namespace Kook;

/// <summary>
///     Specifies the source of the Kook message.
/// </summary>
public enum MessageSource
{
    /// <summary>
    ///     The message is sent by the system.
    /// </summary>
    System,
    /// <summary>
    ///     The message is sent by a user.
    /// </summary>
    User,
    /// <summary>
    ///     The message is sent by a bot.
    /// </summary>
    Bot
}
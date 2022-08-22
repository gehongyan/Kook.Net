namespace Kook;

/// <summary>
///     Specifies that how the guild should notify the user.
/// </summary>
public enum NotifyType
{
    /// <summary>
    ///     Notifies the user as the default behavior of the guild.
    /// </summary>
    Default = 0,
    
    /// <summary>
    ///     Notifies the user of all messages.
    /// </summary>
    AcceptAll = 1,
    
    /// <summary>
    ///     Notifies the user of the messages which mention the user.
    /// </summary>
    OnlyMentioned = 2,
    
    /// <summary>
    ///      ]Never notifies the user.
    /// </summary>
    Muted = 3
}
namespace Kook;

/// <summary>
///     Represents a feature of a guild.
/// </summary>
[Flags]
public enum GuildFeature : uint
{
    /// <summary>
    ///     The guild has no features.
    /// </summary>
    None = 0,

    /// <summary>
    ///     The guild is an official KOOK guild.
    /// </summary>
    Official = 1 << 0,

    /// <summary>
    ///     The guild is a partner guild.
    /// </summary>
    Partner = 1 << 1,

    /// <summary>
    ///     The guild is a key account guild.
    /// </summary>
    KeyAccount = 1 << 2,
}

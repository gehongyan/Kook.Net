namespace Kook;

/// <summary>
///     Represents a generic embed.
/// </summary>
/// <seealso cref="IMessage.Embeds"/>
public interface IEmbed
{
    /// <summary>
    ///     Gets the type of this embed.
    /// </summary>
    /// <returns>
    ///     A <see cref="EmbedType"/> that represents the type of this embed.
    /// </returns>
    EmbedType Type { get; }
}

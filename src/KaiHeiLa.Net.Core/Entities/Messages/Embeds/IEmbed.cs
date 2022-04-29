namespace KaiHeiLa;

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
    
    /// <summary>
    ///     Gets the URL of this embed.
    /// </summary>
    /// <returns>
    ///     A <see langword="string"/> that represents the URL of this embed.
    /// </returns>
    string Url { get; }
}
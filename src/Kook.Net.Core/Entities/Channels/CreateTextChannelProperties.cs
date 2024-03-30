namespace Kook;

/// <summary>
///     Provides properties that are used to create an <see cref="ITextChannel"/> with the specified properties.
/// </summary>
/// <seealso cref="IGuild.CreateTextChannelAsync(string, System.Action{CreateTextChannelProperties}, RequestOptions)"/>
public class CreateTextChannelProperties : CreateGuildChannelProperties
{
    /// <summary>
    ///     Gets or sets the category ID for this channel.
    /// </summary>
    /// <remarks>
    ///     Setting this value to a category's identifier will set this channel's parent to the
    ///     specified channel; setting this value to <c>null</c> will leave this channel alone
    ///     from any parents.
    /// </remarks>
    public ulong? CategoryId { get; set; }
}

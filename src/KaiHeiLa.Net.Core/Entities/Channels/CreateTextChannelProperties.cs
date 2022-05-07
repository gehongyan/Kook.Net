namespace KaiHeiLa;

/// <summary>
///     Provides properties that are used to create an <see cref="ITextChannel"/> with the specified properties.
/// </summary>
/// <seealso cref="IGuild.CreateTextChannelAsync(string, System.Action{CreateTextChannelProperties}, RequestOptions)"/>
public class CreateTextChannelProperties : CreateGuildChannelProperties
{
    /// <summary>
    ///     Gets or sets the topic of the channel.
    /// </summary>
    /// <remarks>
    ///     Setting this value to any string other than <c>null</c> or <see cref="string.Empty"/> will set the
    ///     channel topic or description to the desired value.
    /// </remarks>
    public Optional<string> Topic { get; set; }
}
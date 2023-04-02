namespace Kook;

/// <summary>
///     Represents a single image of an intimacy.
/// </summary>
public class IntimacyImage
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IntimacyImage" /> class.
    /// </summary>
    /// <param name="id">The ID of the image of an intimacy. </param>
    /// <param name="url">The URL of the image of an intimacy. </param>
    internal IntimacyImage(uint id, string url)
    {
        Id = id;
        Url = url;
    }

    /// <summary>
    ///     Gets the ID of the image of an intimacy.
    /// </summary>
    /// <returns>
    ///     An <see langword="int"/> representing the ID of the image of an intimacy.
    /// </returns>
    public uint Id { get; }

    /// <summary>
    ///     Gets the URL of the image of an intimacy.
    /// </summary>
    /// <returns>
    ///     A <see langword="string"/> representing the URL of the image of an intimacy.
    /// </returns>
    public string Url { get; }
}

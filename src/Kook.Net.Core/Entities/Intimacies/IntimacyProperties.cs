namespace Kook;

/// <summary>
///     Properties that are used to modify an <see cref="IIntimacy" /> with the specified changes.
/// </summary>
/// <seealso cref="IIntimacy.UpdateAsync"/>
public class IntimacyProperties
{
    /// <summary>
    ///     The social information to be set on the <see cref="IIntimacy" />.
    /// </summary>
    public string SocialInfo { get; set; }

    /// <summary>
    ///     The ID of the image to be updated on the <see cref="IIntimacy" />.
    /// </summary>
    public uint ImageId { get; set; }

    /// <summary>
    ///     The score to be set on the <see cref="IIntimacy" />.
    /// </summary>
    public int Score { get; set; }
}

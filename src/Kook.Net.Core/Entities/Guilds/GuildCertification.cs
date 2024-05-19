namespace Kook;

/// <summary>
///     Represents a guild certification.
/// </summary>
public class GuildCertification
{
    /// <summary>
    ///     Gets the type of the certification.
    /// </summary>
    public GuildCertificationType Type { get; }

    /// <summary>
    ///     Gets the title of the certification.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the picture URL of the certification.
    /// </summary>
    public string? Picture { get; private set; }

    /// <summary>
    ///     Gets the description of the certification.
    /// </summary>
    public string? Description { get; private set; }

    private GuildCertification(GuildCertificationType type)
    {
        Type = type;
    }

    internal static GuildCertification Create(GuildCertificationType type, string title, string picture, string description)
    {
        GuildCertification certification = new(type);
        certification.Update(title, picture, description);
        return certification;
    }

    private void Update(string title, string picture, string description)
    {
        Title = title;
        Picture = string.IsNullOrWhiteSpace(picture) ? null : picture;
        Description = string.IsNullOrWhiteSpace(description) ? null : description;
    }
}

namespace Kook;

/// <summary>
///     表示一个服务器认证信息。
/// </summary>
public class GuildCertification
{
    /// <summary>
    ///     获取认证的类型。
    /// </summary>
    public GuildCertificationType Type { get; }

    /// <summary>
    ///     获取认证的名称。
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    ///     获取认证的图片 URL。
    /// </summary>
    public string? Picture { get; private set; }

    /// <summary>
    ///     获取认证的描述。
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

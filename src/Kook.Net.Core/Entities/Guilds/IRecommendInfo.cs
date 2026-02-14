namespace Kook;

/// <summary>
///     获取一个通用的推荐信息。
/// </summary>
public interface IRecommendInfo
{
    /// <summary>
    ///     获取推荐服务器的 ID。
    /// </summary>
    ulong GuildId { get; }

    /// <summary>
    ///     获取推荐服务器的公开 ID。
    /// </summary>
    uint? OpenId { get; }

    /// <summary>
    ///     获取推荐服务器的默认文字频道 ID。
    /// </summary>
    ulong DefaultChannelId { get; }

    /// <summary>
    ///     获取推荐服务器的名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     获取推荐服务器的图标 URL。
    /// </summary>
    string Icon { get; }

    /// <summary>
    ///     获取推荐服务器的横幅图像的 URL。
    /// </summary>
    string Banner { get; }

    /// <summary>
    ///     获取推荐服务器的介绍。
    /// </summary>
    string Description { get; }

    /// <summary>
    ///     获取推荐服务器的状态。
    /// </summary>
    int Status { get; }

    /// <summary>
    ///     获取推荐服务器的标签。
    /// </summary>
    string Tag { get; }

    /// <summary>
    ///     获取推荐服务器的特性。
    /// </summary>
    GuildFeatures Features { get; }

    /// <summary>
    ///     获取推荐服务器的所有认证。
    /// </summary>
    IReadOnlyCollection<GuildCertification>? Certifications { get; }

    /// <summary>
    ///     获取推荐服务器的服务器助力等级。
    /// </summary>
    BoostLevel BoostLevel { get; }

    /// <summary>
    ///     获取推荐服务器的自定义 ID。
    /// </summary>
    /// <remarks>
    ///     自定义 ID 可能是一个自定义字符串，当服务器拥有靓号 ID 时，自定义 ID 与靓号 ID 相同，当无自定义 ID 时为空字符串。
    /// </remarks>
    string CustomId { get; }

    /// <summary>
    ///     获取推荐服务器的靓号 ID。
    /// </summary>
    /// <remarks>
    ///     当无靓号 ID 时为空字符串。
    /// </remarks>
    string RareId { get; }

    /// <summary>
    ///     获取服务器的靓号等级。
    /// </summary>
    RareLevel RareLevel { get; }

    /// <summary>
    ///     获取推荐服务器的靓号设置。
    /// </summary>
    RareGuildResources? RareResources { get; }

    /// <summary>
    ///     获取推荐服务器是否是官方合作伙伴。
    /// </summary>
    bool IsOfficialPartner { get; }

    /// <summary>
    ///     TODO: To be documented.
    /// </summary>
    int Sort { get; }

    /// <summary>
    ///     TODO: To be documented.
    /// </summary>
    int AuditStatus { get; }

    /// <summary>
    ///     获取推荐服务器要等待多少天才能再次修改推荐信息。
    /// </summary>
    int DaysBeforeModify { get; }
}

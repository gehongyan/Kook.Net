namespace Kook;

/// <summary>
///     Represents a generic recommendation information.
/// </summary>
public interface IRecommendInfo
{
    /// <summary>
    ///     Gets the ID of the recommended guild.
    /// </summary>
    /// <returns>
    ///     A <c>ulong</c> representing the ID of the recommended guild.
    /// </returns>
    ulong GuildId { get; }

    /// <summary>
    ///     Gets the open ID for the recommended guild.
    /// </summary>
    uint? OpenId { get; }

    /// <summary>
    ///     Gets the default channel ID of the recommended guild.
    /// </summary>
    /// <returns>
    ///     A <c>ulong</c> representing the default channel ID of the recommended guild.
    /// </returns>
    ulong DefaultChannelId { get; }

    /// <summary>
    ///     Gets the name of the recommended guild.
    /// </summary>
    /// <returns>
    ///     A <c>string</c> representing the name of the recommended guild.
    /// </returns>
    string Name { get; }

    /// <summary>
    ///     Gets the icon URL of the recommended guild.
    /// </summary>
    /// <returns>
    ///     A <c>string</c> representing the icon URL of the recommended guild.
    /// </returns>
    string Icon { get; }

    /// <summary>
    ///     Gets the banner URL of the recommended guild.
    /// </summary>
    /// <returns>
    ///     A <c>string</c> representing the banner URL of the recommended guild.
    /// </returns>
    string Banner { get; }

    /// <summary>
    ///     Gets the description of the recommended guild.
    /// </summary>
    /// <returns>
    ///     A <c>string</c> representing the description of the recommended guild.
    /// </returns>
    string Description { get; }

    /// <summary>
    ///     Gets the status of the recommended guild.
    /// </summary>
    /// <returns>
    ///     A <c>int</c> representing the status of the recommended guild.
    /// </returns>
    int Status { get; }

    /// <summary>
    ///     Gets the tag of the recommended guild.
    /// </summary>
    /// <returns>
    ///     A <c>string</c> representing the tag of the recommended guild.
    /// </returns>
    string Tag { get; }

    /// <summary>
    ///     Gets the features of the recommended guild.
    /// </summary>
    GuildFeatures Features { get; }

    /// <summary>
    ///     Gets the certifications of the recommended guild.
    /// </summary>
    IReadOnlyCollection<GuildCertification>? Certifications { get; }

    /// <summary>
    ///     Gets the boost level of the recommended guild.
    /// </summary>
    /// <returns>
    ///     A <see cref="BoostLevel"/> representing the boost level of the recommended guild.
    /// </returns>
    BoostLevel BoostLevel { get; }

    /// <summary>
    ///     TODO: To be documented.
    /// </summary>
    string CustomId { get; }

    /// <summary>
    ///     Gets whether the recommended guild is an official partner.
    /// </summary>
    /// <returns>
    ///     A <c>bool</c> representing whether the recommended guild is an official partner.
    /// </returns>
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
    ///     Gets the number of days need to be waited before the recommendation information can be modified again.
    /// </summary>
    /// <returns>
    ///     An <see cref="T:System.Int32"/> representing the number of days need to be waited before the recommendation information can be modified again.
    /// </returns>
    int DaysBeforeModify { get; }
}

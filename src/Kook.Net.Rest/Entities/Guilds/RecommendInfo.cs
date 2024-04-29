using System.Diagnostics.CodeAnalysis;
using Model = Kook.API.RecommendInfo;

namespace Kook.Rest;

/// <summary>
///     Represents a recommendation information for a guild.
/// </summary>
public class RecommendInfo : IRecommendInfo
{
    /// <inheritdoc />
    public ulong GuildId { get; private set; }

    /// <inheritdoc />
    public uint? OpenId { get; private set; }

    /// <inheritdoc />
    public ulong DefaultChannelId { get; private set; }

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public string Icon { get; private set; }

    /// <inheritdoc />
    public string Banner { get; private set; }

    /// <inheritdoc />
    public string Description { get; private set; }

    /// <inheritdoc />
    public int Status { get; private set; }

    /// <inheritdoc />
    public string Tag { get; private set; }

    /// <inheritdoc />
    public GuildFeatures Features { get; private set; }

    /// <inheritdoc />
    public BoostLevel BoostLevel { get; private set; }

    /// <inheritdoc />
    public string CustomId { get; private set; }

    /// <inheritdoc />
    public bool IsOfficialPartner { get; private set; }

    /// <inheritdoc />
    public int Sort { get; private set; }

    /// <inheritdoc />
    public int AuditStatus { get; private set; }

    /// <inheritdoc />
    public int DaysBeforeModify { get; private set; }

    internal RecommendInfo()
    {
        Name = string.Empty;
        Icon = string.Empty;
        Banner = string.Empty;
        Description = string.Empty;
        Tag = string.Empty;
        CustomId = string.Empty;
    }

    internal static RecommendInfo Create(Model model)
    {
        RecommendInfo entity = new();
        entity.Update(model);
        return entity;
    }

    [MemberNotNull(
        nameof(Name),
        nameof(Icon),
        nameof(Banner),
        nameof(Description),
        nameof(Tag),
        nameof(Features),
        nameof(CustomId)
    )]
    internal void Update(Model model)
    {
        // Update properties from model
        GuildId = model.GuildId;
        OpenId = model.OpenId;
        DefaultChannelId = model.DefaultChannelId;
        Name = model.Name;
        Icon = model.Icon;
        Banner = model.Banner;
        Description = model.Description;
        Status = model.Status;
        Tag = model.Tag;
        Features = model.Features;
        BoostLevel = model.BoostLevel;
        CustomId = model.CustomId;
        IsOfficialPartner = model.IsOfficialPartner;
        Sort = model.Sort;
        AuditStatus = model.AuditStatus;
        DaysBeforeModify = model.UpdateDayInterval;
    }
}

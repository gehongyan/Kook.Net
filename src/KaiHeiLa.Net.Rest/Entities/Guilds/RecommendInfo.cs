using Model = KaiHeiLa.API.RecommendInfo;

namespace KaiHeiLa.Rest;

public class RecommendInfo : IRecommendInfo
{
    /// <inheritdoc />
    public ulong GuildId { get; private set; }
    /// <inheritdoc />
    public uint OpenId { get; private set; }
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
    public object[] Features { get; private set; }
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
    }
    
    internal static RecommendInfo Create(Model model)
    {
        var entity = new RecommendInfo();
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model)
    {
        // Update   properties from model
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
        IsOfficialPartner = model.IsOfficialPartner == 1;
        Sort = model.Sort;
        AuditStatus = model.AuditStatus;
        DaysBeforeModify = model.UpdateDayInterval;
    }
}
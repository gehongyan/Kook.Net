namespace KaiHeiLa;

public interface IRecommendInfo
{
    ulong GuildId { get; }
    uint OpenId { get; }
    ulong DefaultChannelId { get; }
    string Name { get; }
    string Icon { get; }
    string Banner { get; }
    string Description { get; }
    int Status { get; }
    string Tag { get; }
    object[] Features { get; }
    BoostLevel BoostLevel { get; }
    string CustomId { get; }
    bool IsOfficialPartner { get; }
    int Sort { get; }
    int AuditStatus { get; }
    int UpdateDayInterval { get; }
}
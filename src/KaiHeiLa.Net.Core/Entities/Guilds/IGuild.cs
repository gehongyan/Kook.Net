namespace KaiHeiLa;

public interface IGuild : IULongEntity
{
    string Name { get; }

    string Topic { get; }

    uint MasterId { get; }

    string Icon { get; }

    NotifyType NotifyType { get; }

    string Region { get; }

    bool IsOpenEnabled { get; }

    uint OpenId { get; }

    ulong DefaultChannelId { get; }

    ulong WelcomeChannelId { get; }
}
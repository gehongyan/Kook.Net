namespace KaiHeiLa;

public interface IGuildChannel : IChannel
{
    IGuild Guild { get; }

    int Position { get; }

    ChannelType Type { get; }
    
    IReadOnlyCollection<RolePermissionOverwrite> RolePermissionOverwrites { get; }
    
    IReadOnlyCollection<UserPermissionOverwrite> UserPermissionOverwrites { get; }
}
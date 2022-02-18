namespace KaiHeiLa;

public interface IGuildUser : IUser
{
    /// <summary>
    ///     用户在当前服务器的昵称
    /// </summary>
    string Nickname { get; }

    /// <summary>
    ///     用户在当前服务器中的角色 id 组成的列表
    /// </summary>
    IReadOnlyCollection<uint> RoleIds { get; }

    /// <summary>
    ///     此服务器用户所属服务器
    /// </summary>
    IGuild Guild { get; }

    /// <summary>
    ///     此服务器用户所属服务器的ID
    /// </summary>
    ulong GuildId { get; }

    bool IsMobileVerified { get; }
    
    DateTimeOffset JoinedAt { get; }
    
    DateTimeOffset ActiveAt { get; }
    
    Color Color { get; }
    
    bool? IsMaster { get; }
}
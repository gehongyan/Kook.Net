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
    
    /// <summary>
    ///     Gets the built-in role containing all users in this guild.
    /// </summary>
    /// <returns>
    ///     A role object that represents an <c>@everyone</c> role in this guild.
    /// </returns>
    IRole EveryoneRole { get; }
    
    /// <summary>
    ///     Gets a role in this guild.
    /// </summary>
    /// <param name="id">The snowflake identifier for the role.</param>
    /// <returns>
    ///     A role that is associated with the specified <paramref name="id"/>; <see langword="null" /> if none is found.
    /// </returns>
    IRole GetRole(ulong id);
}
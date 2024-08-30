namespace Kook;

/// <summary>
///     用于比较 KOOK 各种实体的 <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>。
/// </summary>
public static class KookComparers
{
    /// <summary>
    ///     获取一个用于比较 <see cref="T:Kook.IUser"/> 的 <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>。
    /// </summary>
    public static IEqualityComparer<IUser> UserComparer { get; } = new EntityEqualityComparer<IUser, ulong>();

    /// <summary>
    ///     获取一个用于比较 <see cref="T:Kook.IGuild"/> 的 <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>。
    /// </summary>
    public static IEqualityComparer<IGuild> GuildComparer { get; } = new EntityEqualityComparer<IGuild, ulong>();

    /// <summary>
    ///    获取一个用于比较 <see cref="T:Kook.IChannel"/> 的 <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>。
    /// </summary>
    public static IEqualityComparer<IChannel> ChannelComparer { get; } = new EntityEqualityComparer<IChannel, ulong>();

    /// <summary>
    ///     获取一个用于比较 <see cref="T:Kook.IRole"/> 的 <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>。
    /// </summary>
    public static IEqualityComparer<IRole> RoleComparer { get; } = new EntityEqualityComparer<IRole, uint>();

    /// <summary>
    ///     获取一个用于比较 <see cref="T:Kook.IMessage"/> 的 <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>。
    /// </summary>
    public static IEqualityComparer<IMessage> MessageComparer { get; } = new EntityEqualityComparer<IMessage, Guid>();

    private sealed class EntityEqualityComparer<TEntity, TId> : EqualityComparer<TEntity>
        where TEntity : IEntity<TId>
        where TId : IEquatable<TId>
    {
        public override bool Equals(TEntity? x, TEntity? y)
        {
            return (x, y) switch
            {
                (null, null) => true,
                (null, _) => false,
                (_, null) => false,
                _ => x.Id.Equals(y.Id)
            };
        }

        public override int GetHashCode(TEntity obj) => obj?.Id.GetHashCode() ?? 0;
    }
}

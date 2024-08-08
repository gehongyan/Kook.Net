namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的通用的私有频道，只有特定的用户可以访问。
/// </summary>
public interface IRestPrivateChannel : IPrivateChannel
{
    /// <inheritdoc cref="P:Kook.IPrivateChannel.Recipients" />
    new IReadOnlyCollection<RestUser> Recipients { get; }
}

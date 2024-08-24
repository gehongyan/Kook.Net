using System.Diagnostics;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的当前登录的用户信息。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestPresence : IPresence
{
    /// <inheritdoc />
    public bool? IsOnline { get; private set; }

    /// <inheritdoc />
    public ClientType? ActiveClient { get; private set; }

    internal RestPresence()
    {
    }

    internal RestPresence(bool? isOnline, ClientType? activeClient)
    {
        IsOnline = isOnline;
        ActiveClient = activeClient;
    }

    internal static RestPresence Create(bool? isOnline, string activeClient)
    {
        RestPresence entity = new();
        entity.Update(isOnline, activeClient);
        return entity;
    }

    internal void Update(bool? isOnline, string? activeClient)
    {
        if (isOnline.HasValue)
            IsOnline = isOnline;
        ActiveClient = ConvertClientType(activeClient);
    }

    private static ClientType? ConvertClientType(string? clientType)
    {
        if (string.IsNullOrWhiteSpace(clientType))
            return null;
        if (Enum.TryParse(clientType, true, out ClientType type))
            return type;
        return null;
    }

    private string DebuggerDisplay => $"{IsOnline switch
        {
            true => "Online",
            false => "Offline",
            _ => "Unknown Status"
        }}, {ActiveClient.ToString()}";

    internal RestPresence Clone() => (RestPresence)MemberwiseClone();
}

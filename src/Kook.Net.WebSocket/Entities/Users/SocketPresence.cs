using System.Diagnostics;

namespace Kook.WebSocket;

/// <summary>
///     Represents the WebSocket user's presence status. This may include their online status and their activity.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketPresence : IPresence
{
    /// <inheritdoc />
    public bool? IsOnline { get; private set; }

    /// <inheritdoc />
    public ClientType? ActiveClient { get; private set; }

    internal SocketPresence()
    {
    }

    internal static SocketPresence Create(bool? isOnline, string activeClient)
    {
        SocketPresence entity = new();
        entity.Update(isOnline, activeClient);
        return entity;
    }

    internal void Update(bool? isOnline)
    {
        if (isOnline.HasValue)
            IsOnline = isOnline;
    }

    internal void Update(bool? isOnline, string? activeClient)
    {
        if (isOnline.HasValue)
            IsOnline = isOnline;
        ActiveClient = ConvertClientType(activeClient);
    }

    /// <summary>
    ///     The client type where a user is active.
    /// </summary>
    /// <param name="clientType">
    ///     A string representing the client type.
    /// </param>
    /// <returns>
    ///     A <see cref="ClientType"/> that this user is active.
    /// </returns>
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

    internal SocketPresence Clone() => (SocketPresence)MemberwiseClone();
}

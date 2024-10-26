using Kook.Rest;
using System.Diagnostics;
using Model = Kook.API.Invite;

namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的邀请。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketInvite : SocketEntity<uint>, IInvite
{
    /// <inheritdoc />
    public ulong? ChannelId { get; private set; }

    /// <inheritdoc cref="Kook.IInvite.Channel" />
    public SocketGuildChannel Channel { get; }

    /// <inheritdoc />
    public ulong? GuildId { get; private set; }

    /// <inheritdoc cref="Kook.IInvite.Guild" />
    public SocketGuild Guild { get; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? ExpiresAt { get; private set; }

    /// <inheritdoc />
    public TimeSpan? MaxAge { get; private set; }

    /// <inheritdoc />
    public int? MaxUses { get; private set; }

    /// <inheritdoc />
    public int? Uses { get; private set; }

    /// <inheritdoc />
    public int? RemainingUses { get; private set; }

    /// <inheritdoc />
    public int InvitedUsersCount { get; private set; }

    /// <inheritdoc cref="Kook.IInvite.Inviter" />
    public SocketGuildUser Inviter { get; private set; }

    /// <inheritdoc />
    public string Code { get; private set; }

    /// <inheritdoc />
    public string Url { get; private set; }

    internal SocketInvite(KookSocketClient kook, SocketGuild guild,
        SocketGuildChannel channel, SocketGuildUser inviter, uint id)
        : base(kook, id)
    {
        Guild = guild;
        Channel = channel;
        Inviter = inviter;
        Code = string.Empty;
        Url = string.Empty;
    }

    internal static SocketInvite Create(KookSocketClient kook, SocketGuild guild,
        SocketGuildChannel channel, SocketGuildUser inviter, Model model)
    {
        SocketInvite entity = new(kook, guild, channel, inviter, model.Id);
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model)
    {
        Code = model.UrlCode;
        Url = model.Url;
        GuildId = model.GuildId;
        ChannelId = model.ChannelId != 0 ? model.ChannelId : null;
        CreatedAt = model.CreatedAt;
        ExpiresAt = model.ExpiresAt;
        MaxAge = model.Duration;
        MaxUses = model.UsingTimes == -1 ? null : model.UsingTimes;
        RemainingUses = model.RemainingTimes == -1 ? null : model.RemainingTimes;
        Uses = MaxUses - RemainingUses;
        InvitedUsersCount = model.InviteesCount;
    }

    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions? options = null) =>
        InviteHelper.DeleteAsync(this, Kook, options);

    /// <inheritdoc cref="Kook.WebSocket.SocketInvite.Url" />
    public override string ToString() => Url;

    private string DebuggerDisplay => $"{Url} ({Guild?.Name} / {Channel.Name})";

    /// <inheritdoc />
    IGuild IInvite.Guild => Guild;

    /// <inheritdoc />
    IChannel IInvite.Channel => Channel;

    /// <inheritdoc />
    IUser IInvite.Inviter => Inviter;

    /// <inheritdoc />
    ChannelType IInvite.ChannelType => Channel switch
    {
        IVoiceChannel => ChannelType.Voice,
        ICategoryChannel => ChannelType.Category,
        ITextChannel => ChannelType.Text,
        IThreadChannel => ChannelType.Thread,
        _ => throw new InvalidOperationException("Invalid channel type.")
    };

    /// <inheritdoc />
    string IInvite.ChannelName => Channel.Name;

    /// <inheritdoc />
    string IInvite.GuildName => Guild.Name;
}

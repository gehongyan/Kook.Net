using Kook.Rest;
using System.Diagnostics;
using Model = Kook.API.Invite;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based invite to a guild.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketInvite : SocketEntity<uint>, IInvite
{
    /// <inheritdoc />
    public ulong? ChannelId { get; private set; }

    /// <summary>
    ///     Gets the channel where this invite was created.
    /// </summary>
    public SocketGuildChannel Channel { get; }

    /// <inheritdoc />
    public ulong? GuildId { get; private set; }

    /// <summary>
    ///     Gets the guild where this invite was created.
    /// </summary>
    public SocketGuild Guild { get; }

    /// <summary>
    ///     Gets the time at which this invite will expire.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; private set; }

    /// <summary>
    ///     Gets the time span until the invite expires.
    /// </summary>
    public TimeSpan? MaxAge { get; private set; }

    /// <summary>
    ///     Gets the max number of uses this invite may have.
    /// </summary>
    public int? MaxUses { get; private set; }

    /// <summary>
    ///     Gets the number of times this invite has been used.
    /// </summary>
    public int? Uses { get; private set; }

    /// <summary>
    ///     Gets the number of times this invite still remains.
    /// </summary>
    public int? RemainingUses { get; private set; }

    /// <summary>
    ///     Gets the user that created this invite if available.
    /// </summary>
    public SocketGuildUser Inviter { get; private set; }

    /// <inheritdoc />
    public string Code { get; private set; }

    /// <inheritdoc />
    public string Url { get; private set; }

    internal SocketInvite(KookSocketClient kook, SocketGuild guild, SocketGuildChannel channel, SocketGuildUser inviter, uint id)
        : base(kook, id)
    {
        Guild = guild;
        Channel = channel;
        Inviter = inviter;
    }

    internal static SocketInvite Create(KookSocketClient kook, SocketGuild guild, SocketGuildChannel channel, SocketGuildUser inviter, Model model)
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
        ChannelId = model.ChannelId;
        ExpiresAt = model.ExpiresAt;
        MaxAge = model.Duration;
        MaxUses = model.UsingTimes == -1 ? null : model.UsingTimes;
        RemainingUses = model.RemainingTimes == -1 ? null : model.RemainingTimes;
        Uses = MaxUses - RemainingUses;
    }

    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions? options = null)
        => InviteHelper.DeleteAsync(this, Kook, options);

    /// <summary>
    ///     Gets the URL of the invite.
    /// </summary>
    /// <returns>
    ///     A string that resolves to the Url of the invite.
    /// </returns>
    public override string ToString() => Url;

    private string DebuggerDisplay => $"{Url} ({Guild?.Name} / {Channel.Name})";

    /// <inheritdoc />
    IGuild IInvite.Guild => Guild;

    /// <inheritdoc />
    IChannel IInvite.Channel => Channel;

    /// <inheritdoc />
    IUser IInvite.Inviter => Inviter;

    /// <inheritdoc />
    ChannelType IInvite.ChannelType =>
        Channel switch
        {
            IVoiceChannel voiceChannel => ChannelType.Voice,
            ICategoryChannel categoryChannel => ChannelType.Category,
            ITextChannel textChannel => ChannelType.Text,
            _ => throw new InvalidOperationException("Invalid channel type.")
        };

    /// <inheritdoc />
    string IInvite.ChannelName => Channel.Name;

    /// <inheritdoc />
    string IInvite.GuildName => Guild.Name;
}

using System.Diagnostics;
using Model = Kook.API.Invite;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based invite.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestInvite : RestEntity<uint>, IInvite, IUpdateable
{
    /// <inheritdoc />
    public string Code { get; private set; }

    /// <inheritdoc />
    public string Url { get; private set; }

    /// <inheritdoc />
    public IUser Inviter { get; private set; }

    /// <inheritdoc />
    public ChannelType ChannelType { get; private set; }

    /// <inheritdoc />
    public ulong? ChannelId { get; private set; }

    /// <inheritdoc />
    public string ChannelName { get; private set; }

    /// <inheritdoc />
    public ulong? GuildId { get; private set; }

    /// <inheritdoc />
    public string GuildName { get; private set; }

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

    internal IChannel Channel { get; }
    internal IGuild Guild { get; }

    internal RestInvite(BaseKookClient kook, IGuild guild, IChannel channel, uint id)
        : base(kook, id)
    {
        Guild = guild;
        Channel = channel;
    }

    internal static RestInvite Create(BaseKookClient kook, IGuild guild, IChannel channel, Model model)
    {
        RestInvite entity = new(kook, guild, channel, model.Id);
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model)
    {
        Code = model.UrlCode;
        Url = model.Url;
        GuildId = model.GuildId;
        ChannelId = model.ChannelId;
        GuildName = model.GuildName;
        ChannelName = model.ChannelName;
        ChannelType = model.ChannelType == ChannelType.Category ? ChannelType.Unspecified : model.ChannelType;
        Inviter = model.Inviter is not null ? RestUser.Create(Kook, model.Inviter) : null;
        ExpiresAt = model.ExpiresAt;
        MaxAge = model.Duration;
        MaxUses = model.UsingTimes == -1 ? null : model.UsingTimes;
        RemainingUses = model.RemainingTimes == -1 ? null : model.RemainingTimes;
        Uses = MaxUses - RemainingUses;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(RequestOptions? options = null)
    {
        IEnumerable<Model> model =
            await Kook.ApiClient.GetGuildInvitesAsync(GuildId, ChannelId, options: options).FlattenAsync().ConfigureAwait(false);
        Update(model.SingleOrDefault(i => i.UrlCode == Code));
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

    private string DebuggerDisplay => $"{Url} ({GuildName} / {ChannelName ?? "Channel not specified"})";

    /// <inheritdoc />
    IGuild IInvite.Guild
    {
        get
        {
            if (Guild != null) return Guild;

            if (Channel is IGuildChannel guildChannel) return guildChannel.Guild; //If it fails, it'll still return this exception

            throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
        }
    }

    /// <inheritdoc />
    IChannel IInvite.Channel
    {
        get
        {
            if (Channel != null) return Channel;

            throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
        }
    }
}

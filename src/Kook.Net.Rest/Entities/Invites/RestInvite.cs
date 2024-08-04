using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Model = Kook.API.Invite;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的邀请。
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
    public string? ChannelName { get; private set; }

    /// <inheritdoc />
    public ulong? GuildId { get; private set; }

    /// <inheritdoc />
    public string GuildName { get; private set; }

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

    internal IChannel Channel { get; }
    internal IGuild Guild { get; }

    internal RestInvite(BaseKookClient kook, IGuild guild, IChannel channel, Model model)
        : base(kook, model.Id)
    {
        Guild = guild;
        Channel = channel;
        Update(model);
    }

    internal static RestInvite Create(BaseKookClient kook, IGuild guild, IChannel channel, Model model) =>
        new(kook, guild, channel, model);

    [MemberNotNull(
        nameof(Code),
        nameof(Url),
        nameof(Inviter),
        nameof(GuildName))]
    internal void Update(Model model)
    {
        Code = model.UrlCode;
        Url = model.Url;
        GuildId = model.GuildId;
        ChannelId = model.ChannelId != 0 ? model.ChannelId : null;
        GuildName = model.GuildName;
        ChannelName = model.ChannelId != 0 ? model.ChannelName : null;
        ChannelType = model.ChannelType == ChannelType.Category ? ChannelType.Unspecified : model.ChannelType;
        Inviter = RestUser.Create(Kook, model.Inviter);
        CreatedAt = model.CreatedAt;
        ExpiresAt = model.ExpiresAt;
        MaxAge = model.Duration;
        MaxUses = model.UsingTimes == -1 ? null : model.UsingTimes;
        RemainingUses = model.RemainingTimes == -1 ? null : model.RemainingTimes;
        Uses = MaxUses - RemainingUses;
        InvitedUsersCount = model.InviteesCount;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(RequestOptions? options = null)
    {
        IEnumerable<Model> model = await Kook.ApiClient
            .GetGuildInvitesAsync(GuildId, ChannelId, options: options)
            .FlattenAsync().ConfigureAwait(false);
        if (model.SingleOrDefault(i => i.UrlCode == Code) is not { } updateModel)
            throw new InvalidOperationException("Cannot fetch the invite from the API.");
        Update(updateModel);
    }

    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions? options = null) =>
        InviteHelper.DeleteAsync(this, Kook, options);

    /// <inheritdoc cref="P:Kook.Rest.RestInvite.Url" />
    /// <returns> 此邀请的 URL，URL 的路径中包含 <see cref="P:Kook.Rest.RestInvite.Code"/> 属性的值。 </returns>
    public override string ToString() => Url;

    private string DebuggerDisplay => $"{Url} ({GuildName} / {ChannelName ?? "Channel not specified"})";

    /// <inheritdoc />
    IGuild IInvite.Guild => Guild;

    /// <inheritdoc />
    IChannel IInvite.Channel => Channel;
}

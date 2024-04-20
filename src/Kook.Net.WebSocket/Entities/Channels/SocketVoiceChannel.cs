using Kook.Rest;
using System.Collections.Immutable;
using System.Diagnostics;
using Kook.Audio;
using Model = Kook.API.Channel;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based voice channel in a guild.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketVoiceChannel : SocketTextChannel, IVoiceChannel, ISocketAudioChannel
{
    #region SocketVoiceChannel

    /// <inheritdoc />
    public VoiceQuality? VoiceQuality { get; private set; }

    /// <inheritdoc />
    public int UserLimit { get; private set; }

    /// <inheritdoc />
    public string ServerUrl { get; private set; }

    /// <inheritdoc />
    public bool? IsVoiceRegionOverwritten { get; private set; }

    /// <inheritdoc />
    public string VoiceRegion { get; private set; }

    /// <inheritdoc />
    public bool HasPassword { get; private set; }

    /// <inheritdoc />
    /// <seealso cref="SocketVoiceChannel.ConnectedUsers"/>
    public override IReadOnlyCollection<SocketGuildUser> Users
        => Guild.Users.Where(x => Permissions.GetValue(
            Permissions.ResolveChannel(Guild, x, this, Permissions.ResolveGuild(Guild, x)),
            ChannelPermission.ViewChannel)).ToImmutableArray();

    /// <summary>
    ///     Gets a collection of users that are currently connected to this voice channel.
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         This property may not always return all the members that are connected to this voice channel,
    ///         because uses may connected this voice channel before the bot has connected to the gateway.
    ///         To ensure accuracy, you may need to enable <see cref="KookSocketConfig.AlwaysDownloadVoiceStates"/>
    ///         to fetch the full voice states upon startup, or use <see cref="SocketGuild.DownloadVoiceStatesAsync"/>
    ///         on the guild this voice channel belongs to to manually download the users voice states,
    ///         or use <see cref="GetConnectedUsersAsync"/> to fetch the connected users from the API.
    ///     </note>
    /// </remarks>
    /// <returns>
    ///     A read-only collection of users that are currently connected to this voice channel.
    /// </returns>
    public IReadOnlyCollection<SocketGuildUser> ConnectedUsers
        => Guild.Users.Where(x => x.VoiceChannel?.Id == Id).ToImmutableArray();

    internal SocketVoiceChannel(KookSocketClient kook, ulong id, SocketGuild guild)
        : base(kook, id, guild)
    {
        Type = ChannelType.Voice;
    }

    internal static new SocketVoiceChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        SocketVoiceChannel entity = new(guild.Kook, model.Id, guild);
        entity.Update(state, model);
        return entity;
    }

    /// <inheritdoc />
    internal override void Update(ClientState state, Model model)
    {
        base.Update(state, model);
        VoiceQuality = model.VoiceQuality;
        UserLimit = model.UserLimit ?? 0;
        ServerUrl = model.ServerUrl;
        VoiceRegion = model.VoiceRegion;
        HasPassword = model.HasPassword;
        IsVoiceRegionOverwritten = model.OverwriteVoiceRegion;
    }

    /// <inheritdoc />
    public override SocketGuildUser GetUser(ulong id)
    {
        SocketGuildUser user = Guild.GetUser(id);
        if (user?.VoiceChannel?.Id == Id)
            return user;
        return null;
    }

    /// <inheritdoc />
    public Task ModifyAsync(Action<ModifyVoiceChannelProperties> func, RequestOptions? options = null)
        => ChannelHelper.ModifyAsync(this, Kook, func, options);

    /// <summary>
    ///     Gets a collection of users that are currently connected to this voice channel.
    /// </summary>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of users
    ///     that are currently connected to this voice channel.
    /// </returns>
    public async Task<IReadOnlyCollection<SocketGuildUser>> GetConnectedUsersAsync(CacheMode mode = CacheMode.AllowDownload,
        RequestOptions options = null)
    {
        if (mode is CacheMode.AllowDownload)
            return await SocketChannelHelper.GetConnectedUsersAsync(this, Guild, Kook, options).ConfigureAwait(false);
        else
            return ConnectedUsers;
    }

    #endregion

    #region TextOverrides

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> Getting messages from a voice channel is not supported. </exception>
    public override IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions options = null)
        => throw new NotSupportedException("Getting messages from a voice channel is not supported.");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> Getting messages from a voice channel is not supported. </exception>
    public override IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions options = null)
        => throw new NotSupportedException("Getting messages from a voice channel is not supported.");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> Getting messages from a voice channel is not supported. </exception>
    public override IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions options = null)
        => throw new NotSupportedException("Getting messages from a voice channel is not supported.");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> Getting messages from a voice channel is not supported. </exception>
    Task<IReadOnlyCollection<IMessage>> ITextChannel.GetPinnedMessagesAsync(RequestOptions? options)
        => Task.FromException<IReadOnlyCollection<IMessage>>(
            new NotSupportedException("Getting messages from a voice channel is not supported."));

    #endregion

    #region IVoiceChannel

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IUser>> IVoiceChannel.GetConnectedUsersAsync(CacheMode mode,
        RequestOptions? options = null)
        => await GetConnectedUsersAsync(mode, options).ConfigureAwait(false);

    #endregion

    private string DebuggerDisplay => $"{Name} ({Id}, Voice)";
    internal new SocketVoiceChannel Clone() => MemberwiseClone() as SocketVoiceChannel;

    #region IAudioChannel

    /// <inheritdoc />
    public Task<IAudioClient> ConnectAsync(/*bool selfDeaf = false, bool selfMute = false, */bool external = false, bool disconnect = true)
        => Guild.ConnectAudioAsync(Id, /*selfDeaf, selfMute, */external, disconnect);

    /// <inheritdoc />
    public Task DisconnectAsync()
        => Guild.DisconnectAudioAsync();

    #endregion

    #region IGuildChannel

    /// <inheritdoc />
    Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options = null)
        => Task.FromResult<IGuildUser>(GetUser(id));

    /// <inheritdoc />
    /// <seealso cref="IVoiceChannel.GetConnectedUsersAsync"/>
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode,
        RequestOptions? options = null) =>
        mode == CacheMode.AllowDownload
            ? ChannelHelper.GetUsersAsync(this, Guild, Kook, KookConfig.MaxUsersPerBatch, 1, options)
            : ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();

    #endregion

    #region INestedChannel

    /// <inheritdoc />
    Task<ICategoryChannel> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions? options = null)
        => Task.FromResult(Category);

    #endregion
}

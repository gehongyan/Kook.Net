using System.Collections.Immutable;
using System.Diagnostics;
using Kook.Audio;
using Model = Kook.API.Channel;

namespace Kook.Rest;

/// <summary>
///     表示服务器中一个基于 REST 的具有语音聊天能力的频道。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestVoiceChannel : RestTextChannel, IVoiceChannel, IRestAudioChannel
{
    #region RestVoiceChannel

    /// <inheritdoc />
    public VoiceQuality? VoiceQuality { get; private set; }

    /// <inheritdoc />
    public int UserLimit { get; private set; }

    /// <inheritdoc />
    public string? ServerUrl { get; private set; }

    /// <inheritdoc />
    public bool? IsVoiceRegionOverwritten { get; private set; }

    /// <inheritdoc />
    public string? VoiceRegion { get; private set; }

    /// <inheritdoc />
    public bool HasPassword { get; private set; }

    internal RestVoiceChannel(BaseKookClient kook, IGuild guild, ulong id)
        : base(kook, guild, id)
    {
        Type = ChannelType.Voice;
    }

    internal static new RestVoiceChannel Create(BaseKookClient kook, IGuild guild, Model model)
    {
        RestVoiceChannel entity = new(kook, guild, model.Id);
        entity.Update(model);
        return entity;
    }

    /// <inheritdoc />
    internal override void Update(Model model)
    {
        base.Update(model);
        VoiceQuality = model.VoiceQuality;
        UserLimit = model.UserLimit ?? 0;
        ServerUrl = model.ServerUrl;
        IsVoiceRegionOverwritten = model.OverwriteVoiceRegion;
        VoiceRegion = model.VoiceRegion;
        HasPassword = model.HasPassword;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(Action<ModifyVoiceChannelProperties> func, RequestOptions? options = null)
    {
        Model model = await ChannelHelper.ModifyAsync(this, Kook, func, options).ConfigureAwait(false);
        Update(model);
    }

    /// <inheritdoc />
    public override Task UpdateAsync(RequestOptions? options = null) =>
        ChannelHelper.UpdateAsync(this, Kook, options);

    /// <summary>
    ///     获取连接到此频道的用户。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含连接到此频道的所有服务器用户。 </returns>
    public async Task<IReadOnlyCollection<IUser>> GetConnectedUsersAsync(RequestOptions? options) =>
        await ChannelHelper.GetConnectedUsersAsync(this, Guild, Kook, options).ConfigureAwait(false);

    #endregion

    private string DebuggerDisplay => $"{Name} ({Id}, Voice)";

    #region TextOverrides

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持在语音频道中获取消息。 </exception>
    public override IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        throw new NotSupportedException("Getting messages from a voice channel is not supported.");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持在语音频道中获取消息。 </exception>
    public override IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(
        Guid referenceMessageId, Direction dir, int limit = KookConfig.MaxMessagesPerBatch,
        RequestOptions? options = null) =>
        throw new NotSupportedException("Getting messages from a voice channel is not supported.");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持在语音频道中获取消息。 </exception>
    public override IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(
        IMessage referenceMessage, Direction dir, int limit = KookConfig.MaxMessagesPerBatch,
        RequestOptions? options = null) =>
        throw new NotSupportedException("Getting messages from a voice channel is not supported.");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持在语音频道中获取消息。 </exception>
    public override Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions? options = null) =>
        throw new NotSupportedException("Getting messages from a voice channel is not supported.");

    #endregion

    #region IAudioChannel

    /// <inheritdoc />
    IAudioClient? IAudioChannel.AudioClient => null;

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持连接到基于 REST 的语音频道。 </exception>
    Task<IAudioClient?> IAudioChannel.ConnectAsync( /*bool selfDeaf, bool selfMute, */
        bool external, bool disconnect, string? password) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持连接到基于 REST 的语音频道。 </exception>
    Task IAudioChannel.DisconnectAsync() => throw new NotSupportedException();

    #endregion

    #region IGuildChannel

    /// <inheritdoc />
    Task<IGuildUser?> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IGuildUser?>(null);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(
        CacheMode mode, RequestOptions? options) =>
        AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();

    #endregion

    #region INestedChannel

    /// <inheritdoc />
    async Task<ICategoryChannel?> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions? options) =>
        CategoryId.HasValue && mode == CacheMode.AllowDownload
            ? await Guild.GetChannelAsync(CategoryId.Value, mode, options).ConfigureAwait(false) as ICategoryChannel
            : null;

    #endregion

    #region IVoiceChannel

    async Task<IReadOnlyCollection<IGuildUser>> IVoiceChannel.GetConnectedUsersAsync(
        CacheMode mode, RequestOptions? options) =>
        mode is CacheMode.AllowDownload
            ? await ChannelHelper.GetConnectedUsersAsync(this, Guild, Kook, options).ConfigureAwait(false)
            : ImmutableArray.Create<IGuildUser>();

    #endregion
}

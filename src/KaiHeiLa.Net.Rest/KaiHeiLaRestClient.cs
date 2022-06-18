using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace KaiHeiLa.Rest;

public class KaiHeiLaRestClient : BaseKaiHeiLaClient, IKaiHeiLaClient
{
    #region KaiHeiLaRestClient

    internal static readonly JsonSerializerOptions SerializerOptions = new()
        {Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping};
    
    /// <summary>
    ///     Gets the logged-in user.
    /// </summary>
    public new RestSelfUser CurrentUser { get => base.CurrentUser as RestSelfUser; internal set => base.CurrentUser = value; }

    public KaiHeiLaRestClient() : this(new KaiHeiLaRestConfig()) { }
    
    public KaiHeiLaRestClient(KaiHeiLaRestConfig config) : base(config, CreateApiClient(config)) { }
    
    internal KaiHeiLaRestClient(KaiHeiLaRestConfig config, API.KaiHeiLaRestApiClient api) : base(config, api) { }
    
    private static API.KaiHeiLaRestApiClient CreateApiClient(KaiHeiLaRestConfig config)
        => new API.KaiHeiLaRestApiClient(config.RestClientProvider, KaiHeiLaRestConfig.UserAgent, serializerOptions: SerializerOptions);

    internal override void Dispose(bool disposing)
    {
        if (disposing)
            ApiClient.Dispose();

        base.Dispose(disposing);
    }
    
    /// <inheritdoc />
    internal override async Task OnLoginAsync(TokenType tokenType, string token)
    {
        var user = await ApiClient.GetSelfUserAsync(new RequestOptions { RetryMode = RetryMode.AlwaysRetry }).ConfigureAwait(false);
        ApiClient.CurrentUserId = user.Id;
        base.CurrentUser = RestSelfUser.Create(this, user);
    }
    internal void CreateRestSelfUser(API.Rest.SelfUser user)
    {
        base.CurrentUser = RestSelfUser.Create(this, user);
    }
    /// <inheritdoc />
    internal override Task OnLogoutAsync()
    {
        return Task.Delay(0);
    }

    #endregion

    #region Guilds
    
    public Task<RestGuild> GetGuildAsync(ulong id, RequestOptions options = null)
        => ClientHelper.GetGuildAsync(this, id, options);
    public Task<IReadOnlyCollection<RestGuild>> GetGuildsAsync(RequestOptions options = null)
        => ClientHelper.GetGuildsAsync(this, options);
    
    #endregion

    #region Channels

    public Task<RestChannel> GetChannelAsync(ulong id, RequestOptions options = null)
        => ClientHelper.GetChannelAsync(this, id, options);
    public Task<RestDMChannel> GetDMChannelAsync(Guid chatCode, RequestOptions options = null)
        => ClientHelper.GetDMChannelAsync(this, chatCode, options);
    public Task<IReadOnlyCollection<RestDMChannel>> GetDMChannelsAsync(RequestOptions options = null)
        => ClientHelper.GetDMChannelsAsync(this, options);
    
    #endregion

    #region Roles

    public Task AddRoleAsync(ulong guildId, ulong userId, uint roleId)
        => ClientHelper.AddRoleAsync(this, guildId, userId, roleId);
    public Task RemoveRoleAsync(ulong guildId, ulong userId, uint roleId)
        => ClientHelper.RemoveRoleAsync(this, guildId, userId, roleId);

    #endregion
    
    #region Users

    public Task<RestUser> GetUserAsync(ulong id, RequestOptions options = null)
        => ClientHelper.GetUserAsync(this, id, options);
    public Task<RestGuildUser> GetGuildUserAsync(ulong guildId, ulong id, RequestOptions options = null)
        => ClientHelper.GetGuildMemberAsync(this, guildId, id, options);

    #endregion

    #region Reactions

    public Task AddReactionAsync(Guid messageId, IEmote emote, RequestOptions options = null)
        => MessageHelper.AddReactionAsync(messageId, emote, this, options);
    public Task RemoveReactionAsync(Guid messageId, ulong userId, IEmote emote, RequestOptions options = null)
        => MessageHelper.RemoveReactionAsync(messageId, userId, emote, this, options);

    public Task AddDirectMessageReactionAsync(Guid messageId, IEmote emote, RequestOptions options = null)
        => MessageHelper.AddDirectMessageReactionAsync(messageId, emote, this, options);
    public Task RemoveDirectMessageReactionAsync(Guid messageId, ulong userId, IEmote emote, RequestOptions options = null)
        => MessageHelper.RemoveDirectMessageReactionAsync(messageId, userId, emote, this, options);

    #endregion
    
    #region Assets
    
    public Task<string> CreateAssetAsync(string path, string fileName, RequestOptions options = null)
        => ClientHelper.CreateAssetAsync(this, File.OpenRead(path), fileName, options);
    public Task<string> CreateAssetAsync(Stream stream, string fileName, RequestOptions options = null)
        => ClientHelper.CreateAssetAsync(this, stream, fileName, options);

    #endregion

    #region Games
    
    public IAsyncEnumerable<IReadOnlyCollection<RestGame>> GetGamesAsync(RequestOptions options = null)
        => ClientHelper.GetGamesAsync(this, options);
    public Task<RestGame> CreateGameAsync(string name, string processName, string iconUrl, RequestOptions options = null)
        => ClientHelper.CreateGameAsync(this, name, processName, iconUrl, options);

    #endregion

    #region IKaiHeiLaClient

    /// <inheritdoc />
    async Task<IGuild> IKaiHeiLaClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetGuildAsync(id, options).ConfigureAwait(false);
        else
            return null;
    }
    /// <inheritdoc />
    async Task<IReadOnlyCollection<IGuild>> IKaiHeiLaClient.GetGuildsAsync(CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetGuildsAsync(options).ConfigureAwait(false);
        else
            return ImmutableArray.Create<IGuild>();
    }
    
    /// <inheritdoc />
    async Task<IChannel> IKaiHeiLaClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetChannelAsync(id, options).ConfigureAwait(false);
        else
            return null;
    }
    /// <inheritdoc />
    async Task<IDMChannel> IKaiHeiLaClient.GetDMChannelAsync(Guid chatCode, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetDMChannelAsync(chatCode, options).ConfigureAwait(false);
        else
            return null;
    }
    /// <inheritdoc />
    async Task<IReadOnlyCollection<IDMChannel>> IKaiHeiLaClient.GetDMChannelsAsync(CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetDMChannelsAsync(options).ConfigureAwait(false);
        else
            return ImmutableArray.Create<IDMChannel>();
    }

    #endregion
}
using System.Collections.Immutable;
using KaiHeiLa.API.Rest;

namespace KaiHeiLa.Rest;

internal static class ClientHelper
{
    public static async Task<RestGuild> GetGuildAsync(BaseKaiHeiLaClient client,
        ulong id, RequestOptions options)
    {
        var model = await client.ApiClient.GetGuildAsync(id, options).ConfigureAwait(false);
        if (model != null)
            return RestGuild.Create(client, model);
        return null;
    }
    public static async Task<IReadOnlyCollection<RestGuild>> GetGuildsAsync(BaseKaiHeiLaClient client, RequestOptions options)
    {
        var models = await client.ApiClient.GetGuildsAsync(options).ConfigureAwait(false);
        var guilds = ImmutableArray.CreateBuilder<RestGuild>();
        foreach (var model in models)
        {
            var guildModel = await client.ApiClient.GetGuildAsync(model.Id).ConfigureAwait(false);
            if (guildModel != null)
                guilds.Add(RestGuild.Create(client, guildModel));
        }
        return guilds.ToImmutable();
    }
    public static async Task<RestChannel> GetChannelAsync(BaseKaiHeiLaClient client,
        ulong id, RequestOptions options)
    {
        var model = await client.ApiClient.GetGuildChannelAsync(id, options).ConfigureAwait(false);
        if (model != null)
            return RestChannel.Create(client, model);
        return null;
    }
    
    public static async Task<RestDMChannel> GetDMChannelAsync(BaseKaiHeiLaClient client,
        Guid chatCode, RequestOptions options)
    {
        var model = await client.ApiClient.GetUserChatAsync(chatCode, options).ConfigureAwait(false);
        if (model != null)
            return RestDMChannel.Create(client, model);
        return null;
    }
    
    public static async Task<IReadOnlyCollection<RestDMChannel>> GetDMChannelsAsync(BaseKaiHeiLaClient client, RequestOptions options)
    {
        var model = await client.ApiClient.GetUserChatsAsync(options).ConfigureAwait(false);
        if (model != null)
            return model.Select(x => RestDMChannel.Create(client, x)).ToImmutableArray();
        return null;
    }
    
    public static async Task<RestUser> GetUserAsync(BaseKaiHeiLaClient client,
        ulong id, RequestOptions options)
    {
        var model = await client.ApiClient.GetUserAsync(id, options).ConfigureAwait(false);
        if (model != null)
            return RestUser.Create(client, model);
        return null;
    }

    public static async Task<string> CreateAssetAsync(BaseKaiHeiLaClient client, Stream stream, string fileName, RequestOptions options)
    {
        var model = await client.ApiClient.CreateAssetAsync(new CreateAssetParams {File = stream, FileName = fileName}, options);
        if (model != null)
            return model.Url;
        return null;
    }
}
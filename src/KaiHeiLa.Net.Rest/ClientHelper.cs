namespace KaiHeiLa.Rest;

internal static class ClientHelper
{
    public static async Task<RestChannel> GetChannelAsync(BaseKaiHeiLaClient client,
        ulong id, RequestOptions options)
    {
        var model = await client.ApiClient.GetGuildChannelAsync(id, options).ConfigureAwait(false);
        if (model != null)
            return RestChannel.Create(client, model);
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
}
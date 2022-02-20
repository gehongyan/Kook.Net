namespace KaiHeiLa.Rest;

internal static class GuildHelper
{
    #region Users

    public static async Task<RestGuildUser> GetUserAsync(IGuild guild, BaseKaiHeiLaClient client,
        ulong id, RequestOptions options)
    {
        var model = await client.ApiClient.GetGuildMemberAsync(guild.Id, id, options).ConfigureAwait(false);
        if (model != null)
            return RestGuildUser.Create(client, guild, model);
        return null;
    }

    #endregion

    #region Channels

    public static async Task<RestGuildChannel> GetChannelAsync(IGuild guild, BaseKaiHeiLaClient client,
        ulong id, RequestOptions options)
    {
        var model = await client.ApiClient.GetGuildChannelAsync(id, options).ConfigureAwait(false);
        if (model != null)
            return RestGuildChannel.Create(client, guild, model);
        return null;
    }

    #endregion
}
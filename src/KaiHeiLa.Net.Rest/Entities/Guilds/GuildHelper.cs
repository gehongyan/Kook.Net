using System.Collections.Immutable;
using KaiHeiLa.API.Rest;

namespace KaiHeiLa.Rest;

internal static class GuildHelper
{
    #region General

    public static async Task LeaveAsync(IGuild guild, BaseKaiHeiLaClient client,
        RequestOptions options)
    {
        await client.ApiClient.LeaveGuildAsync(guild.Id, options).ConfigureAwait(false);
    }

    #endregion
    
    #region Users

    public static async Task<RestGuildUser> GetUserAsync(IGuild guild, BaseKaiHeiLaClient client,
        ulong id, RequestOptions options)
    {
        var model = await client.ApiClient.GetGuildMemberAsync(guild.Id, id, options).ConfigureAwait(false);
        if (model != null)
            return RestGuildUser.Create(client, guild, model);
        return null;
    }
    
    public static async Task<(IReadOnlyCollection<ulong> Muted, IReadOnlyCollection<ulong> Deafened)> GetGuildMuteDeafListAsync(IGuild guild, BaseKaiHeiLaClient client, 
        RequestOptions options)
    {
        var models = await client.ApiClient.GetGuildMuteDeafListAsync(guild.Id, options).ConfigureAwait(false);
        return (models.Mute.UserIds, models.Deaf.UserIds);
    }
    
    public static async Task MuteUserAsync(IGuildUser user, BaseKaiHeiLaClient client, RequestOptions options)
    {
        CreateOrRemoveGuildMuteDeafParams args = new()
        {
            GuildId = user.GuildId,
            UserId = user.Id,
            Type = MuteOrDeafType.Mute
        };
        await client.ApiClient.CreateGuildMuteDeafAsync(args, options).ConfigureAwait(false);
    }
    
    public static async Task DeafenUserAsync(IGuildUser user, BaseKaiHeiLaClient client, RequestOptions options)
    {
        CreateOrRemoveGuildMuteDeafParams args = new()
        {
            GuildId = user.GuildId,
            UserId = user.Id,
            Type = MuteOrDeafType.Deaf
        };
        await client.ApiClient.CreateGuildMuteDeafAsync(args, options).ConfigureAwait(false);
    }
    
    public static async Task UnmuteUserAsync(IGuildUser user, BaseKaiHeiLaClient client, RequestOptions options)
    {
        CreateOrRemoveGuildMuteDeafParams args = new()
        {
            GuildId = user.GuildId,
            UserId = user.Id,
            Type = MuteOrDeafType.Mute
        };
        await client.ApiClient.RemoveGuildMuteDeafAsync(args, options).ConfigureAwait(false);
    }
    
    public static async Task UndeafenUserAsync(IGuildUser user, BaseKaiHeiLaClient client, RequestOptions options)
    {
        CreateOrRemoveGuildMuteDeafParams args = new()
        {
            GuildId = user.GuildId,
            UserId = user.Id,
            Type = MuteOrDeafType.Deaf
        };
        await client.ApiClient.RemoveGuildMuteDeafAsync(args, options).ConfigureAwait(false);
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
    public static async Task<IReadOnlyCollection<RestGuildChannel>> GetChannelsAsync(IGuild guild, BaseKaiHeiLaClient client,
        RequestOptions options)
    {
        var models = await client.ApiClient.GetGuildChannelsAsync(guild.Id, options).ConfigureAwait(false);
        return models.Select(x => RestGuildChannel.Create(client, guild, x)).ToImmutableArray();
    }

    #endregion

}
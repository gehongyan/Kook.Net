using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using KaiHeiLa.API;
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

    #region Invites

    public static async Task<IReadOnlyCollection<RestInvite>> GetInvitesAsync(IGuild guild, BaseKaiHeiLaClient client,
        RequestOptions options)
    {
        var models = await client.ApiClient.GetGuildInvitesAsync(guild.Id, null, options: options).FlattenAsync().ConfigureAwait(false);
        return models.Select(x => RestInvite.Create(client, guild, x.ChannelId.HasValue
            ? guild.GetChannelAsync(x.ChannelId.Value, CacheMode.CacheOnly).GetAwaiter().GetResult()
            : null, x)).ToImmutableArray();
    }
    
    /// <exception cref="ArgumentException">
    /// <paramref name="guild.Id"/> may not be equal to zero.
    /// <paramref name="maxAge"/> and <paramref name="maxUses"/> must be greater than zero.
    /// <paramref name="maxAge"/> must be lesser than 604800.
    /// </exception>
    public static async Task<RestInvite> CreateInviteAsync(IGuild guild, BaseKaiHeiLaClient client,
        int? maxAge, int? maxUses, RequestOptions options)
    {
        var args = new CreateGuildInviteParams()
        {
            GuildId = guild.Id,
            MaxAge = (InviteMaxAge) (maxAge ?? 0),
            MaxUses = (InviteMaxUses) (maxUses ?? -1),
        };
        var model = await client.ApiClient.CreateGuildInviteAsync(args, options).ConfigureAwait(false);
        var invites = await client.ApiClient.GetGuildInvitesAsync(guild.Id, options: options).FlattenAsync().ConfigureAwait(false);
        Invite invite = invites.SingleOrDefault(x => x.Url == model.Url);
        return RestInvite.Create(client, guild, invite?.ChannelId.HasValue ?? false
            ? guild.GetChannelAsync(invite.ChannelId.Value, CacheMode.CacheOnly).GetAwaiter().GetResult()
            : null, invite);
    }
    
    public static Task<RestInvite> CreateInviteAsync(IGuild channel, BaseKaiHeiLaClient client,
        InviteMaxAge maxAge, InviteMaxUses maxUses, RequestOptions options)
    {
        return CreateInviteAsync(channel, client, (int?) maxAge, (int?) maxUses, options);
    }

    #endregion
    
    #region Roles
    
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
    public static async Task<RestRole> CreateRoleAsync(IGuild guild, BaseKaiHeiLaClient client,
        string name, RequestOptions options)
    {
        if (name == null) throw new ArgumentNullException(paramName: nameof(name));

        var createGuildRoleParams = new CreateGuildRoleParams
        {
            Name = name,
            GuildId = guild.Id
        };

        var model = await client.ApiClient.CreateGuildRoleAsync(createGuildRoleParams, options).ConfigureAwait(false);

        return RestRole.Create(client, guild, model);
    }
    
    #endregion
    
    #region Users

    public static IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync(IGuild guild, BaseKaiHeiLaClient client, int limit, int fromPage, RequestOptions options)
    {
        return client.ApiClient.GetGuildMembersAsync(guild.Id, limit: limit, fromPage: fromPage, options: options)
            .Select(x => x
                .Select(y => RestGuildUser.Create(client, guild, y)).ToImmutableArray() as IReadOnlyCollection<RestGuildUser>);
    }
    
    public static async Task<RestGuildUser> GetUserAsync(IGuild guild, BaseKaiHeiLaClient client,
        ulong id, RequestOptions options)
    {
        var model = await client.ApiClient.GetGuildMemberAsync(guild.Id, id, options).ConfigureAwait(false);
        if (model != null)
            return RestGuildUser.Create(client, guild, model);
        return null;
    }
    
    public static async Task<(IReadOnlyCollection<ulong> Muted, IReadOnlyCollection<ulong> Deafened)> GetGuildMutedDeafenedUsersAsync(IGuild guild, BaseKaiHeiLaClient client, 
        RequestOptions options)
    {
        var models = await client.ApiClient.GetGuildMutedDeafenedUsersAsync(guild.Id, options).ConfigureAwait(false);
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
    
    public static IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> SearchUsersAsync(IGuild guild, BaseKaiHeiLaClient client, Action<SearchGuildMemberProperties> func, int limit, RequestOptions options)
    {
        var models = client.ApiClient.GetGuildMembersAsync(guild.Id, func, limit, 1, options);
        return models.Select(x => x.Select(y => RestGuildUser.Create(client, guild, y)).ToImmutableArray() as IReadOnlyCollection<RestGuildUser>);
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
        var models = await client.ApiClient.GetGuildChannelsAsync(guild.Id, options: options).FlattenAsync().ConfigureAwait(false);
        return models.Select(x => RestGuildChannel.Create(client, guild, x)).ToImmutableArray();
    }
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
    public static async Task<RestTextChannel> CreateTextChannelAsync(IGuild guild, BaseKaiHeiLaClient client, 
        string name, RequestOptions options, Action<TextChannelProperties> func)
    {
        if (name == null) throw new ArgumentNullException(paramName: nameof(name));

        var props = new TextChannelProperties();
        func?.Invoke(props);
        
        var args = new CreateGuildChannelParams
        {
            Name = name,
            CategoryId = props.CategoryId,
            GuildId = guild.Id,
            Type = ChannelType.Text
        };
        var model = await client.ApiClient.CreateGuildChannelAsync(args, options).ConfigureAwait(false);
        return RestTextChannel.Create(client, guild, model);
    }
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
    public static async Task<RestVoiceChannel> CreateVoiceChannelAsync(IGuild guild, BaseKaiHeiLaClient client, 
        string name, RequestOptions options, Action<VoiceChannelProperties> func)
    {
        if (name == null) throw new ArgumentNullException(paramName: nameof(name));

        var props = new VoiceChannelProperties();
        func?.Invoke(props);
        
        var args = new CreateGuildChannelParams
        {
            Name = name,
            CategoryId = props.CategoryId,
            GuildId = guild.Id,
            Type = ChannelType.Voice,
            LimitAmount = props.UserLimit,
            VoiceQuality = props.VoiceQuality
        };
        var model = await client.ApiClient.CreateGuildChannelAsync(args, options).ConfigureAwait(false);
        return RestVoiceChannel.Create(client, guild, model);
    }

    #endregion

    #region Bans

    public static async Task<IReadOnlyCollection<RestBan>> GetBansAsync(IGuild guild, BaseKaiHeiLaClient client,
        RequestOptions options)
    {
        var models = await client.ApiClient.GetGuildBansAsync(guild.Id, options).ConfigureAwait(false);
        return models.Select(x => RestBan.Create(client, x)).ToImmutableArray();
    }
    public static async Task<RestBan> GetBanAsync(IGuild guild, BaseKaiHeiLaClient client, ulong userId, RequestOptions options)
    {
        var models = await client.ApiClient.GetGuildBansAsync(guild.Id, options).ConfigureAwait(false);
        Ban model = models.FirstOrDefault(x => x.User.Id == userId);
        return model == null ? null : RestBan.Create(client, model);
    }

    public static async Task AddBanAsync(IGuild guild, BaseKaiHeiLaClient client,
        ulong userId, int pruneDays, string reason, RequestOptions options)
    {
        var args = new CreateGuildBanParams { DeleteMessageDays = pruneDays, Reason = reason, GuildId = guild.Id, UserId = userId };
        await client.ApiClient.CreateGuildBanAsync(args, options).ConfigureAwait(false);
    }
    public static async Task RemoveBanAsync(IGuild guild, BaseKaiHeiLaClient client,
        ulong userId, RequestOptions options)
    {
        var args = new RemoveGuildBanParams { GuildId = guild.Id, UserId = userId };
        await client.ApiClient.RemoveGuildBanAsync(args, options).ConfigureAwait(false);
    }

    #endregion

    #region Emotes
    
    public static async Task<IReadOnlyCollection<GuildEmote>> GetEmotesAsync(IGuild guild, BaseKaiHeiLaClient client, RequestOptions options)
    {
        var models = await client.ApiClient.GetGuildEmotesAsync(guild.Id, options: options).FlattenAsync().ConfigureAwait(false);
        return models.Select(x => x.ToEntity(client, guild.Id)).ToImmutableArray();
    }
    public static async Task<GuildEmote> GetEmoteAsync(IGuild guild, BaseKaiHeiLaClient client, string id, RequestOptions options)
    {
        var emote = await client.ApiClient.GetGuildEmotesAsync(guild.Id, options: options).FlattenAsync().ConfigureAwait(false);
        return emote.FirstOrDefault(x => x.Id == id)?.ToEntity(client, guild.Id);
    }
    public static async Task<GuildEmote> CreateEmoteAsync(IGuild guild, BaseKaiHeiLaClient client, string name, Image image, RequestOptions options)
    {
        var args = new CreateGuildEmoteParams
        {
            Name = name,
            File = image.Stream,
            GuildId = guild.Id
        };

        var emote = await client.ApiClient.CreateGuildEmoteAsync(args, options).ConfigureAwait(false);
        return emote.ToEntity(client, guild.Id);
    }
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <c>null</c>.</exception>
    public static async Task ModifyEmoteNameAsync(IGuild guild, BaseKaiHeiLaClient client, IEmote emote, Action<string> func,
        RequestOptions options)
    {
        if (func == null) throw new ArgumentNullException(paramName: nameof(func));

        string name = emote.Name;
        func(name);

        var args = new ModifyGuildEmoteParams { Name = name, Id = emote.Id };
        await client.ApiClient.ModifyGuildEmoteAsync(args, options).ConfigureAwait(false);
    }
    public static async Task DeleteEmoteAsync(IGuild guild, BaseKaiHeiLaClient client, string id, RequestOptions options) 
        => await client.ApiClient.DeleteGuildEmoteAsync(id, options).ConfigureAwait(false);

    #endregion

    #region Badges
    
    public static async Task<Stream> GetBadgeAsync(IGuild guild, BaseKaiHeiLaClient client, BadgeStyle style, RequestOptions options)
    {
        return await client.ApiClient.GetGuildBadgeAsync(guild.Id, style, options).ConfigureAwait(false);
    }

    #endregion
}
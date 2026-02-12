using System.Collections.Immutable;
using Kook.API.Rest;
using Kook.Rest.Extensions;
using Kook.WebSocket;

namespace Kook.Rest;

internal static class ExperimentalGuildHelper
{
    public static async Task<IReadOnlyCollection<RestBehaviorRestriction>> GetBehaviorRestrictionsAsync(
        BaseKookClient client, ulong guildId, RequestOptions? options)
    {
        ImmutableArray<RestBehaviorRestriction>.Builder restrictions = ImmutableArray
            .CreateBuilder<RestBehaviorRestriction>();
        IEnumerable<GuildSecurityItem> models = await client.ApiClient
            .GetGuildSecurityItemsAsync(guildId, options: options)
            .FlattenAsync()
            .ConfigureAwait(false);
        foreach (GuildSecurityItem model in models)
            restrictions.Add(RestBehaviorRestriction.Create(client, guildId, model));
        return restrictions.ToImmutable();
    }

    public static async Task<RestBehaviorRestriction> CreateBehaviorRestrictionAsync(BaseKookClient client,
        IGuild guild, string name, IReadOnlyCollection<IBehaviorRestrictionCondition> conditions,
        TimeSpan duration, BehaviorRestrictionType restrictionType, bool isEnabled, RequestOptions? options)
    {
        CreateGuildSecurityItemParams args = new()
        {
            GuildId = guild.Id,
            Action = restrictionType,
            Conditions = conditions.Select(x => x.ToModel()).ToArray(),
            LimitTime = (int)duration.TotalMinutes,
            Name = name,
            Switch = isEnabled
        };
        GuildSecurityItem created = await client.ApiClient.CreateGuildSecurityItemAsync(args, options);
        return RestBehaviorRestriction.Create(client, guild.Id, created);
    }

    public static async Task ModifyBehaviorRestrictionAsync(BaseKookClient client,
        RestBehaviorRestriction restriction, Action<ModifyBehaviorRestrictionProperties> func,
        RequestOptions? options)
    {
        ModifyBehaviorRestrictionProperties properties = new();
        func(properties);
        IEnumerable<GuildSecurityCondition>? conditions = properties.Conditions?.Select(x => x.ToModel());
        UpdateGuildSecurityItemParams args = new()
        {
            GuildId = restriction.GuildId,
            Id = restriction.Id,
            Action = properties.RestrictionType,
            Conditions = conditions is not null ? [..conditions] : null,
            LimitTime = (int?)properties.Duration?.TotalMinutes,
            Name = properties.Name,
            Switch = properties.IsEnabled
        };
        await client.ApiClient.UpdateGuildSecurityItemAsync(args, options);
    }

    public static async Task EnableBehaviorRestrictionAsync(BaseKookClient client,
        RestBehaviorRestriction restriction, RequestOptions? options) =>
        await ModifyBehaviorRestrictionAsync(client, restriction, x => x.IsEnabled = true, options);

    public static async Task DisableBehaviorRestrictionAsync(BaseKookClient client,
        RestBehaviorRestriction restriction, RequestOptions? options) =>
        await ModifyBehaviorRestrictionAsync(client, restriction, x => x.IsEnabled = false, options);

    public static async Task DeleteGuildBehaviorRestrictionAsync(BaseKookClient client,
        RestBehaviorRestriction restriction, RequestOptions? options)
    {
        DeleteGuildSecurityItemParams args = new()
        {
            GuildId = restriction.GuildId,
            Id = restriction.Id,
        };
        await client.ApiClient.DeleteGuildSecurityItemAsync(args, options);
    }
}

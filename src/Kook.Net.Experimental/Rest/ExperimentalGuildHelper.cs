using System.Collections.Immutable;
using Kook.API.Rest;
using Kook.Rest.Extensions;

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

    public static async Task DeleteBehaviorRestrictionAsync(BaseKookClient client,
        RestBehaviorRestriction restriction, RequestOptions? options)
    {
        DeleteGuildSecurityItemParams args = new()
        {
            GuildId = restriction.GuildId,
            Id = restriction.Id,
        };
        await client.ApiClient.DeleteGuildSecurityItemAsync(args, options);
    }

    public static async Task<IReadOnlyCollection<RestContentFilter>> GetContentFiltersAsync(BaseKookClient client,
        ulong guildId, RequestOptions? options)
    {
        ImmutableArray<RestContentFilter>.Builder restrictions = ImmutableArray
            .CreateBuilder<RestContentFilter>();
        IEnumerable<GuildSecurityWordfilterItem> models = await client.ApiClient
            .GetGuildSecurityWordfilterItemsAsync(guildId, options: options)
            .FlattenAsync()
            .ConfigureAwait(false);
        foreach (GuildSecurityWordfilterItem model in models)
            restrictions.Add(RestContentFilter.Create(client, guildId, model));
        return restrictions.ToImmutable();
    }

    public static async Task<RestContentFilter> CreateContentFilterAsync(BaseKookClient client, IGuild guild,
        IContentFilterTarget target, IReadOnlyCollection<IContentFilterHandler>? handlers,
        IReadOnlyCollection<ContentFilterExemption>? exemptions, bool isEnabled, RequestOptions? options)
    {
        CreateGuildWordfilterItemParams args = new()
        {
            GuildId = guild.Id,
            Type = target.Type,
            Targets = target.ToModel(),
            Handlers = [..handlers?.Select(x => x.ToModel()) ?? []],
            Exemptions = [..exemptions?.Select(x => x.ToModel()) ?? []],
            Switch = isEnabled
        };
        GuildSecurityWordfilterItem created = await client.ApiClient.CreateGuildWordfilterItemAsync(args, options);
        return RestContentFilter.Create(client, guild.Id, created);
    }

    public static async Task ModifyContentFilterAsync(BaseKookClient client,
        RestContentFilter filter, Action<ModifyContentFilterProperties> func, RequestOptions? options)
    {
        ModifyContentFilterProperties properties = new();
        func(properties);
        if (properties.Target?.Type is { } modifiedType && modifiedType != filter.Target.Type)
            throw new InvalidOperationException("Cannot change the type of the content filter target.");
        UpdateGuildWordfilterItemParams args = new()
        {
            GuildId = filter.GuildId,
            Id = filter.Id,
            Targets = properties.Target?.ToModel(),
            Handlers = properties.Handlers?.Select(x => x.ToModel()).ToArray(),
            Exemptions = properties.Exemptions?.Select(x => x.ToModel()).ToArray(),
            Switch = properties.IsEnabled
        };
        await client.ApiClient.UpdateGuildWordfilterItemAsync(args, options);
    }

    public static async Task EnableContentFilterAsync(BaseKookClient client,
        RestContentFilter filter, RequestOptions? options) =>
        await ModifyContentFilterAsync(client, filter, x => x.IsEnabled = true, options);

    public static async Task DisableContentFilterAsync(BaseKookClient client,
        RestContentFilter filter, RequestOptions? options) =>
        await ModifyContentFilterAsync(client, filter, x => x.IsEnabled = false, options);

    public static async Task DeleteContentFilterAsync(BaseKookClient client,
        RestContentFilter filter, RequestOptions? options)
    {
        DeleteGuildContentFilterParams args = new()
        {
            GuildId = filter.GuildId,
            Id = filter.Id,
        };
        await client.ApiClient.DeleteGuildWordfilterAsync(args, options);
    }
}

using Kook.API.Rest;
using Kook.Rest.Extensions;
using System.Collections.Immutable;

namespace Kook.Rest;

internal static class ExperimentalClientHelper
{
    #region Guild

    public static async Task<RestGuild> CreateGuildAsync(BaseKookClient client,
        string name, IVoiceRegion region = null, Stream icon = null, int? templateId = null, RequestOptions options = null)
    {
        var args = new CreateGuildParams
        {
            Name = name,
            RegionId = region?.Id,
            TemplateId = templateId
        };
        if (icon != null)
            args.Icon = new Image(icon);

        var model = await client.ApiClient.CreateGuildAsync(args, options).ConfigureAwait(false);
        return RestGuild.Create(client, model);
    }

    public static async Task<IReadOnlyCollection<RestGuild>> GetAdminGuildsAsync(BaseKookClient client, RequestOptions options)
    {
        var guilds = ImmutableArray.CreateBuilder<RestGuild>();
        var models = await client.ApiClient.GetAdminGuildsAsync(options: options).FlattenAsync().ConfigureAwait(false);
        foreach (var model in models)
            guilds.Add(RestGuild.Create(client, model));
        return guilds.ToImmutable();
    }

    #endregion

    #region Voice Region

    public static async Task<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync(BaseKookClient client, RequestOptions options)
    {
        var models = await client.ApiClient.GetVoiceRegionsAsync(options: options).FlattenAsync().ConfigureAwait(false);
        return models.Select(x => RestVoiceRegion.Create(client, x)).ToImmutableArray();
    }
    public static async Task<RestVoiceRegion> GetVoiceRegionAsync(BaseKookClient client, string id, RequestOptions options)
    {
        var models = await client.ApiClient.GetVoiceRegionsAsync(options: options).FlattenAsync().ConfigureAwait(false);
        return models.Select(x => RestVoiceRegion.Create(client, x)).FirstOrDefault(x => x.Id == id);
    }

    #endregion
}

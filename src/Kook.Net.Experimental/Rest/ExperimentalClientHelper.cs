using System.Collections.Immutable;
using Kook.API.Rest;
using Kook.Rest.Extensions;

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
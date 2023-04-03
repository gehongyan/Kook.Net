using Kook.API.Rest;
using Kook.Rest.Extensions;
using System.Collections.Immutable;
using Kook.API;

namespace Kook.Rest;

internal static class ExperimentalClientHelper
{
    #region Guild

    public static async Task<RestGuild> CreateGuildAsync(BaseKookClient client,
        string name, IVoiceRegion region = null, Stream icon = null, int? templateId = null, RequestOptions options = null)
    {
        CreateGuildParams args = new() { Name = name, RegionId = region?.Id, TemplateId = templateId };
        if (icon != null) args.Icon = new Image(icon);

        RichGuild model = await client.ApiClient.CreateGuildAsync(args, options).ConfigureAwait(false);
        return RestGuild.Create(client, model);
    }

    public static async Task<IReadOnlyCollection<RestGuild>> GetAdminGuildsAsync(BaseKookClient client, RequestOptions options)
    {
        ImmutableArray<RestGuild>.Builder guilds = ImmutableArray.CreateBuilder<RestGuild>();
        IEnumerable<Guild> models = await client.ApiClient.GetAdminGuildsAsync(options: options).FlattenAsync().ConfigureAwait(false);
        foreach (Guild model in models) guilds.Add(RestGuild.Create(client, model));

        return guilds.ToImmutable();
    }

    #endregion

    #region Voice Region

    public static async Task<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync(BaseKookClient client, RequestOptions options)
    {
        IEnumerable<VoiceRegion> models = await client.ApiClient.GetVoiceRegionsAsync(options: options).FlattenAsync().ConfigureAwait(false);
        return models.Select(x => RestVoiceRegion.Create(client, x)).ToImmutableArray();
    }

    public static async Task<RestVoiceRegion> GetVoiceRegionAsync(BaseKookClient client, string id, RequestOptions options)
    {
        IEnumerable<VoiceRegion> models = await client.ApiClient.GetVoiceRegionsAsync(options: options).FlattenAsync().ConfigureAwait(false);
        return models.Select(x => RestVoiceRegion.Create(client, x)).FirstOrDefault(x => x.Id == id);
    }

    #endregion

    #region Messages

    public static Task ValidateCardsAsync(KookRestClient client, IEnumerable<ICard> cards, RequestOptions options)
    {
        ValidateCardsParams args = ValidateCardsParams.FromCards(cards);
        return client.ApiClient.ValidateCardsAsync(args, options);
    }

    public static Task ValidateCardsAsync(KookRestClient client, string cardsJson, RequestOptions options)
    {
        ValidateCardsParams args = cardsJson;
        return client.ApiClient.ValidateCardsAsync(args, options);
    }

    #endregion
}

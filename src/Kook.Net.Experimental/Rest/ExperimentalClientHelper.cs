using Kook.API.Rest;
using Kook.Rest.Extensions;
using System.Collections.Immutable;
using Kook.API;

namespace Kook.Rest;

internal static class ExperimentalClientHelper
{
    #region Guild

    public static async Task<IReadOnlyCollection<RestGuild>> GetAdminGuildsAsync(BaseKookClient client, RequestOptions? options)
    {
        ImmutableArray<RestGuild>.Builder guilds = ImmutableArray.CreateBuilder<RestGuild>();
        IEnumerable<Guild> models = await client.ApiClient
            .GetAdminGuildsAsync(options: options)
            .FlattenAsync()
            .ConfigureAwait(false);
        foreach (Guild model in models)
            guilds.Add(RestGuild.Create(client, model));
        return guilds.ToImmutable();
    }

    #endregion

    #region Messages

    public static Task ValidateCardsAsync(KookRestClient client, IEnumerable<ICard> cards, RequestOptions? options)
    {
        ValidateCardsParams args = ValidateCardsParams.FromCards(cards);
        return client.ApiClient.ValidateCardsAsync(args, options);
    }

    public static Task ValidateCardsAsync(KookRestClient client, string cardsJson, RequestOptions? options)
    {
        ValidateCardsParams args = cardsJson;
        return client.ApiClient.ValidateCardsAsync(args, options);
    }

    #endregion
}

using Kook.API;
using Kook.API.Rest;

namespace Kook.Rest;

internal static class GameHelper
{
    public static async Task DeleteAsync(IGame game, BaseKookClient client,
        RequestOptions? options) =>
        await client.ApiClient.DeleteGameAsync(game.Id, options).ConfigureAwait(false);

    public static async Task<RestGame> ModifyAsync(IGame game, BaseKookClient client, Action<GameProperties> func,
        RequestOptions? options)
    {
        GameProperties properties = new() { Name = game.Name, IconUrl = game.Icon };
        func(properties);
        ModifyGameParams args = new() { Id = game.Id, Name = properties.Name, Icon = properties.IconUrl };
        Game model = await client.ApiClient.ModifyGameAsync(args, options).ConfigureAwait(false);
        return RestGame.Create(client, model);
    }
}

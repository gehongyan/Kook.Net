using Kook.Rest;

namespace Kook.WebSocket;

public static class BaseSocketClientExperimentalExtensions
{
    public static Task<RestGuild> CreateGuildAsync(this BaseSocketClient client,
        string name, IVoiceRegion region = null, Stream icon = null, int? templateId = null, RequestOptions options = null)
        => ExperimentalClientHelper.CreateGuildAsync(client, name, region, icon, templateId, options ?? RequestOptions.Default);
}

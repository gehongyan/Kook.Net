using Kook.API.Rest;
using Kook.Rest.Extensions;

namespace Kook.Rest;

internal static class ExperimentalClientHelper
{
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
    
}
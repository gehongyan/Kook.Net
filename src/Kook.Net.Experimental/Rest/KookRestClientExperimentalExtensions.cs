namespace Kook.Rest;

public static class KookRestClientExperimentalExtensions
{
    public static Task<RestGuild> CreateGuildAsync(this KookRestClient client,
        string name, IVoiceRegion region = null, Stream icon = null, int? templateId = null, RequestOptions options = null)
        => ExperimentalClientHelper.CreateGuildAsync(client, name, region, icon, templateId, options);
}

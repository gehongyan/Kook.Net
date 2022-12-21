namespace Kook.Rest;

public static class KookRestClientExperimentalExtensions
{
    public static Task<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync(this KookRestClient client, RequestOptions options = null)
        => ExperimentalClientHelper.GetVoiceRegionsAsync(client, options);
    public static Task<RestVoiceRegion> GetVoiceRegionAsync(this KookRestClient client, string id, RequestOptions options = null)
        => ExperimentalClientHelper.GetVoiceRegionAsync(client, id, options);
    public static Task<RestGuild> CreateGuildAsync(this KookRestClient client,
        string name, IVoiceRegion region = null, Stream icon = null, int? templateId = null, RequestOptions options = null)
        => ExperimentalClientHelper.CreateGuildAsync(client, name, region, icon, templateId, options);
}

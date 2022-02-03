using System.Text.Encodings.Web;
using System.Text.Json;

namespace KaiHeiLa.Rest;

public class KaiHeiLaRestClient : BaseKaiHeiLaClient, IKaiHeiLaClient
{
    #region KaiHeiLaRestClient

    internal static readonly JsonSerializerOptions SerializerOptions = new()
        { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
    
    public KaiHeiLaRestClient() : this(new KaiHeiLaRestConfig()) { }
    
    public KaiHeiLaRestClient(KaiHeiLaRestConfig config) : base(config, CreateApiClient(config)) { }
    
    internal KaiHeiLaRestClient(KaiHeiLaRestConfig config, API.KaiHeiLaRestApiClient api) : base(config, api) { }
    
    private static API.KaiHeiLaRestApiClient CreateApiClient(KaiHeiLaRestConfig config)
        => new API.KaiHeiLaRestApiClient(config.RestClientProvider, KaiHeiLaRestConfig.UserAgent, serializerOptions: SerializerOptions);

    internal override void Dispose(bool disposing)
    {
        if (disposing)
            ApiClient.Dispose();

        base.Dispose(disposing);
    }

    #endregion
}
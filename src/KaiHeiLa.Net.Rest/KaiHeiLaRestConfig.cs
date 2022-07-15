using KaiHeiLa.Net.Rest;

namespace KaiHeiLa.Rest;

public class KaiHeiLaRestConfig : KaiHeiLaConfig
{
    public RestClientProvider RestClientProvider { get; set; } = DefaultRestClientProvider.Instance;
}
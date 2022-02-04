using KaiHeiLa.Net.Rest;

namespace KaiHeiLa.Rest;

public class KaiHeiLaRestConfig : KaiHeiLaConfig
{
    public const int DefaultPaginationPerPage  = 100;
    
    public RestClientProvider RestClientProvider { get; set; } = DefaultRestClientProvider.Instance;
}
using KaiHeiLa.Net.Rest;

namespace KaiHeiLa.Rest;

/// <summary>
///     Represents a configuration class for <see cref="KaiHeiLaRestClient"/>.
/// </summary>
public class KaiHeiLaRestConfig : KaiHeiLaConfig
{
    public RestClientProvider RestClientProvider { get; set; } = DefaultRestClientProvider.Instance;
}
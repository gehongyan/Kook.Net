using Kook.Net.Rest;

namespace Kook.Rest;

/// <summary>
///     Represents a configuration class for <see cref="KookRestClient"/>.
/// </summary>
public class KookRestConfig : KookConfig
{
    public RestClientProvider RestClientProvider { get; set; } = DefaultRestClientProvider.Instance;
}
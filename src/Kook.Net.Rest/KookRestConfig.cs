using Kook.Net.Rest;

namespace Kook.Rest;

/// <summary>
///     Represents a configuration class for <see cref="KookRestClient"/>.
/// </summary>
public class KookRestConfig : KookConfig
{
    /// <summary>
    ///     Gets or sets the <see cref="Kook.Net.Rest.RestClientProvider"/> to use.
    /// </summary>
    public RestClientProvider RestClientProvider { get; set; } = DefaultRestClientProvider.Instance;
}

namespace Kook.Net.Rest;

/// <summary>
///     Represents a delegate that provides a <see cref="IRestClient"/> instance.
/// </summary>
public delegate IRestClient RestClientProvider(string baseUrl);

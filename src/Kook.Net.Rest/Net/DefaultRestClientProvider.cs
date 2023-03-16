namespace Kook.Net.Rest;

/// <summary>
///     Represents a default <see cref="RestClientProvider"/> that creates <see cref="DefaultRestClient"/> instances.
/// </summary>
public static class DefaultRestClientProvider
{
    public static readonly RestClientProvider Instance = Create();

    /// <summary>
    ///     Creates a delegate that creates a new <see cref="DefaultRestClient"/> instance.
    /// </summary>
    /// <param name="useProxy"> Whether to use the system proxy. </param>
    /// <returns> A delegate that creates a new <see cref="DefaultRestClient"/> instance. </returns>
    // /// <exception cref="PlatformNotSupportedException">The default RestClientProvider is not supported on this platform.</exception>
    public static RestClientProvider Create(bool useProxy = false)
    {
        return url =>
        {
            try
            {
                return new DefaultRestClient(url, useProxy);
            }
            catch (PlatformNotSupportedException ex)
            {
                throw new PlatformNotSupportedException("The default RestClientProvider is not supported on this platform.", ex);
            }
        };
    }
}

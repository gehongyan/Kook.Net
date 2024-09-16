namespace Kook.Net.Rest;

/// <summary>
///     表示一个默认的 <see cref="Kook.Net.Rest.RestClientProvider"/>，用于创建
///     <see cref="Kook.Net.Rest.IRestClient"/> 的默认实现的实例。
/// </summary>
public static class DefaultRestClientProvider
{
    /// <summary>
    ///     获取一个默认的 <see cref="Kook.Net.Rest.RestClientProvider"/> 委托，用于创建
    ///     <see cref="Kook.Net.Rest.IRestClient"/> 的默认实现的实例。
    /// </summary>
    public static readonly RestClientProvider Instance = Create();

    /// <summary>
    ///     创建一个新的 <see cref="Kook.Net.Rest.RestClientProvider"/> 委托。
    /// </summary>
    /// <param name="useProxy"> 是否使用系统代理。 </param>
    /// <returns> 一个新的 <see cref="Kook.Net.Rest.RestClientProvider"/> 委托。 </returns>
    public static RestClientProvider Create(bool useProxy = false) =>
        url =>
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

using System.Net;
#if NET462
using System.Net.Http;
#endif

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
    /// <seealso cref="Kook.Net.Rest.DefaultRestClientProvider.Create(System.Boolean,System.Net.IWebProxy)"/>
    public static readonly RestClientProvider Instance = Create();

    /// <summary>
    ///     创建一个新的 <see cref="Kook.Net.Rest.RestClientProvider"/> 委托。
    /// </summary>
    /// <param name="useProxy"> 是否使用系统代理。 </param>
    /// <param name="webProxy"> 代理。 </param>
    /// <returns> 一个新的 <see cref="Kook.Net.Rest.RestClientProvider"/> 委托。 </returns>
    /// <remarks>
    ///     此方式创建的 <see cref="IRestClient"/> 实例将使用默认的 HTTP 客户端实现，并根据提供的参数配置代理设置。内部创建的
    ///     <seealso cref="HttpClient"/> 实例会启用 <see cref="System.Net.DecompressionMethods.GZip"/>
    ///     及 <see cref="System.Net.DecompressionMethods.Deflate"/>，并设置请求头
    ///     <see cref="System.Net.Http.Headers.HttpRequestHeaders.AcceptEncoding"/> 为
    ///     <c>gzip, deflate</c>，以启用对压缩响应的支持。
    /// </remarks>
    public static RestClientProvider Create(bool useProxy = false, IWebProxy? webProxy = null) =>
        url =>
        {
            try
            {
                return new DefaultRestClient(url, useProxy, webProxy);
            }
            catch (PlatformNotSupportedException ex)
            {
                throw new PlatformNotSupportedException("The default RestClientProvider is not supported on this platform.", ex);
            }
        };

    /// <summary>
    ///     创建一个新的 <see cref="Kook.Net.Rest.RestClientProvider"/> 委托。
    /// </summary>
    /// <param name="httpClientFactory"> 用于创建 <see cref="HttpClient"/> 实例的工厂方法。 </param>
    /// <returns> 一个新的 <see cref="Kook.Net.Rest.RestClientProvider"/> 委托。 </returns>
    /// <remarks>
    ///     在使用自定义的 <see cref="HttpClient"/> 工厂方法时，请确保返回的 <see cref="HttpClient"/>
    ///     实例具有适当的配置，例如正确的超时设置、代理设置等，以确保与 KOOK API 的通信正常进行。Kook.Net
    ///     不会为此方式创建的 <see cref="HttpClient"/> 实例进行任何管理或处理，因此需要开发者自行负责这些实例的生命周期和配置。
    ///     <note type="warning">
    ///         此方式创建的 <see cref="HttpClient"/> 不应设置其 <see cref="HttpClient.BaseAddress"/> 属性。
    ///     </note>
    /// </remarks>
    /// <example>
    ///     以下示例代码，在依赖注入环境中，展示了如何使用自定义的 <see cref="HttpClient"/> 工厂方法来创建一个
    ///     <see cref="RestClientProvider"/>。<br />
    ///     首先在依赖注入容器中注册一个命名的 <see cref="HttpClient"/>，并配置其默认请求头和消息处理程序：
    ///     <code language="cs">
    ///     HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());
    ///     builder.Services.AddHttpClient("KookRestHttpClient")
    ///         .ConfigureHttpClient(x =&gt;
    ///         {
    ///             x.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
    ///             x.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
    ///         })
    ///         .ConfigurePrimaryHttpMessageHandler(() =&gt; new HttpClientHandler
    ///         {
    ///             AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
    ///             UseCookies = false,
    ///         });
    ///     </code>
    ///     然后在注册 KOOK 客户端的配置中，使用该命名的 <see cref="HttpClient"/> 来创建一个 <see cref="RestClientProvider"/>：
    ///     <code language="cs">
    ///     builder.Services.AddSingleton&lt;KookSocketConfig&gt;(provider =&gt;
    ///     {
    ///         IHttpClientFactory httpClientFactory = provider.GetRequiredService&lt;IHttpClientFactory&gt;();
    ///         return new KookSocketConfig
    ///         {
    ///             RestClientProvider = DefaultRestClientProvider.Create(() =&gt; httpClientFactory.CreateClient("KookRestHttpClient"))
    ///         };
    ///     });
    ///     </code>
    /// </example>
    public static RestClientProvider Create(Func<HttpClient> httpClientFactory) =>
        url =>
        {
            try
            {
                return new DefaultRestClient(url, false, null, httpClientFactory);
            }
            catch (PlatformNotSupportedException ex)
            {
                throw new PlatformNotSupportedException("The default RestClientProvider is not supported on this platform.", ex);
            }
        };
}

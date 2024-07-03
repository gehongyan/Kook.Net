namespace Kook.Net.Rest;

/// <summary>
///     表示一个用于创建 <see cref="IRestClient"/> 实例的委托。
/// </summary>
public delegate IRestClient RestClientProvider(string baseUrl);

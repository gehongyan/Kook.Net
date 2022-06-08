// 在 GetUserAsync 的结果中，RestUser 实体包含了用户登录开黑啦的连接方式，
// 而在 RestGuild 上的 GetUsersAsync 结果中的 RestUser 不包含此信息。
// Socket 建立连接后，如果配置指示客户端始终下载全部用户信息，
// 客户端则会通过 RestGuild.GetUsersAsync 所实际调用的 API 获取服务器用户信息，
// 尽管如此，全局缓存中也不会包含可靠的用户连接方式的信息。
// 在这种少数情况下，则需要通过访问 GetUserAsync 返回的 RestUser 才能获取所需要的连接方式信息。
public static async Task<ClientType?> GetUserClientType(IUser user, KaiHeiLaRestClient restClient)
{
    var restUser = await restClient.GetUserAsync(user.Id);
    return restUser.ActiveClient;
}
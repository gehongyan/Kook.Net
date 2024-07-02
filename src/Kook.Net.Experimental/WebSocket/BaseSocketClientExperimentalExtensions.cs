using Kook.Rest;

namespace Kook.WebSocket;

/// <summary>
///     Provides extension methods of experimental functionalities for <see cref="BaseSocketClient"/>s.
/// </summary>
public static class BaseSocketClientExperimentalExtensions
{
    /// <summary>
    ///     Creates a guild for the logged-in user.
    /// </summary>
    /// <remarks>
    ///     This method creates a new guild on behalf of the logged-in user.
    ///     <br />
    ///     <note type="warning">
    ///         This method is still in experimental state, which means that it is not for official API implementation
    ///         usage, may violate the developer rules or policies, not guaranteed to be stable,and may be changed or removed in the future.
    ///     </note>
    /// </remarks>
    /// <param name="client">The KOOK rest client instance.</param>
    /// <param name="name">The name of the new guild.</param>
    /// <param name="region">The voice region to create the guild with.</param>
    /// <param name="icon">The icon of the new guild.</param>
    /// <param name="templateId">The identifier of the guild template to be used to create the new guild.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous creation operation. The task result contains the created guild.
    /// </returns>
    public static Task<RestGuild> CreateGuildAsync(this BaseSocketClient client, string name,
        IVoiceRegion? region = null, Stream? icon = null, int? templateId = null, RequestOptions? options = null) =>
        ExperimentalClientHelper.CreateGuildAsync(client, name, region, icon, templateId, options);
}

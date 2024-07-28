namespace Kook;

/// <summary>
///     表示一个通用的当前登录的用户信息。
/// </summary>
public interface ISelfUser : IUser
{
    /// <summary>
    ///     获取此用户的手机号码前缀。
    /// </summary>
    string? MobilePrefix { get; }

    /// <summary>
    ///     获取此用户的手机号码。
    /// </summary>
    string? Mobile { get; }

    /// <summary>
    ///     获取此用户的邀请用户数。
    /// </summary>
    int InvitedCount { get; }

    /// <summary>
    ///     获取此用户是否验证了手机号码。
    /// </summary>
    bool IsMobileVerified { get; }

    /// <summary>
    ///     开始一个新的游戏活动。
    /// </summary>
    /// <param name="game"> 要显示的游戏。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步开始操作的任务。 </returns>
    /// <remarks>
    ///     此操作会使当前用户的资料卡片上显式指定的游戏信息。
    /// </remarks>
    Task StartPlayingAsync(IGame game, RequestOptions? options = null);

    /// <summary>
    ///     开始一个新的听音乐活动。
    /// </summary>
    /// <param name="music"> 要播放的音乐。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步开始操作的任务。 </returns>
    /// <remarks>
    ///     此操作会使当前用户的资料卡片上显式指定的音乐信息。
    /// </remarks>
    Task StartPlayingAsync(Music music, RequestOptions? options = null);

    /// <summary>
    ///     停止活动。
    /// </summary>
    /// <param name="type"> 要停止的活动类型。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步停止操作的任务。 </returns>
    /// <remarks>
    ///     此操作会使当前用户的资料卡片上不再显示指定的活动信息。
    /// </remarks>
    Task StopPlayingAsync(ActivityType type, RequestOptions? options = null);
}

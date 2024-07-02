namespace Kook;

/// <summary>
///     表示是一个通用的游戏信息。
/// </summary>
public interface IGame : IActivity, IEntity<int>, IDeletable
{
    /// <summary>
    ///     获取游戏的名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     获取游戏的类型。
    /// </summary>
    GameType GameType { get; }

    /// <summary>
    ///     获取游戏的额外信息。
    /// </summary>
    string? Options { get; }

    /// <summary>
    ///     获取 KOOK 客户端是否需要管理员权限来检测游戏进程。
    /// </summary>
    bool RequireAdminPrivilege { get; }

    /// <summary>
    ///     获取游戏的进程名称。
    /// </summary>
    IReadOnlyCollection<string> ProcessNames { get; }

    /// <summary>
    ///     获取游戏的产品名称。
    /// </summary>
    IReadOnlyCollection<string> ProductNames { get; }

    /// <summary>
    ///     获取游戏图标的 URL。
    /// </summary>
    string? Icon { get; }

    /// <summary>
    ///     修改此游戏信息的属性。
    /// </summary>
    /// <remarks>
    ///     此方法使用指定的属性修改当前游戏信息。要查看可用的属性，请参考 <see cref="GameProperties"/>。
    /// </remarks>
    /// <param name="func"> 一个包含修改游戏属性的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示信息属性修改操作的异步任务。 </returns>
    Task<IGame> ModifyAsync(Action<GameProperties> func, RequestOptions? options = null);
}

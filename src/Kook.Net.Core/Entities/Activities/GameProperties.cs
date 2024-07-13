namespace Kook;

/// <summary>
///     提供用于修改 <see cref="T:Kook.IGame" /> 的属性。
/// </summary>
/// <seealso cref="M:Kook.IGame.ModifyAsync(System.Action{Kook.GameProperties},Kook.RequestOptions)"/>
public class GameProperties
{
    /// <summary>
    ///     获取或设置游戏的名称。
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     获取或设置游戏的图标 URL。
    /// </summary>
    public string? IconUrl { get; set; }
}

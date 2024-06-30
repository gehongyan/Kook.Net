namespace Kook;

/// <summary>
///     用于修改 <see cref="IGame" /> 的属性，以应用指定的更改。
/// </summary>
/// <seealso cref="IGame.ModifyAsync"/>
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

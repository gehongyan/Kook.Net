namespace Kook;

/// <summary>
///     表示一个通用的表情符号。
/// </summary>
public interface IEmote : IEntity<string>
{
    /// <summary>
    ///     获取此表情符号的显示名称或 Unicode 表示。
    /// </summary>
    string Name { get; }
}

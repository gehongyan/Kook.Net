namespace Kook;

/// <summary>
///     表示一个通用的 POKE 资源。
/// </summary>
public interface IPokeResource
{
    /// <summary>
    ///     获取此 POKE 资源的类型。
    /// </summary>
    PokeResourceType Type { get; }
}

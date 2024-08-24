namespace Kook;

/// <summary>
///     表示一个通用的媒体模块，可用于 <see cref="ICard"/> 中。
/// </summary>
public interface IMediaModule : IModule
{
    /// <summary>
    ///     获取与此模块关联的媒体的源。
    /// </summary>
    string Source { get; }

    /// <summary>
    ///     获取与此模块关联的媒体的标题。
    /// </summary>
    string? Title { get; }
}

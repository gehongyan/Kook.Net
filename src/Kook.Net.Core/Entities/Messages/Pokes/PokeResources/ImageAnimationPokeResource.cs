namespace Kook;

/// <summary>
///     表示一个 POKE 的图像与动画的资源。
/// </summary>
public struct ImageAnimationPokeResource : IPokeResource
{
    internal ImageAnimationPokeResource(IReadOnlyDictionary<string, string> resources,
        TimeSpan duration, int width, int height, decimal percent)
    {
        Resources = resources;
        Duration = duration;
        Width = width;
        Height = height;
        Percent = percent;
    }

    /// <inheritdoc />
    public PokeResourceType Type => PokeResourceType.ImageAnimation;

    /// <summary>
    ///     获取图像动画的资源。
    /// </summary>
    public IReadOnlyDictionary<string, string> Resources { get; }

    /// <summary>
    ///     获取此动画以全屏的形式播放的持续时间。
    /// </summary>
    public TimeSpan Duration { get; }

    /// <summary>
    ///     获取图像动画的宽度。
    /// </summary>
    public int Width { get; }

    /// <summary>
    ///     获取图像动画的高度。
    /// </summary>
    public int Height { get; }

    /// <summary>
    ///     // TODO: To be documented.
    /// </summary>
    public decimal Percent { get; }
}

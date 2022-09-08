namespace Kook;

/// <summary>
///     Represents an image animation poke resource.
/// </summary>
public struct ImageAnimationPokeResource : IPokeResource
{
    public ImageAnimationPokeResource(IReadOnlyDictionary<string, string> resources, TimeSpan duration, int width, int height, double percent)
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
    ///     Gets the resources of the image animation.
    /// </summary>
    public IReadOnlyDictionary<string, string> Resources { get; internal set; }
    
    /// <summary>
    ///     Gets how long this animation animation should last filling the full screen.
    /// </summary>
    public TimeSpan Duration { get; internal set; }
    
    /// <summary>
    ///     Gets the width of the image animation.
    /// </summary>
    public int Width { get; internal set; }

    /// <summary>
    ///     Gets the height of the image animation.
    /// </summary>
    public int Height { get; internal set; }
    
    /// <summary>
    ///     // TODO: To be documented.
    /// </summary>
    public double Percent { get; internal set; }
}
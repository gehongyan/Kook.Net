using System.Collections.Immutable;
using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
/// Represents an image group module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ImageGroupModule : IModule
{
    internal ImageGroupModule(ImmutableArray<ImageElement> elements)
    {
        Elements = elements;
    }
    
       /// <inheritdoc />
    public ModuleType Type => ModuleType.ImageGroup;

    /// <summary>
    ///     Gets the image elements in this image group module.
    /// </summary>
    /// <returns>
    ///     An <see cref="ImmutableArray{ImageElement}"/> representing the images in this image group module.
    /// </returns>
    public ImmutableArray<ImageElement> Elements { get; internal set; }
    
    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
}
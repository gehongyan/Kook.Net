using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
/// Represents an image group module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ImageGroupModule : IModule, IEquatable<ImageGroupModule>, IEquatable<IModule>
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
    public ImmutableArray<ImageElement> Elements { get; }

    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";

    /// <summary>
    ///     Determines whether the specified <see cref="ImageGroupModule"/> is equal to the current <see cref="ImageGroupModule"/>.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="ImageGroupModule"/> is equal to the current <see cref="ImageGroupModule"/>;
    /// </returns>
    public static bool operator ==(ImageGroupModule left, ImageGroupModule right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ImageGroupModule"/> is not equal to the current <see cref="ImageGroupModule"/>.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="ImageGroupModule"/> is not equal to the current <see cref="ImageGroupModule"/>;
    /// </returns>
    public static bool operator !=(ImageGroupModule left, ImageGroupModule right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="ImageGroupModule"/> is equal to the current <see cref="ImageGroupModule"/>.</summary>
    /// <remarks>If the object passes is an <see cref="ImageGroupModule"/>, <see cref="Equals(ImageGroupModule)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="ImageGroupModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ImageGroupModule"/> is equal to the current <see cref="ImageGroupModule"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is ImageGroupModule imageGroupModule && Equals(imageGroupModule);

    /// <summary>Determines whether the specified <see cref="ImageGroupModule"/> is equal to the current <see cref="ImageGroupModule"/>.</summary>
    /// <param name="imageGroupModule">The <see cref="ImageGroupModule"/> to compare with the current <see cref="ImageGroupModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ImageGroupModule"/> is equal to the current <see cref="ImageGroupModule"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] ImageGroupModule? imageGroupModule)
        => GetHashCode() == imageGroupModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            hash = (hash * 16777619) ^ Type.GetHashCode();
            foreach (ImageElement element in Elements)
                hash = (hash * 16777619) ^ element.GetHashCode();
            return hash;
        }
    }

    bool IEquatable<IModule>.Equals([NotNullWhen(true)] IModule? module) =>
        Equals(module as ImageGroupModule);
}

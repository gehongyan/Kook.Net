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
    ///     判定两个 <see cref="ImageGroupModule"/> 是否相等。
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="ImageGroupModule"/> is equal to the current <see cref="ImageGroupModule"/>;
    /// </returns>
    public static bool operator ==(ImageGroupModule left, ImageGroupModule right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="ImageGroupModule"/> 是否不相等。
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="ImageGroupModule"/> is not equal to the current <see cref="ImageGroupModule"/>;
    /// </returns>
    public static bool operator !=(ImageGroupModule left, ImageGroupModule right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ImageGroupModule imageGroupModule && Equals(imageGroupModule);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] ImageGroupModule? imageGroupModule) =>
        GetHashCode() == imageGroupModule?.GetHashCode();

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

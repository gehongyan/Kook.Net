using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Represents a section module in card.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SectionModule : IModule, IEquatable<SectionModule>, IEquatable<IModule>
{
    internal SectionModule(SectionAccessoryMode? mode, IElement? text = null, IElement? accessory = null)
    {
        Mode = mode;
        Text = text;
        Accessory = accessory;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Section;

    /// <summary>
    ///     Specifies that the <see cref="Accessory"/> is to the left or right of <see cref="Text"/>.
    /// </summary>
    /// <returns>
    ///     <see cref="SectionAccessoryMode.Left"/> if the <see cref="Accessory"/> is to the left of <see cref="Text"/>,
    ///     <see cref="SectionAccessoryMode.Right"/> if the <see cref="Accessory"/> is to the right of <see cref="Text"/>,
    /// </returns>
    public SectionAccessoryMode? Mode { get; }

    /// <summary>
    ///     Gets the text of the section.
    /// </summary>
    /// <returns>
    ///     An <see cref="IElement"/> representing the text of the section.
    /// </returns>
    public IElement? Text { get; }

    /// <summary>
    ///     Gets the accessory of the section.
    /// </summary>
    /// <returns>
    ///     An <see cref="IElement"/> representing the accessory of the section.
    /// </returns>
    public IElement? Accessory { get; }

    private string DebuggerDisplay => $"{Type}: {Text}{(Accessory is null ? string.Empty : $"{Mode} Accessory")}";

    /// <summary>
    ///     判定两个 <see cref="SectionModule"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="SectionModule"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(SectionModule left, SectionModule right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="SectionModule"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="SectionModule"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(SectionModule left, SectionModule right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is SectionModule sectionModule && Equals(sectionModule);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] SectionModule? sectionModule) =>
        GetHashCode() == sectionModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            hash = (hash * 16777619) ^ (Type, Mode).GetHashCode();
            hash = (hash * 16777619) ^ (Text?.GetHashCode() ?? 0);
            hash = (hash * 16777619) ^ (Accessory?.GetHashCode() ?? 0);
            return hash;
        }
    }

    bool IEquatable<IModule>.Equals([NotNullWhen(true)] IModule? module) =>
        Equals(module as SectionModule);
}

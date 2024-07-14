using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     A file module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class FileModule : IMediaModule, IEquatable<FileModule>, IEquatable<IModule>
{
    internal FileModule(string source, string? title, int? size = null)
    {
        Source = source;
        Title = title;
        Size = size;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.File;

    /// <inheritdoc />
    public string Source { get; }

    /// <inheritdoc />
    public string? Title { get; }

    /// <summary>
    ///     The size of the file in bytes.
    /// </summary>
    public int? Size { get; }

    private string DebuggerDisplay => $"{Type}: {Title}";

    /// <summary>
    ///     判定两个 <see cref="FileModule"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="FileModule"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(FileModule left, FileModule right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="FileModule"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="FileModule"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(FileModule left, FileModule right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is FileModule fileModule && Equals(fileModule);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] FileModule? fileModule) =>
        GetHashCode() == fileModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode() =>
        (Type, Source, Title).GetHashCode();

    bool IEquatable<IModule>.Equals([NotNullWhen(true)] IModule? module) =>
        Equals(module as FileModule);
}

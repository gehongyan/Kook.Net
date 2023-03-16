using System.Diagnostics;

namespace Kook;

/// <summary>
///     A file module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class FileModule : IMediaModule, IEquatable<FileModule>
{
    internal FileModule(string source, string title)
    {
        Source = source;
        Title = title;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.File;

    /// <inheritdoc />
    public string Source { get; }

    /// <inheritdoc />
    public string Title { get; }

    private string DebuggerDisplay => $"{Type}: {Title}";

    /// <summary>
    ///     Determines whether the specified <see cref="FileModule"/> is equal to the current <see cref="FileModule"/>.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="FileModule"/> is equal to the current <see cref="FileModule"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(FileModule left, FileModule right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="FileModule"/> is not equal to the current <see cref="FileModule"/>.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="FileModule"/> is not equal to the current <see cref="FileModule"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(FileModule left, FileModule right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="FileModule"/> is equal to the current <see cref="FileModule"/>.</summary>
    /// <remarks>If the object passes is an <see cref="FileModule"/>, <see cref="Equals(FileModule)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="FileModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="FileModule"/> is equal to the current <see cref="FileModule"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is FileModule fileModule && Equals(fileModule);

    /// <summary>Determines whether the specified <see cref="FileModule"/> is equal to the current <see cref="FileModule"/>.</summary>
    /// <param name="fileModule">The <see cref="FileModule"/> to compare with the current <see cref="FileModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="FileModule"/> is equal to the current <see cref="FileModule"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(FileModule fileModule)
        => GetHashCode() == fileModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Type, Source, Title).GetHashCode();
}

using System.Diagnostics;

namespace Kook;

/// <summary>
///     Represents a header module in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class HeaderModule : IModule, IEquatable<HeaderModule>
{
    internal HeaderModule(PlainTextElement text)
    {
        Text = text;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Header;

    /// <summary>
    ///     Gets the text element of the header.
    /// </summary>
    /// <returns>
    ///     A <see cref="PlainTextElement"/> representing the text of the header.
    /// </returns>
    public PlainTextElement Text { get; }
    
    public override string ToString() => Text.ToString();
    private string DebuggerDisplay => $"{Type}: {Text}";
    
    public static bool operator ==(HeaderModule left, HeaderModule right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(HeaderModule left, HeaderModule right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="HeaderModule"/> is equal to the current <see cref="HeaderModule"/>.</summary>
    /// <remarks>If the object passes is an <see cref="HeaderModule"/>, <see cref="Equals(HeaderModule)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="HeaderModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="HeaderModule"/> is equal to the current <see cref="HeaderModule"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is HeaderModule headerModule && Equals(headerModule);

    /// <summary>Determines whether the specified <see cref="HeaderModule"/> is equal to the current <see cref="HeaderModule"/>.</summary>
    /// <param name="headerModule">The <see cref="HeaderModule"/> to compare with the current <see cref="HeaderModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="HeaderModule"/> is equal to the current <see cref="HeaderModule"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(HeaderModule headerModule)
        => GetHashCode() == headerModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int) 2166136261;
            hash = (hash * 16777619) ^ Type.GetHashCode();
            hash = (hash * 16777619) ^ Text.GetHashCode();
            return hash;
        }
    }
}
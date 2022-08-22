using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     A divider module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class DividerModule : IModule
{
    internal DividerModule()
    {
        
    }
    
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Divider;
    
    private string DebuggerDisplay => $"{Type}";
    
    public static bool operator ==(DividerModule left, DividerModule right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(DividerModule left, DividerModule right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="DividerModule"/> is equal to the current <see cref="DividerModule"/>.</summary>
    /// <remarks>If the object passes is an <see cref="DividerModule"/>, <see cref="Equals(DividerModule)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="DividerModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="DividerModule"/> is equal to the current <see cref="DividerModule"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is DividerModule dividerModule && Equals(dividerModule);

    /// <summary>Determines whether the specified <see cref="DividerModule"/> is equal to the current <see cref="DividerModule"/>.</summary>
    /// <param name="dividerModule">The <see cref="DividerModule"/> to compare with the current <see cref="DividerModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="DividerModule"/> is equal to the current <see cref="DividerModule"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(DividerModule dividerModule)
        => GetHashCode() == dividerModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => Type.GetHashCode();
}
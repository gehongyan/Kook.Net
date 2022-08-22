namespace Kook;

/// <summary>
///     Represents a generic permission overwrite.
/// </summary>
/// <typeparam name="TTarget">
///     The type of the target.
/// </typeparam>
public interface IPermissionOverwrite<TTarget>
{
    /// <summary>
    ///     Gets the target of this overwrite.
    /// </summary>
    TTarget Target { get; }
    
    /// <summary>
    ///     Gets the overwritten permission.
    /// </summary>
    OverwritePermissions Permissions { get; }
}
namespace Kook;

/// <summary>
///     Properties that are used to modify an <see cref="IRole" /> with the specified changes.
/// </summary>
/// <seealso cref="IRole.ModifyAsync" />
public class RoleProperties
{
    /// <summary>
    ///     Gets or sets the name of the role.
    /// </summary>
    /// <remarks>
    ///     This value may not be set if the role is an @everyone role.
    /// </remarks>
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the color of the role.
    /// </summary>
    /// <remarks>
    ///     This value may not be set if the role is an @everyone role.
    /// </remarks>
    public Color? Color { get; set; }

    /// <summary>
    ///     Gets or sets whether or not this role should be displayed independently in the user list.
    /// </summary>
    /// <remarks>
    ///     This value may not be set if the role is an @everyone role.
    /// </remarks>
    public bool? Hoist { get; set; }

    /// <summary>
    ///     Gets or sets whether or not this role can be mentioned.
    /// </summary>
    /// <remarks>
    ///     This value may not be set if the role is an @everyone role.
    /// </remarks>
    public bool? Mentionable { get; set; }

    /// <summary>
    ///     Gets or sets the role's <see cref="GuildPermission"/>.
    /// </summary>
    public GuildPermissions? Permissions { get; set; }
}

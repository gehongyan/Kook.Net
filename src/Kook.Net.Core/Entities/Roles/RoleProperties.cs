namespace Kook;

/// <summary>
///     提供用于修改 <see cref="Kook.IRole"/> 的属性。
/// </summary>
/// <seealso cref="Kook.IRole.ModifyAsync(System.Action{Kook.RoleProperties},Kook.RequestOptions)" />
public class RoleProperties
{
    /// <summary>
    ///     获取或设置要设置到此角色的名称。
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     获取或设置要设置到此角色的颜色。
    /// </summary>
    public Color? Color { get; set; }

    /// <summary>
    ///     获取或设置要设置到此角色拥有此角色的用户是否在用户列表中与普通在线成员分开显示。
    /// </summary>
    public bool? IsHoisted { get; set; }

    /// <inheritdoc cref="Kook.RoleProperties.IsHoisted" />
    [Obsolete("Use IsHoisted instead.")]
    public bool? Hoist
    {
        get => IsHoisted;
        set => IsHoisted = value;
    }

    /// <summary>
    ///     获取或设置要设置到此角色是否允许任何人提及此角色。
    /// </summary>
    public bool? IsMentionable { get; set; }

    /// <inheritdoc cref="Kook.RoleProperties.IsMentionable" />
    [Obsolete("Use IsMentionable instead.")]
    public bool? Mentionable
    {
        get => IsMentionable;
        set => IsMentionable = value;
    }

    /// <summary>
    ///     获取或设置要设置到此角色的权限。
    /// </summary>
    public GuildPermissions? Permissions { get; set; }
}

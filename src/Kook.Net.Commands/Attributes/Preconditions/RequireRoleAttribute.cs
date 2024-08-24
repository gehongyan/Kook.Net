namespace Kook.Commands;

/// <summary>
///     要求调用命令的用户在命令调用所在的服务器拥有指定的角色。
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireRoleAttribute : PreconditionAttribute
{
    private readonly string? _roleName;
    private readonly uint? _roleId;

    /// <summary>
    ///     获取或设置由于在服务器频道外执行命令而导致的先决条件失败的错误消息。
    /// </summary>
    public string? NotAGuildErrorMessage { get; set; }

    /// <summary>
    ///     初始化一个 <see cref="RequireRoleAttribute"/> 类的新实例。
    /// </summary>
    /// <param name="roleId"> 所要求调用命令的用户需要在命令调用所在的的服务器拥有的角色的 ID。 </param>
    public RequireRoleAttribute(uint roleId)
    {
        _roleId = roleId;
    }

    /// <summary>
    ///     初始化一个 <see cref="RequireRoleAttribute"/> 类的新实例。
    /// </summary>
    /// <param name="roleName"> 所要求调用命令的用户需要在命令调用所在的的服务器拥有的角色的名称。 </param>
    public RequireRoleAttribute(string roleName)
    {
        _roleName = roleName;
    }

    /// <inheritdoc />
    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.User is not IGuildUser guildUser)
            return Task.FromResult(PreconditionResult.FromError(NotAGuildErrorMessage ?? "Command must be used in a guild channel."));

        if (_roleId.HasValue)
        {
            return Task.FromResult(guildUser.RoleIds.Contains(_roleId.Value)
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError(ErrorMessage ?? $"User requires guild role {guildUser.Guild.GetRole(_roleId.Value)?.Name}."));
        }

        if (!string.IsNullOrEmpty(_roleName))
        {
            IEnumerable<string> roleNames = guildUser.RoleIds
                .Select(x => guildUser.Guild.GetRole(x))
                .OfType<IRole>()
                .Select(x => x.Name);
            return Task.FromResult(roleNames.Contains(_roleName)
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError(ErrorMessage ?? $"User requires guild role {_roleName}."));
        }

        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}

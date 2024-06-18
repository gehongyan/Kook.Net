namespace Kook.Commands;

/// <summary>
///     Requires the user invoking the command to have a specified role.
/// </summary>
/// <remarks>
///     This precondition will restrict the access of the command or module to a user with the specified role.
///     If the precondition fails to be met, an erroneous <see cref="PreconditionResult"/> will be returned with the
///     message "Command can only be run by the specified user." For example, you can pass the guild manager role
///     to restrict the command to the guild managers to be able to use it.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireRoleAttribute : PreconditionAttribute
{
    private readonly string? _roleName;
    private readonly uint? _roleId;

    /// <summary>
    ///     Gets or sets the error message if the precondition
    ///     fails due to being run outside a Guild channel.
    /// </summary>
    public string? NotAGuildErrorMessage { get; set; }

    /// <summary>
    ///     Requires that the user invoking the command to have a specific Role.
    /// </summary>
    /// <param name="roleId">Id of the role that the user must have.</param>
    public RequireRoleAttribute(uint roleId)
    {
        _roleId = roleId;
    }

    /// <summary>
    ///     Requires that the user invoking the command to have a specific Role.
    /// </summary>
    /// <param name="roleName">Name of the role that the user must have.</param>
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

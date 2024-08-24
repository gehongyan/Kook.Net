namespace Kook.Commands;

/// <summary>
///     要求调用命令的用户具有指定的 ID。
/// </summary>
/// <example>
///     以下代码示例将 admin 命令组限制为仅允许用户 ID 为 2810246202 的用户可以调用。
///     <code language="cs">
///     [RequireUser(2810246202)]
///     [Group("admin")]
///     public class AdminModule : ModuleBase
///     {
///         [Command("exit")]
///         public async Task ExitAsync()
///         {
///             await ReplyTextAsync("Goodbye!");
///             Environment.Exit(0);
///         }
///     }
///     </code>
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireUserAttribute : PreconditionAttribute
{
    private readonly ulong _userId;

    /// <summary>
    ///     初始化一个 <see cref="RequireUserAttribute"/> 类的新实例。
    /// </summary>
    /// <param name="userId"> 所要求调用命令的用户应具有的 ID。 </param>
    public RequireUserAttribute(ulong userId)
    {
        _userId = userId;
    }

    /// <summary>
    ///     获取或设置错误消息。
    /// </summary>
    public override string? ErrorMessage { get; set; }

    /// <inheritdoc />
    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services) =>
        Task.FromResult(context.User.Id != _userId
            ? PreconditionResult.FromError(ErrorMessage ?? $"Command can only be run by the specified user with identifier {_userId}.")
            : PreconditionResult.FromSuccess());
}

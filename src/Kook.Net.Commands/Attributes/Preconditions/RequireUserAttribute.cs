namespace Kook.Commands;

/// <summary>
///     Requires the command to be invoked by the specified user.
/// </summary>
/// <remarks>
///     This precondition will restrict the access of the command or module to a specified user.
///     If the precondition fails to be met, an erroneous <see cref="PreconditionResult"/> will be returned with the
///     message "Command can only be run by the specified user." For example, you can pass the owner of this bot
///     application to restrict the command to the bot owner to be able to use it.
/// </remarks>
/// <example>
///     The following example restricts the command to a set of sensitive commands that only the specified user
///     should be able to access.
///     <code language="cs">
///     [RequireUser(2810246202)]
///     [Group("admin")]
///     public class AdminModule : ModuleBase
///     {
///         [Command("exit")]
///         public async Task ExitAsync()
///         {
///             Environment.Exit(0);
///         }
///     }
///     </code>
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireUserAttribute : PreconditionAttribute
{
    private readonly ulong _userId;

    /// <summary>
    ///     Initializes a new <see cref="RequireUserAttribute" /> attribute with the specified user identifier.
    /// </summary>
    /// <param name="userId"> The identifier of the user. </param>
    public RequireUserAttribute(ulong userId) => _userId = userId;

    /// <summary>
    ///     Initializes a new <see cref="RequireUserAttribute" /> attribute with the specified user.
    /// </summary>
    /// <param name="user"> An <see cref="IUser"/> representing the user. </param>
    public RequireUserAttribute(IUser user) => _userId = user.Id;

    /// <inheritdoc />
    public override string ErrorMessage { get; set; }

    /// <inheritdoc />
    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services) =>
        Task.FromResult(context.User.Id != _userId
            ? PreconditionResult.FromError(ErrorMessage ?? $"Command can only be run by the specified user with identifier {_userId}.")
            : PreconditionResult.FromSuccess());
}

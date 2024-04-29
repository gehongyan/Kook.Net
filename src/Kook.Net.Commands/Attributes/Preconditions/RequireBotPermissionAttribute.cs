namespace Kook.Commands;

/// <summary>
///     Requires the bot to have a specific permission in the channel a command is invoked in.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireBotPermissionAttribute : PreconditionAttribute
{
    /// <summary>
    ///     Gets the specified <see cref="Kook.GuildPermission" /> of the precondition.
    /// </summary>
    public GuildPermission? GuildPermission { get; }

    /// <summary>
    ///     Gets the specified <see cref="Kook.ChannelPermission" /> of the precondition.
    /// </summary>
    public ChannelPermission? ChannelPermission { get; }

    /// <inheritdoc />
    public override string? ErrorMessage { get; set; }

    /// <summary>
    ///     Gets or sets the error message if the precondition
    ///     fails due to being run outside a Guild channel.
    /// </summary>
    public string? NotAGuildErrorMessage { get; set; }

    /// <summary>
    ///     Requires the bot account to have a specific <see cref="Kook.GuildPermission"/>.
    /// </summary>
    /// <remarks>
    ///     This precondition will always fail if the command is being invoked in a <see cref="IPrivateChannel"/>.
    /// </remarks>
    /// <param name="permission">
    ///     The <see cref="Kook.GuildPermission"/> that the bot must have. Multiple permissions can be specified
    ///     by ORing the permissions together.
    /// </param>
    public RequireBotPermissionAttribute(GuildPermission permission)
    {
        GuildPermission = permission;
        ChannelPermission = null;
    }

    /// <summary>
    ///     Requires that the bot account to have a specific <see cref="Kook.ChannelPermission"/>.
    /// </summary>
    /// <param name="permission">
    ///     The <see cref="Kook.ChannelPermission"/> that the bot must have. Multiple permissions can be
    ///     specified by ORing the permissions together.
    /// </param>
    public RequireBotPermissionAttribute(ChannelPermission permission)
    {
        ChannelPermission = permission;
        GuildPermission = null;
    }

    /// <inheritdoc />
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        IGuildUser? guildUser = null;
        if (context.Guild != null)
            guildUser = await context.Guild.GetCurrentUserAsync().ConfigureAwait(false);

        if (GuildPermission.HasValue)
        {
            if (guildUser == null)
                return PreconditionResult.FromError(NotAGuildErrorMessage ?? "Command must be used in a guild channel.");
            if (!guildUser.GuildPermissions.Has(GuildPermission.Value))
                return PreconditionResult.FromError(ErrorMessage ?? $"Bot requires guild permission {GuildPermission.Value}.");
        }

        if (ChannelPermission.HasValue)
        {
            if (guildUser == null)
                return PreconditionResult.FromError(NotAGuildErrorMessage ?? "Command must be used in a guild channel.");
            ChannelPermissions perms;
            perms = context.Channel is IGuildChannel guildChannel
                ? guildUser.GetPermissions(guildChannel)
                : ChannelPermissions.All(context.Channel);
            if (!perms.Has(ChannelPermission.Value))
                return PreconditionResult.FromError(ErrorMessage ?? $"Bot requires channel permission {ChannelPermission.Value}.");
        }

        return PreconditionResult.FromSuccess();
    }
}

namespace Kook.Commands;

/// <summary>
///     要求当前用户在命令调用所在的的频道或服务器拥有指定的权限。
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireBotPermissionAttribute : PreconditionAttribute
{
    /// <summary>
    ///     获取此先决条件所要求的服务器权限。
    /// </summary>
    public GuildPermission? GuildPermission { get; }

    /// <summary>
    ///     获取此先决条件所要求的频道权限。
    /// </summary>
    public ChannelPermission? ChannelPermission { get; }

    /// <summary>
    ///     获取或设置错误消息。
    /// </summary>
    public override string? ErrorMessage { get; set; }

    /// <summary>
    ///     获取或设置由于在服务器频道外执行命令而导致的先决条件失败的错误消息。
    /// </summary>
    public string? NotAGuildErrorMessage { get; set; }

    /// <summary>
    ///     初始化一个 <see cref="RequireBotPermissionAttribute"/> 类的新实例。
    /// </summary>
    /// <remarks>
    ///     设置此先决条件将导致命令在私有频道中无法使用。
    /// </remarks>
    /// <param name="permission"> 所要求当前用户需要在命令调用所在的的服务器拥有的权限。 </param>
    public RequireBotPermissionAttribute(GuildPermission permission)
    {
        GuildPermission = permission;
        ChannelPermission = null;
    }

    /// <summary>
    ///     初始化一个 <see cref="RequireBotPermissionAttribute"/> 类的新实例。
    /// </summary>
    /// <remarks>
    ///     设置此先决条件将导致命令在私有频道中无法使用。
    /// </remarks>
    /// <param name="permission"> 所要求当前用户需要在命令调用所在的的服务器频道拥有的权限。 </param>
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

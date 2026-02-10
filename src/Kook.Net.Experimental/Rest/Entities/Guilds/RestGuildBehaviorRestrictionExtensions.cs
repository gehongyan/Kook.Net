namespace Kook.Rest;

/// <summary>
///     提供对 <see cref="Kook.Rest.RestGuildBehaviorRestriction"/> 的实验性方法。
/// </summary>
public static class RestGuildBehaviorRestrictionExtensions
{
    /// <inheritdoc cref="Kook.GuildBehaviorRestrictionExtensions.ModifyAsync(Kook.IGuildBehaviorRestriction,System.Action{ModifyGuildBehaviorRestrictionProperties},Kook.RequestOptions)" />
    public static async Task ModifyAsync(this RestGuildBehaviorRestriction restriction,
        Action<ModifyGuildBehaviorRestrictionProperties> func, RequestOptions? options = null)
    {
        await ExperimentalGuildHelper.ModifyBehaviorRestrictionAsync(restriction.Kook, restriction, func, options);
    }
}

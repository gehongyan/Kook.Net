namespace Kook.Rest;

/// <summary>
///     提供对 <see cref="Kook.Rest.RestGuild"/> 的实验性方法。
/// </summary>
public static class RestGuildExperimentalExtensions
{
    /// <inheritdoc cref="Kook.GuildExperimentalExtensions.GetBehaviorRestrictionsAsync(Kook.IGuild,Kook.RequestOptions)" />
    public static async Task<IReadOnlyCollection<RestGuildBehaviorRestriction>> GetBehaviorRestrictionsAsync(
        this RestGuild guild, RequestOptions? options = null) =>
        await ExperimentalClientHelper.GetBehaviorRestrictionsAsync(guild.Kook, guild.Id, options);
}

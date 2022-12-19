namespace Kook.Rest;

public static class RestGuildExperimentalExtensions
{
    public static Task DeleteAsync(this RestGuild guild, RequestOptions options = null)
        => ExperimentalGuildHelper.DeleteAsync(guild, guild.Kook, options);
}

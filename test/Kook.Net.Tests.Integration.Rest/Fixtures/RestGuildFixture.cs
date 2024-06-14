using Kook.Rest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kook;

/// <summary>
///     Gets or creates a guild to use for testing.
/// </summary>
public class RestGuildFixture : KookRestClientFixture
{
    public RestGuild Guild { get; private set; } = null!;

    public RestGuildFixture()
    {
        InitializeAsync().GetAwaiter().GetResult();
    }

    private async Task InitializeAsync()
    {
        const string guildName = "KOOK NET INTEGRATION TEST";
        List<RestGuild> guilds = Client.GetGuildsAsync().GetAwaiter().GetResult()
            .Where(x => x.Name == guildName)
            .ToList();
        await RemoveUselessTestGuildsAsync(guilds.Skip(1));

        // If there is a guild created by this bot, use it
        if (guilds.Find(x => x.OwnerId == Client.CurrentUser?.Id) is { } guild)
            Guild = guild;
        // If there is a guild with the same name, and the bot has admin permissions, use it
        else if (guilds.Count > 0
                 && (await guilds[0].GetCurrentUserAsync())?.GuildPermissions.Has(GuildPermission.Administrator) is true)
            Guild = guilds[0];
        // Otherwise, create a new guild
        else
            Guild = await Client.CreateGuildAsync(guildName);

        await RemoveAllChannelsAsync();
        await RemoveAllRolesAsync();
    }

    private async Task RemoveUselessTestGuildsAsync(IEnumerable<RestGuild> guilds)
    {
        foreach (RestGuild guild in guilds)
            await guild.DeleteAsync();
    }

    /// <summary>
    ///     Removes all channels in the guild.
    /// </summary>
    private async Task RemoveAllChannelsAsync()
    {
        IReadOnlyCollection<RestGuildChannel> channels = Guild.GetChannelsAsync().GetAwaiter().GetResult();
        foreach (RestGuildChannel channel in channels.Where(x => x is INestedChannel))
            await channel.DeleteAsync();
        foreach (RestGuildChannel channel in channels.Where(x => x is ICategoryChannel))
            await channel.DeleteAsync();
    }

    private async Task RemoveAllRolesAsync()
    {
        List<RestRole> roles = Guild.Roles.Where(x => x.Type == RoleType.UserCreated).ToList();
        if (Guild.OwnerId != Client.CurrentUser?.Id)
        {
            RestGuildUser guildUser = await Guild.GetCurrentUserAsync();
            int highestPosition = guildUser.RoleIds
                .Select(x => roles.Find(y => y.Id == x))
                .OfType<RestRole>()
                .Select(x => x.Position)
                .Min();
            roles = guildUser.RoleIds
                .Select(x => roles.Find(y => y.Id == x))
                .OfType<RestRole>()
                .Where(x => x.Position > highestPosition)
                .ToList();
        }
        foreach (RestRole role in roles)
            await role.DeleteAsync();
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        if (Guild.OwnerId == Client.CurrentUser?.Id)
            await Guild.DeleteAsync();
        await base.DisposeAsync();
    }

    /// <inheritdoc />
    public override void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();
}

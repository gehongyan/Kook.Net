using System;
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

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        const string guildName = "KOOK NET INTEGRATION TEST";
        List<RestGuild> guilds = Client.GetGuildsAsync().GetAwaiter().GetResult()
            .Where(x => x.Name == guildName)
            .ToList();

        // If there is a guild created by this bot, use it
        if (guilds.Find(x => x.OwnerId == Client.CurrentUser?.Id) is { } guild)
            Guild = guild;
        // If there is a guild with the same name, and the bot has admin permissions, use it
        else if (guilds.Count > 0
                 && (await guilds[0].GetCurrentUserAsync())?.GuildPermissions.Has(GuildPermission.Administrator) is true)
            Guild = guilds[0];
        // Otherwise, create a new guild
        else
            throw new InvalidOperationException("No guild found to use for testing.");

        await RemoveAllChannelsAsync();
        await RemoveAllRolesAsync();
    }

    /// <summary>
    ///     Removes all channels in the guild.
    /// </summary>
    private async Task RemoveAllChannelsAsync()
    {
        IReadOnlyCollection<RestGuildChannel> channels = await Guild.GetChannelsAsync();
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
}

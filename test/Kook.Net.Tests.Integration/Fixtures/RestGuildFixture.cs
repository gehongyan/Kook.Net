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
    public RestGuild Guild { get; private set; }

    public RestGuildFixture() : base()
    {
        const string guildName = "KOOK NET INTEGRATION TEST";
        List<RestGuild> guilds = Client.GetGuildsAsync().GetAwaiter().GetResult()
            .Where(x => x.OwnerId == Client.CurrentUser?.Id)
            .Where(x => x.Name == guildName)
            .ToList();
        RemoveUselessTestGuilds(guilds.Skip(1));

        Guild = guilds.FirstOrDefault()
            ?? Client.CreateGuildAsync(guildName).GetAwaiter().GetResult();

        RemoveAllChannels();
        RemoveAllRoles();
    }

    private void RemoveUselessTestGuilds(IEnumerable<RestGuild> guilds)
    {
        foreach (RestGuild guild in guilds)
            guild.DeleteAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Removes all channels in the guild.
    /// </summary>
    private void RemoveAllChannels()
    {
        IReadOnlyCollection<RestGuildChannel> channels = Guild.GetChannelsAsync().GetAwaiter().GetResult();
        foreach (RestGuildChannel channel in channels.Where(x => x is INestedChannel)) channel.DeleteAsync().GetAwaiter().GetResult();

        foreach (RestGuildChannel channel in channels.Where(x => x is ICategoryChannel)) channel.DeleteAsync().GetAwaiter().GetResult();
    }

    private void RemoveAllRoles()
    {
        IEnumerable<RestRole> roles = Guild.Roles.Where(r => r.Type == RoleType.UserCreated);
        foreach (RestRole role in roles) role.DeleteAsync().GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        await Guild.DeleteAsync();
        await base.DisposeAsync();
    }

    /// <inheritdoc />
    public override void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();
}

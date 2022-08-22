using KaiHeiLa.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KaiHeiLa;

/// <summary>
///     Gets or creates a guild to use for testing.
/// </summary>
public class RestGuildFixture : KaiHeiLaRestClientFixture
{
    public RestGuild Guild { get; private set; }

    public RestGuildFixture() : base()
    {
        RestGuild guild = Client.GetGuildsAsync().GetAwaiter().GetResult()
            .FirstOrDefault(x => x.Name == "KAIHEILA NET INTEGRATION TEST");

        Guild = guild ?? throw new Exception("No guild found to use for integration tests.");
        RemoveAllChannels();
        RemoveAllRoles();
    }

    /// <summary>
    ///     Removes all channels in the guild.
    /// </summary>
    private void RemoveAllChannels()
    {
        IReadOnlyCollection<RestGuildChannel> channels = Guild.GetChannelsAsync().GetAwaiter().GetResult();
        foreach (RestGuildChannel channel in channels)
        {
            channel.DeleteAsync().GetAwaiter().GetResult();
        }
    }

    private void RemoveAllRoles()
    {
        IEnumerable<RestRole> roles = Guild.Roles.Where(r => r.Type == RoleType.UserCreated);
        foreach (RestRole role in roles)
        {
            role.DeleteAsync().GetAwaiter().GetResult();
        }
    }
}
using Kook.Rest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Kook;

/// <summary>
///     Tests that channels can be created and modified.
/// </summary>
[CollectionDefinition(nameof(RoleTests), DisableParallelization = true)]
[Trait("Category", "Integration.Rest")]
public class RoleTests : IClassFixture<RestGuildFixture>
{
    private readonly IGuild _guild;
    private readonly ITestOutputHelper _output;

    public RoleTests(RestGuildFixture guildFixture, ITestOutputHelper output)
    {
        _guild = guildFixture.Guild;
        _output = output;
        _output.WriteLine($"RestGuildFixture using guild: {_guild.Id}");
        // capture all console output
        guildFixture.Client.Log += LogAsync;
    }

    private Task LogAsync(LogMessage message)
    {
        _output.WriteLine(message.ToString());
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ModifyRoleAsync()
    {
        IRole role = await _guild.CreateRoleAsync("TEST ROLE");
        try
        {
            Assert.NotNull(role);
            // check that it can be modified
            await role.ModifyAsync(x =>
            {
                x.Name = "UPDATED ROLE";
                x.IsHoisted = true;
                x.IsMentionable = true;
                x.Color = Color.Red;
                x.Permissions = GuildPermissions.All;
            });
            // check the results of modifying this channel
            Assert.Equal("UPDATED ROLE", role.Name);
            Assert.True(role.IsHoisted);
            Assert.True(role.IsMentionable);
            Assert.Equal(Color.Red, role.Color);
            Assert.Equal(GuildPermissions.All.RawValue, role.Permissions.RawValue);
        }
        finally
        {
            await role.DeleteAsync();
        }
    }

    [Fact]
    public async Task AddRemoveRoleAsync()
    {
        IRole role = await _guild.CreateRoleAsync("TEST ROLE");
        try
        {
            RestGuildUser? selfUser = await _guild.GetCurrentUserAsync() as RestGuildUser;
            Assert.NotNull(role);
            Assert.NotNull(selfUser);
            // check that the bot can add the role
            Assert.DoesNotContain(role.Id, selfUser.RoleIds);
            await selfUser.AddRoleAsync(role);
            await selfUser.UpdateAsync();
            Assert.Contains(role.Id, selfUser.RoleIds);
            // check that getting the users in the role returns the bot
            IEnumerable<IGuildUser> users = await role.GetUsersAsync().FlattenAsync();
            Assert.Contains(selfUser.Id, users.Select(user => user.Id));
            // check that the bot can remove the role
            await selfUser.RemoveRoleAsync(role);
            await selfUser.UpdateAsync();
            Assert.DoesNotContain(role.Id, selfUser.RoleIds);
        }
        finally
        {
            await role.DeleteAsync();
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kook;

/// <summary>
///     Tests users in guilds
/// </summary>
[CollectionDefinition(nameof(GuildTests), DisableParallelization = true)]
[Trait("Category", "Integration.Rest")]
public class GuildTests : IClassFixture<RestGuildFixture>
{
    private readonly IGuild _guild;
    private readonly ITestOutputHelper _output;

    public GuildTests(RestGuildFixture guildFixture, ITestOutputHelper output)
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
    public async Task GetUsersAsync()
    {
        IGuildUser? currentUser = await _guild.GetCurrentUserAsync();
        Assert.NotNull(currentUser);

        IReadOnlyCollection<IGuildUser> users = await _guild.GetUsersAsync();
        Assert.Single(users, x => x.Id == currentUser.Id);
        IGuildUser self = users.Single(x => x.Id == currentUser.Id);
        Assert.Equal(currentUser, self, (expected, actual) =>
            expected.Nickname == actual.Nickname
            && expected.DisplayName == actual.DisplayName
            && expected.RoleIds.SequenceEqual(actual.RoleIds)
            && expected.Guild == actual.Guild
            && expected.GuildId == actual.GuildId
            && expected.IsMobileVerified == actual.IsMobileVerified
            && expected.JoinedAt == actual.JoinedAt
            && expected.ActiveAt == actual.ActiveAt
            && expected.GuildPermissions.RawValue == actual.GuildPermissions.RawValue);
    }

    [Fact]
    public async Task ModifyNicknameAsync()
    {
        IGuildUser? currentUser = await _guild.GetCurrentUserAsync();
        Assert.NotNull(currentUser);

        await currentUser.ModifyNicknameAsync("UPDATED NICKNAME");
        Assert.Equal("UPDATED NICKNAME", currentUser.Nickname);
        Assert.Equal("UPDATED NICKNAME", (await _guild.GetCurrentUserAsync())?.Nickname);

        await currentUser.ModifyNicknameAsync(null);
        Assert.True(string.IsNullOrEmpty(currentUser.Nickname));
        Assert.True(string.IsNullOrEmpty((await _guild.GetCurrentUserAsync())?.Nickname));
        Assert.Equal(currentUser.Username, currentUser.DisplayName);
        Assert.Equal(currentUser.Username, (await _guild.GetCurrentUserAsync())?.DisplayName);
    }
}

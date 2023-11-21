using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Kook;

/// <summary>
///     Tests users in guilds
/// </summary>
[CollectionDefinition(nameof(GuildTests), DisableParallelization = true)]
[Trait("Category", "Integration")]
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
        IGuildUser currentUser = await _guild.GetCurrentUserAsync();

        IReadOnlyCollection<IGuildUser> users = await _guild.GetUsersAsync();
        users.Should().ContainSingle(x => x.Id == currentUser.Id);
        IGuildUser self = users.Single(x => x.Id == currentUser.Id);
        self.Should().BeEquivalentTo(currentUser, config => config
            // Due to lack of fields from guild user list
            .Excluding(x => x.ActiveClient)
            .Excluding(x => x.HasBuff)
            .Excluding(x => x.HasAnnualBuff)
            .Excluding(x => x.UserTag)
            .Excluding(x => x.Nameplates)
            .Excluding(x => x.IsDenoiseEnabled)
            .Excluding(x => x.IsOwner)
            .Excluding(x => x.IsSystemUser)
            // Due to different domain names
            .Excluding(x => x.Avatar)
            .Excluding(x => x.BuffAvatar)
        );
    }

    [Fact]
    public async Task ModifyNicknameAsync()
    {
        IGuildUser currentUser = await _guild.GetCurrentUserAsync();

        await currentUser.ModifyNicknameAsync("UPDATED NICKNAME");
        currentUser.Nickname.Should().Be("UPDATED NICKNAME");
        (await _guild.GetCurrentUserAsync()).Nickname.Should().Be("UPDATED NICKNAME");

        await currentUser.ModifyNicknameAsync(null);
        currentUser.Nickname.Should().BeNullOrEmpty();
        (await _guild.GetCurrentUserAsync()).Nickname.Should().BeNullOrEmpty();
        currentUser.DisplayName.Should().Be(currentUser.Username);
        (await _guild.GetCurrentUserAsync()).DisplayName.Should().Be(currentUser.Username);
    }
}

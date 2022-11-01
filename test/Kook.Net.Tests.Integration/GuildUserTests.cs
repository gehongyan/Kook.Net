using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Kook;

/// <summary>
///     Tests users in guilds
/// </summary>
[CollectionDefinition(nameof(GuildUserTests), DisableParallelization = true)]
[Trait("Category", "Integration")]
public class GuildUserTests : IClassFixture<RestGuildFixture>
{
    private readonly IGuild _guild;
    private readonly ITestOutputHelper _output;

    public GuildUserTests(RestGuildFixture guildFixture, ITestOutputHelper output)
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
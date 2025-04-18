using Kook.Rest;
using System.Threading.Tasks;

namespace Kook;

/// <summary>
///     Gets or creates a guild to use for testing.
/// </summary>
public class RestChannelFixture : RestGuildFixture
{
    public RestTextChannel TextChannel { get; private set; } = null!;

    public RestVoiceChannel VoiceChannel { get; private set; } = null!;

    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();
        TextChannel = await Guild.CreateTextChannelAsync("TEST TEXT CHANNEL");
        VoiceChannel = await Guild.CreateVoiceChannelAsync("TEST VOICE CHANNEL");
    }

    public override async ValueTask DisposeAsync()
    {
        await TextChannel.DeleteAsync();
        await VoiceChannel.DeleteAsync();
        await base.DisposeAsync();
    }
}

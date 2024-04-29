using Kook.Rest;
using System;
using System.Threading.Tasks;

namespace Kook;

/// <summary>
///     Gets or creates a guild to use for testing.
/// </summary>
public class RestChannelFixture : RestGuildFixture
{
    public RestTextChannel TextChannel { get; private set; }

    public RestVoiceChannel VoiceChannel { get; private set; }

    public RestChannelFixture() : base()
    {
        RestTextChannel textChannel = Guild.CreateTextChannelAsync("TEST TEXT CHANNEL").GetAwaiter().GetResult();
        TextChannel = textChannel
            ?? throw new Exception("Test text channel cannot be created for test.");
        RestVoiceChannel voiceChannel = Guild.CreateVoiceChannelAsync("TEST VOICE CHANNEL").GetAwaiter().GetResult();
        VoiceChannel = voiceChannel
            ?? throw new Exception("Test text channel cannot be created for test.");
    }

    public override async ValueTask DisposeAsync()
    {
        await TextChannel.DeleteAsync();
        await VoiceChannel.DeleteAsync();
        await base.DisposeAsync();
    }

    /// <inheritdoc />
    public override void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();
}

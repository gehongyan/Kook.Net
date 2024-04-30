using Kook.Rest;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Kook;

/// <summary>
///     Gets or creates a guild to use for testing.
/// </summary>
public class RestChannelFixture : RestGuildFixture
{
    public RestTextChannel TextChannel { get; private set; }

    public RestVoiceChannel VoiceChannel { get; private set; }

    public RestChannelFixture()
    {
        InitializeAsync().GetAwaiter().GetResult();
    }

    [MemberNotNull(nameof(TextChannel), nameof(VoiceChannel))]
    private async Task InitializeAsync()
    {
        TextChannel = await Guild.CreateTextChannelAsync("TEST TEXT CHANNEL")
            ?? throw new Exception("Test text channel cannot be created for test.");
        VoiceChannel = await Guild.CreateVoiceChannelAsync("TEST VOICE CHANNEL")
            ?? throw new Exception("Test voice channel cannot be created for test.");
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

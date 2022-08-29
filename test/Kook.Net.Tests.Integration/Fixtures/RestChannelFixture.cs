using Kook.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Kook;

/// <summary>
///     Gets or creates a guild to use for testing.
/// </summary>
public class RestChannelFixture : RestGuildFixture, IDisposable
{
    public RestTextChannel TextChannel { get; private set; }

    public RestChannelFixture() : base()
    {
        RestTextChannel textChannel = Guild.CreateTextChannelAsync("TEST TEXT CHANNEL").GetAwaiter().GetResult();

        TextChannel = textChannel ?? throw new Exception("Test text channel cannot be created for test.");
    }

    public new void Dispose()
    {
        TextChannel.DeleteAsync().GetAwaiter().GetResult();
        base.Dispose();
    }
}
using Kook.API.Rest;
using Kook.Rest.Extensions;
using RichModel = Kook.API.Rest.RichGuild;

namespace Kook.Rest;

internal static class ExperimentalGuildHelper
{
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <c>null</c>.</exception>
    public static async Task<RichModel> ModifyAsync(IGuild guild, BaseKookClient client,
        Action<GuildProperties> func, RequestOptions options)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        GuildProperties args = new();
        func(args);

        ModifyGuildParams apiArgs = new() { GuildId = args.GuildId, EnableOpen = args.EnableOpen };

        if (args.Region is not null)
            apiArgs.RegionId = args.Region.Id;
        else if (!string.IsNullOrWhiteSpace(args.RegionId)) apiArgs.RegionId = args.RegionId;

        if (args.DefaultChannel is not null)
            apiArgs.DefaultChannelId = args.DefaultChannel.Id;
        else if (args.DefaultChannelId is not null) apiArgs.DefaultChannelId = args.DefaultChannelId.Value;

        if (args.WelcomeChannel is not null)
            apiArgs.WelcomeChannelId = args.WelcomeChannel.Id;
        else if (args.WelcomeChannelId is not null) apiArgs.WelcomeChannelId = args.WelcomeChannelId.Value;

        if (args.WidgetChannel is not null)
            apiArgs.WidgetChannelId = args.WidgetChannel.Id;
        else if (args.WidgetChannelId is not null) apiArgs.WidgetChannelId = args.WidgetChannelId.Value;

        return await client.ApiClient.ModifyGuildAsync(guild.Id, apiArgs, options).ConfigureAwait(false);
    }

    public static async Task DeleteAsync(IGuild guild, BaseKookClient client,
        RequestOptions options) =>
        await client.ApiClient.DeleteGuildAsync(guild.Id, options).ConfigureAwait(false);
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Kook.WebSocket;

namespace Kook;

public class SocketGuildFixture : KookSocketClientFixture
{
    private readonly TaskCompletionSource<SocketGuild> _joinedPromise = new();

    public SocketGuild Guild { get; private set; } = null!;

    public SocketGuildFixture()
    {
        InitializeAsync().GetAwaiter().GetResult();
    }

    private async Task InitializeAsync()
    {
        const string guildName = "KOOK NET INTEGRATION TEST";
        // Find any existing guilds created by this bot
        // If there are more than one, delete all but the first one
        List<SocketGuild> guilds = Client.Guilds
            .Where(x => x.Name == guildName)
            .ToList();
        await RemoveUselessTestGuildsAsync(guilds.Skip(1));

        // If there is a guild created by this bot, use it
        if (guilds.Find(x => x.OwnerId == Client.CurrentUser?.Id) is { } guild)
            Guild = guild;
        // If there is a guild with the same name, and the bot has admin permissions, use it
        else if (guilds.FirstOrDefault() is { } existingGuild
                 && existingGuild.CurrentUser?.GuildPermissions.Has(GuildPermission.Administrator) is true)
            Guild = existingGuild;
        // Otherwise, create a new guild
        else
        {
            Client.JoinedGuild += OnJoinedGuild;
            await Client.CreateGuildAsync(guildName);
            Guild = await _joinedPromise.Task.WithTimeout();
            Client.JoinedGuild -= OnJoinedGuild;
        }

        await RemoveAllChannelsAsync();
        await RemoveAllRolesAsync();
    }

    private Task OnJoinedGuild(SocketGuild guild)
    {
        _joinedPromise.SetResult(guild);
        return Task.CompletedTask;
    }

    private async Task RemoveUselessTestGuildsAsync(IEnumerable<SocketGuild> guilds)
    {
        foreach (SocketGuild guild in guilds)
            await guild.DeleteAsync();
    }

    /// <summary>
    ///     Removes all channels in the guild.
    /// </summary>
    private async Task RemoveAllChannelsAsync()
    {
        IReadOnlyCollection<SocketGuildChannel> channels = Guild.Channels;
        foreach (SocketGuildChannel channel in channels.Where(x => x is INestedChannel))
            await channel.DeleteAsync();
        foreach (SocketGuildChannel channel in channels.Where(x => x is ICategoryChannel))
            await channel.DeleteAsync();
    }

    private async Task RemoveAllRolesAsync()
    {
        List<SocketRole> roles = Guild.Roles.Where(r => r.Type == RoleType.UserCreated).ToList();
        if (Client.CurrentUser is { } currentUser
            && Guild.CurrentUser is { } guildUser
            && Guild.OwnerId != currentUser.Id)
        {
            int highestPosition = guildUser.Roles
                .Select(x => roles.Find(y => y.Id == x.Id))
                .OfType<SocketRole>()
                .Select(x => x.Position)
                .Min();
            roles = guildUser.Roles
                .Select(x => roles.Find(y => y.Id == x.Id))
                .OfType<SocketRole>()
                .Where(x => x.Position > highestPosition)
                .ToList();
        }
        foreach (SocketRole role in roles)
            await role.DeleteAsync();
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        if (Guild.OwnerId == Client.CurrentUser?.Id)
            await Guild.DeleteAsync();
        await base.DisposeAsync();
    }

    /// <inheritdoc />
    public override void Dispose() => DisposeAsync().AsTask();
}

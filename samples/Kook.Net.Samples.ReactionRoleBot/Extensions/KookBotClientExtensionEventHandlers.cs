using Kook.WebSocket;

namespace Kook.Net.Samples.ReactionRoleBot.Extensions;

public partial class KookBotClientExtension
{
    private async Task ProcessReactionRoleAdd(Cacheable<IMessage, Guid> message, ISocketMessageChannel channel,
        SocketReaction reaction)
    {
        if (channel.Id != 5770952608991958) return;

        if (!reaction.Emote.Equals(Emoji.Parse(":computer:"))) return;

        SocketRole socketRole = _kookSocketClient.GetGuild(1591057729615250).GetRole(3001653);
        SocketGuildUser socketGuildUser = (SocketGuildUser)reaction.User;

        await socketGuildUser.AddRoleAsync(socketRole);

        SocketDMChannel dmChannel = await socketGuildUser.CreateDMChannelAsync();

        string time = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(8)).ToString("yyyy'/'M'/'d HH:mm:ss");
        CardBuilder builder = new CardBuilder()
            .WithColor(Color.Green)
            .WithSize(CardSize.Large)
            .AddModule(new SectionModuleBuilder()
                .WithText($"已在服务器 `{socketGuildUser.Guild.Name}` 内为您授予角色 `{socketRole.Name}`", true))
            .AddModule(new ContextModuleBuilder().AddElement(new KMarkdownElementBuilder()
                .WithContent($"{_kookSocketClient.CurrentUser.Username} | {time}")));

        await dmChannel.SendCardAsync(builder.Build());

        _logger.Information("{User}#{IdentifyNumber} is granted {RoleName}",
            socketGuildUser.Username, socketGuildUser.IdentifyNumber, socketRole.Name);
    }

    private async Task ProcessReactionRoleRemove(Cacheable<IMessage, Guid> message, ISocketMessageChannel channel,
        SocketReaction reaction)
    {
        if (channel.Id != 5770952608991958) return;

        if (!reaction.Emote.Equals(Emoji.Parse(":computer:"))) return;

        SocketRole socketRole = _kookSocketClient.GetGuild(1591057729615250).GetRole(3001653);
        SocketGuildUser socketGuildUser = (SocketGuildUser)reaction.User;

        await socketGuildUser.RemoveRoleAsync(socketRole);

        SocketDMChannel dmChannel = await socketGuildUser.CreateDMChannelAsync();

        string time = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(8)).ToString("yyyy'/'M'/'d HH:mm:ss");
        CardBuilder builder = new CardBuilder()
            .WithColor(Color.Red)
            .WithSize(CardSize.Large)
            .AddModule(new SectionModuleBuilder()
                .WithText($"已在服务器 `{socketGuildUser.Guild.Name}` 内为您撤销角色 `{socketRole.Name}`", true))
            .AddModule(new ContextModuleBuilder().AddElement(new KMarkdownElementBuilder()
                .WithContent($"{_kookSocketClient.CurrentUser.Username} | {time}")));

        await dmChannel.SendCardAsync(builder.Build());

        _logger.Information("{User}#{IdentifyNumber} is revoked {RoleName}",
            socketGuildUser.Username, socketGuildUser.IdentifyNumber, socketRole.Name);
    }

    private async Task SendReactionCard()
    {
        CardBuilder builder = new CardBuilder()
            .WithTheme(CardTheme.Info)
            .WithSize(CardSize.Large)
            .AddModule(new HeaderModuleBuilder().WithText("互动角色"))
            .AddModule(new SectionModuleBuilder().WithText("点击下方回应以获取/移除角色\n:computer: 开发者", true));
        Cacheable<IUserMessage, Guid> response = await _kookSocketClient.GetGuild(1591057729615250).GetTextChannel(5770952608991958)
            .SendCardAsync(builder.Build()).ConfigureAwait(false);
        await _kookSocketClient.Rest.AddReactionAsync(response.Id, Emoji.Parse(":computer:"));
    }
}

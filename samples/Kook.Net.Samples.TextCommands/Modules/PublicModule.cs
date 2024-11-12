using Kook.Commands;
using Kook.Net.Samples.TextCommands.Services;
using Kook.WebSocket;

namespace Kook.Net.Samples.TextCommands.Modules;

// Modules must be public and inherit from an IModuleBase
public class PublicModule : ModuleBase<SocketCommandContext>
{
    // Dependency Injection will fill this value in for us
    public required PictureService PictureService { get; set; }

    [Command("ping")]
    [Alias("pong", "hello")]
    public Task PingAsync() =>
        ReplyTextAsync("pong!");

    [Command("template", RunMode = RunMode.Async)]
    public async Task TemplateAsync()
    {
        Cacheable<IUserMessage, Guid> msgInfo = await ReplyCardsAsync(62419125, new
        {
            guildname = "KOOK开发者中心",
            username = "开发者",
            roles = new[] { "role1", "role2", "role3" }
        });
        IUserMessage? msg = await msgInfo.GetOrDownloadAsync();
        if (msg is not null)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            await msg.ModifyAsync(x =>
            {
                x.TemplateId = 62419125;
                x.Parameters = new
                {
                    guildname = "KOOK开发者中心111",
                    username = "开发者111",
                    roles = new[] { "role21", "role22", "role23" }
                };
            });
        }
    }

    [Command("cat")]
    public async Task CatAsync()
    {
        // Get a stream containing an image of a cat
        Stream stream = await PictureService.GetCatPictureAsync();
        // Streams must be seeked to their beginning before being uploaded!
        stream.Seek(0, SeekOrigin.Begin);
        await ReplyFileAsync(stream, "cat.png", AttachmentType.Image);
    }

    // Get info on a user, or the user who invoked the command if one is not specified
    [Command("userinfo")]
    public async Task UserInfoAsync(IUser? user = null)
    {
        user ??= Context.User;
        await ReplyTextAsync(user.ToString() ?? user.Username);
    }

    [Command("emoji")]
    public async Task Emoji([Remainder] string? _) =>
        await Context.Message.AddReactionAsync(new Emoji("\uD83D\uDC4C"));

    [Command("image")]
    public async Task Image(Uri image)
    {
        if (Context.Message.MaybeTextImageMixedMessage()
            && image.IsAbsoluteUri)
            await ReplyFileAsync(new FileAttachment(image, "image.png", AttachmentType.Image));
    }

    // Ban a user
    [Command("ban")]
    [RequireContext(ContextType.Guild)]
    // make sure the user invoking the command can ban
    [RequireUserPermission(GuildPermission.BanMembers)]
    // make sure the bot itself can ban
    [RequireBotPermission(GuildPermission.BanMembers)]
    public async Task BanUserAsync(IGuildUser user, [Remainder] string? reason = null)
    {
        await user.Guild.AddBanAsync(user, reason: reason);
        await ReplyTextAsync("ok!");
    }

    // [Remainder] takes the rest of the command's arguments as one argument, rather than splitting every space
    [Command("echo")]
    public Task EchoAsync([Remainder] string text) =>
        // Insert a ZWSP before the text to prevent triggering other bots!
        ReplyTextAsync('\u200B' + text);

    // 'params' will parse space-separated elements into a list
    [Command("list")]
    public Task ListAsync(params string[] objects) =>
        ReplyTextAsync($"You listed: {string.Join("; ", objects)}");

    // Setting a custom ErrorMessage property will help clarify the precondition error
    [Command("guild_only")]
    [RequireContext(ContextType.Guild,
        ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
    public Task GuildOnlyCommand() =>
        ReplyTextAsync("Nothing to see here!");

    [Command("per")]
    public async Task ModifyCategoryPermissions()
    {
        if (Context.Guild is not { } guild) return;
        if (Context.Channel is not IGuildChannel guildChannel) return;
        await guildChannel.AddPermissionOverwriteAsync((IGuildUser)Context.User);
        if (guildChannel is SocketChannel socketChannel)
            await socketChannel.UpdateAsync();
        if (guild.GetChannel(Context.Channel.Id) is { } socketGuildChannel)
        {
            await socketGuildChannel.ModifyPermissionOverwriteAsync((IGuildUser)Context.User,
                permissions => permissions.Modify(
                    viewChannel: PermValue.Allow,
                    sendMessages: PermValue.Deny,
                    attachFiles: PermValue.Allow));
        }
    }
}

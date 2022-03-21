// See https://aka.ms/new-console-template for more information

using KaiHeiLa;
using KaiHeiLa.Rest;
using KaiHeiLa.WebSocket;

string token = Environment.GetEnvironmentVariable("KaiHeiLaDebugToken", EnvironmentVariableTarget.User) 
               ?? throw new ArgumentNullException(nameof(token));
ulong guildId = ulong.Parse(Environment.GetEnvironmentVariable("KaiHeiLaDebugGuild", EnvironmentVariableTarget.User) 
                            ?? throw new ArgumentNullException(nameof(token)));
ulong channelId = ulong.Parse(Environment.GetEnvironmentVariable("KaiHeiLaDebugChannel", EnvironmentVariableTarget.User) 
                              ?? throw new ArgumentNullException(nameof(token)));

KaiHeiLaSocketClient client = new(new KaiHeiLaSocketConfig()
{
    AlwaysDownloadUsers = true
});
client.Log += log =>
{
    Console.WriteLine(log.ToString());
    return Task.CompletedTask;
};
client.MessageReceived += ClientOnMessageReceived;
// client.Ready += ModifyMessageDemo;
client.MessageButtonClicked += async (s, user, arg3, arg4, arg5) =>
{
    // await arg3.AddReactionAsync(Emote.Parse("[:djbigfan:1990044438283387/hvBcVC4nHX03k03k]", TagMode.PlainText));
    // await arg3.AddReactionAsync(Emote.Parse("(emj)djbigfan(emj)[1990044438283387/hvBcVC4nHX03k03k]", TagMode.KMarkdown));
    // await arg3.RemoveReactionAsync(Emote.Parse("[:djbigfan:1990044438283387/hvBcVC4nHX03k03k]", TagMode.PlainText), client.CurrentUser);
    // IEnumerable<IMessage> selectMany = (await client.GetGuild(1990044438283387).GetTextChannel(6286033651700207).GetMessagesAsync(Guid.Parse("ed260ee9-1616-44ec-abff-d5cfcf9903a0"), Direction.Around, 5).ToListAsync()).SelectMany(x => x.ToList());
    // await client.GetUserAsync(0);
    // IReadOnlyCollection<RestMessage> pinnedMessagesAsync = await client.GetGuild(1990044438283387).GetTextChannel(6286033651700207).GetPinnedMessagesAsync();
    // await (user as SocketGuildUser).AddRoleAsync(1681537);
};
client.ReactionRemoved += (cacheable, cacheable1, arg3) =>
{
    return Task.CompletedTask;
};
client.MessageUpdated += async (cacheable, message, arg3) =>
{
    await cacheable.DownloadAsync();
};
async Task ClientOnMessageReceived(SocketMessage msg)
{
    // await msg.ReloadAsync();
    // await CardDemo(msg);
}

async Task CardDemo(SocketMessage message)
{
    if (message.Author.Id == client.CurrentUser.Id) return;
    // SocketGuildUser user = client.GetGuild(guildId).GetUser(0);
    string messageCleanContent = message.CleanContent;

    if (message.Content != "/test") return;

    CardBuilder cardBuilder = new CardBuilder()
        .WithSize(CardSize.Large)
        .AddModule(new HeaderModuleBuilder().WithText("This is header"))
        .AddModule(new DividerModuleBuilder())
        .AddModule(new SectionModuleBuilder().WithText("**This** *is* ~~kmarkdown~~", true))
        .AddModule(new SectionModuleBuilder()
            .WithText(new ParagraphStructBuilder()
                .WithColumnCount(2)
                .AddField(new PlainTextElementBuilder().WithContent("多列文本测试"))
                .AddField(new KMarkdownElementBuilder().WithContent("**昵称**\n白给Doc"))
                .AddField(new KMarkdownElementBuilder().WithContent("**在线时间**\n9:00-21:00"))
                .AddField(new KMarkdownElementBuilder().WithContent("**服务器**\n吐槽中心")))
            .WithAccessory(new ImageElementBuilder()
                .WithSource("https://img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg")
                .WithSize(ImageSize.Small))
            .WithMode(SectionAccessoryMode.Right))
        .AddModule(new SectionModuleBuilder()
            .WithText(new PlainTextElementBuilder().WithContent("您是否认为\"开黑啦\"是最好的语音软件？"))
            .WithAccessory(new ButtonElementBuilder().WithTheme(ButtonTheme.Primary).WithText("完全同意", false))
            .WithMode(SectionAccessoryMode.Right))
        .AddModule(new ContainerModuleBuilder()
            .AddElement(new ImageElementBuilder().WithSource("https://img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg")))
        .AddModule(new ImageGroupModuleBuilder()
            .AddElement(new ImageElementBuilder().WithSource("https://img.kaiheila.cn/assets/2021-01/pWsmcLsPJq08c08c.jpeg"))
            .AddElement(new ImageElementBuilder().WithSource("https://img.kaiheila.cn/assets/2021-01/YIfHfnvxaV0dw0dw.jpg")))
        .AddModule(new ContextModuleBuilder()
            .AddElement(new PlainTextElementBuilder().WithContent("开黑啦气氛组，等你一起来搞事情"))
            .AddElement(new ImageElementBuilder().WithSource("https://img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg"))
            .AddElement(new ImageElementBuilder().WithSource("https://img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg"))
            .AddElement(new ImageElementBuilder().WithSource("https://img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg")))
        .AddModule(new ActionGroupModuleBuilder()
            .AddElement(new ButtonElementBuilder().WithTheme(ButtonTheme.Primary).WithText("确定").WithValue("ok"))
            .AddElement(new ButtonElementBuilder().WithTheme(ButtonTheme.Danger).WithText("取消").WithValue("cancel")))
        .AddModule(new FileModuleBuilder()
            .WithSource("https://img.kaiheila.cn/attachments/2021-01/21/600972b5d0d31.txt")
            .WithTitle("开黑啦介绍.txt"))
        .AddModule(new AudioModuleBuilder()
            .WithSource("https://img.kaiheila.cn/attachments/2021-01/21/600975671b9ab.mp3")
            .WithTitle("命运交响曲")
            .WithCover("https://img.kaiheila.cn/assets/2021-01/rcdqa8fAOO0hs0mc.jpg"))
        .AddModule(new VideoModuleBuilder()
            .WithSource("https://img.kaiheila.cn/attachments/2021-01/20/6008127e8c8de.mp4")
            .WithTitle("有本事别笑"))
        .AddModule(new DividerModuleBuilder())
        .AddModule(new CountdownModuleBuilder().WithMode(CountdownMode.Day).WithEndTime(DateTimeOffset.Now.AddMinutes(1)))
        .AddModule(new CountdownModuleBuilder().WithMode(CountdownMode.Hour).WithEndTime(DateTimeOffset.Now.AddMinutes(1)))
        .AddModule(new CountdownModuleBuilder().WithMode(CountdownMode.Second).WithEndTime(DateTimeOffset.Now.AddMinutes(2)).WithStartTime(DateTimeOffset.Now.AddMinutes(1)));
    
    (Guid MessageId, DateTimeOffset MessageTimestamp) response = await client.GetGuild(guildId)
        .GetTextChannel(channelId)
        .SendCardMessageAsync(cardBuilder.Build(), quote: new Quote(message.Id));
};

async Task ModifyMessageDemo()
{
    await Task.Delay(TimeSpan.FromSeconds(1));
    
    SocketTextChannel channel = client.GetGuild(guildId).GetTextChannel(channelId);
    (Guid MessageId, DateTimeOffset MessageTimestamp) response = await channel
        .SendKMarkdownMessageAsync("BeforeModification");
    await Task.Delay(TimeSpan.FromSeconds(1));
    
    IUserMessage msg = await channel.GetMessageAsync(response.MessageId) as IUserMessage;
    await msg!.ModifyAsync(properties => properties.Content += "\n==========\nModified");
    await Task.Delay(TimeSpan.FromSeconds(1));
    
    await msg.DeleteAsync();
    await Task.Delay(TimeSpan.FromSeconds(1));

    response = await channel.SendCardMessageAsync(new CardBuilder()
        .AddModule(new HeaderModuleBuilder().WithText("Test")).Build());
    await Task.Delay(TimeSpan.FromSeconds(1));
    
    msg = await channel.GetMessageAsync(response.MessageId) as IUserMessage;
    await msg!.ModifyAsync(properties => properties.Cards.Add(new CardBuilder()
        .AddModule(new DividerModuleBuilder())
        .AddModule(new HeaderModuleBuilder().WithText("ModificationHeader")).Build()));
}

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();
await Task.Delay(-1);
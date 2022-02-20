// See https://aka.ms/new-console-template for more information

using KaiHeiLa;
using KaiHeiLa.API;
using KaiHeiLa.WebSocket;

string token = Environment.GetEnvironmentVariable("KaiHeiLaDebugToken", EnvironmentVariableTarget.User) 
         ?? throw new ArgumentNullException(nameof(token));

KaiHeiLaSocketClient client = new(new KaiHeiLaSocketConfig()
{
    AlwaysDownloadUsers = true
});
client.Log += log =>
{
    Console.WriteLine(log.ToString());
    return Task.CompletedTask;
};
client.MessageReceived += async message =>
{
    if (message.Author.Id == client.CurrentUser.Id) return;
    const ulong guildId = 0;
    const ulong channelId = 0;
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
    
    (Guid Messageid, DateTimeOffset MessageTimestamp) response = await client.GetGuild(guildId)
        .GetTextChannel(channelId)
        .SendCardMessageAsync(cardBuilder.Build(), quote: new Quote(message.Id));
};
await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();
await Task.Delay(-1);
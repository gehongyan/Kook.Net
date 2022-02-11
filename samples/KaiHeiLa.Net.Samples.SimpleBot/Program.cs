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
    if (message.Author.Id == client.CurrentUser.Id)
    {
        return;
    }
    const ulong guildId = 0;
    const ulong channelId = 0;
    SocketGuildUser user = client.GetGuild(guildId).GetUser(0);
    (Guid Messageid, DateTimeOffset MessageTimestamp) response = await client.GetGuild(guildId)
            .GetTextChannel(channelId)
            .SendTextMessageAsync("Text", quote: new Quote(message.Id), ephemeralUser: user);
};
await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();
await Task.Delay(-1);
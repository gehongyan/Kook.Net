using Kook;
using Kook.Webhook.HttpListener;

KookHttpListenerWebhookConfig config = new()
{
    VerifyToken = default,
    EncryptKey = default,
    LogLevel = LogSeverity.Debug,
    UriPrefixes =
    [
        "http://localhost:5043/",
        "http://127.0.0.1:5043/"
    ]
};
KookHttpListenerWebhookClient client = new(config);
client.Log += message =>
{
    Console.WriteLine($"Kook.Webhook: {message.Message}");
    return Task.CompletedTask;
};
client.Ready += () =>
{
    Console.WriteLine("Ready!");
    return Task.CompletedTask;
};
client.MessageReceived += (message, author, channel) =>
{
    Console.WriteLine($"Message: {message.Content}");
    return Task.CompletedTask;
};

await client.LoginAsync(TokenType.Bot, string.Empty);
await client.StartAsync();
await Task.Delay(Timeout.Infinite);

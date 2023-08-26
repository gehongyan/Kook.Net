async Task RunAsync(string[] args)
{
    // 从客户端请求实例。
    // 我们在这里首先请求它，因此其目标构造函数将会调用，我们会得到一个客户端的实例。
    var client = _services.GetRequiredService<KookSocketClient>();

    client.Log += async (msg) =>
    {
        await Task.CompletedTask;
        Console.WriteLine(msg);
    }

    await client.LoginAsync(TokenType.Bot, "");
    await client.StartAsync();

    await Task.Delay(Timeout.Infinite);
}

public class Program
{
    private KookSocketClient _client;
	
    public static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {
        _client = new KookSocketClient();
        _client.Log += Log;
        await _client.LoginAsync(TokenType.Bot, 
            Environment.GetEnvironmentVariable("KookToken"));
        await _client.StartAsync();
		
        await Task.Delay(Timeout.Infinite);
    }
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}
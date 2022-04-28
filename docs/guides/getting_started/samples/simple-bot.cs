public class Program
{
    private KaiHeiLaSocketClient _client;
	
    public static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {
        _client = new KaiHeiLaSocketClient();
        _client.Log += Log;
        await _client.LoginAsync(TokenType.Bot, 
            Environment.GetEnvironmentVariable("KaiHeiLaToken"));
        await _client.StartAsync();
		
        await Task.Delay(-1);
    }
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}
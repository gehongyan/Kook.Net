using KaiHeiLa;
using Serilog;
using Serilog.Events;

public class Program
{
    private KaiHeiLaSocketClient _client;
    public static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        _client = new KaiHeiLaSocketClient();

        _client.Log += LogAsync;

        //  将 Token 写入字符串变量，用于 Bot 登录过程的身份认证
        //  这很不安全，尤其是在有公开源代码的情况下，不应该这么做
        var token = "token";

        // 一些其它存储 Token 的方案，如环境变量、文件等
        // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
        // var token = File.ReadAllText("token.txt");
        // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(Timeout.Infinite);
    }
}
using KookSocketClient client = new();
client.Log += LogAsync;

//  将 Token 写入字符串变量，用于 Bot 登录过程的身份认证
//  这很不安全，尤其是在有公开源代码的情况下，不应该这么做
string token = "token";

// 一些其它存储 Token 的方案，如环境变量、文件等
// string token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
// string token = File.ReadAllText("token.txt");
// string token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();

// 阻塞程序直到关闭
await Task.Delay(Timeout.Infinite);

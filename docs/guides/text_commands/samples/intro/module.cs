// 无前缀的模块
public class InfoModule : ModuleBase<SocketCommandContext>
{
    // !say hello world -> hello world
    [Command("say")]
    [Summary("回显消息")]
    public Task SayAsync([Remainder] [Summary("要回显的消息")] string echo)
        => ReplyTextAsync(echo);
		
    // ReplyTextAsync 为 ModuleBase 上的方法
}

// 创建前缀为 'sample' 的模块
[Group("sample")]
public class SampleModule : ModuleBase<SocketCommandContext>
{
    // ~sample square 20 -> 400
    [Command("square")]
    [Summary("计算给定数字的平方")]
    public async Task SquareAsync(
        [Summary("要计算的数字")]) 
        int num)
    {
        // 也可通过命令上下文访问频道属性
        await Context.Channel.SendTextMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
    }

    // !sample userinfo --> foxbot#0282
    // !sample userinfo @Khionu --> Khionu#8708
    // !sample userinfo Khionu#8708 --> Khionu#8708
    // !sample userinfo Khionu --> Khionu#8708
    // !sample userinfo 96642168176807936 --> Khionu#8708
    // !sample whois 96642168176807936 --> Khionu#8708
    [Command("userinfo")]
    [Summary
        ("Returns info about the current user, or the user parameter, if one passed.")]
    [Alias("user", "whois")]
    public async Task UserInfoAsync(
        [Summary("The (optional) user to get info from")]
        SocketUser user = null)
    {
        var userInfo = user ?? Context.Client.CurrentUser;
        await ReplyTextAsync($"{userInfo.Username}#{userInfo.Discriminator}");
    }
}
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
    // !sample square 20 -> 400
    [Command("square")]
    [Summary("计算给定数字的平方")]
    public async Task SquareAsync(
        [Summary("要计算的数字")]) 
        int num)
    {
        // 也可通过命令上下文访问频道属性
        await Context.Channel.SendTextMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
    }

    // !sample userinfo --> 戈小荷#0439
    // !sample userinfo @戈小荷 --> 戈小荷#0439
    // !sample userinfo 戈小荷#0439 --> 戈小荷#0439
    // !sample userinfo 戈小荷 --> 戈小荷#0439
    // !sample userinfo 2810246202 --> 戈小荷#0439
    // !sample whois 2810246202 --> 戈小荷#0439
    [Command("userinfo")]
    [Summary("打印当前用户的信息，或用户参数指定的用户信息")]
    [Alias("user", "whois")]
    public async Task UserInfoAsync(
        [Summary("要获取信息的用户")]
        SocketUser user = null)
    {
        var userInfo = user ?? Context.Client.CurrentUser;
        await ReplyTextAsync($"{userInfo.Username}#{userInfo.IdentifyNumber}");
    }
}
using Kook.Commands;

public class DatabaseModule : ModuleBase<SocketCommandContext>
{
    private readonly ApplicationDbContext _dbContext;

    public SampleModule(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [Command("sample")]
    public async Task Sample()
    {
        // 在命令的业务逻辑中使用所注入的数据库上下文
        var user = _dbContext.Users.SingleOrDefault(x => x.Id == Context.User.Id);
        // ...
    }
}
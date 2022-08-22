using Kook.Commands;

// 在配置依赖注入后，模块需要通过某种方式让类库知道容器为其在执行时注入哪些依赖

// Kook.Net 中支持两种注入方式：构造函数 / 可公共写属性

// 通过构造函数注入
public class DatabaseModule : ModuleBase<SocketCommandContext>
{
    private readonly DatabaseService _database;
    public DatabaseModule(DatabaseService database)
    {
        _database = database;
    }

    [Command("read")]
    public async Task ReadFromDbAsync()
    {
        await ReplyKMarkdownAsync(_database.GetData());
    }
}

// 通过可公共写属性注入
public class DatabaseModule : ModuleBase<SocketCommandContext>
{
    public DatabaseService DbService { get; set; }

    [Command("read")]
    public async Task ReadFromDbAsync()
    {
        await ReplyKMarkdownAsync(DbService.GetData());
    }
}
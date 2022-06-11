private static ServiceProvider ConfigureServices()
{
    return new ServiceCollection()
        .AddDbContext<ApplicationDbContext>(
            optionsBuilder => optionsBuilder.UseSqlServer("数据库连接字符串")
        )
        // ...
        .BuildServiceProvider();
}
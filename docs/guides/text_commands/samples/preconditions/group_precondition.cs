// 该示例展示了先决条件的析取的声明方式
// 将 RequireUserPermission 与 RequireUser 中的 Group 属性都设置为 "Permission"
// 则该命令调用者在当前服务器内拥有管理员权限或其用户 ID 为 2810246202 时都可以通过先决条件检查
[RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
[RequireUser(2810246202, Group = "Permission")]
public class AdminModule : ModuleBase<SocketCommandContext>
{
    [Command("ban")]
    public Task BanAsync(IUser user) => Context.Guild.AddBanAsync(user);
}
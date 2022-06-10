using System;
using System.Linq;
using System.Threading.Tasks;
using KaiHeiLa.Commands;
using KaiHeiLa.WebSocket;

// 继承自 PreconditionAttribute
public class RequireRoleAttribute : PreconditionAttribute
{
    // 创建字段存储指定的角色名称
    private readonly string _name;

    // 创建构造函数，用来在使用时接收角色名称作为参数
    public RequireRoleAttribute(string name) => _name = name;

    // 重写 CheckPermissions 方法
    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        // 检查该用户是否为服务器内的用户，只有服务器内的用户存在角色
        if (context.User is SocketGuildUser guildUser)
        {
            // 如果该用户存在预先指定的角色，则返回成功
            if (guildUser.Roles.Any(r => r.Name == _name))
                // 由于方法内没有异步代码，要返回的结果需要包装在 `Task.FromResult` 中来避免编译错误
                return Task.FromResult(PreconditionResult.FromSuccess());
            // 角色检查不通过，则返回失败及错误信息
            else
                return Task.FromResult(PreconditionResult.FromError($"You must have a role named {_name} to run this command."));
        }
        else
            return Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command."));
    }
}
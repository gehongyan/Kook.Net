using System;
using System.Linq;
using System.Threading.Tasks;
using Kook.Commands;
using Kook.WebSocket;

// 继承自 PreconditionAttribute
public class RequireTimeAttribute : PreconditionAttribute
{
    // 创建字段存储指定的时间范围
    private readonly TimeOnly _from;
    private readonly TimeOnly _to;

    // 创建构造函数，用来在使用时接收时间范围作为参数
    public RequireTimeAttribute(TimeOnly from, TimeOnly to)
    {
        _from = from;
        _to = to;
    }

    // 重写 CheckPermissions 方法
    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        // 检查当前时间是否在指定的时间范围内
        if ((_from < _to && TimeOnly.FromDateTime(DateTime.Now) >= _from && TimeOnly.FromDateTime(DateTime.Now) <= _to)
            || (_from > _to && (TimeOnly.FromDateTime(DateTime.Now) >= _from || TimeOnly.FromDateTime(DateTime.Now) <= _to)))
            // 如果在指定时间范围内，则返回成功
            // 由于方法内没有异步代码，要返回的结果需要包装在 `Task.FromResult` 中来避免编译错误
            return Task.FromResult(PreconditionResult.FromSuccess());
        // 如果不在指定时间范围内，则返回失败及错误信息
        return Task.FromResult(PreconditionResult.FromError($"You can only run this command between {_from} and {_to}"));
    }
}

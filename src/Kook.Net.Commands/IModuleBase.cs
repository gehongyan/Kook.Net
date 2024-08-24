using Kook.Commands.Builders;

namespace Kook.Commands;

/// <summary>
///     表示一个通用的模块基类。
/// </summary>
public interface IModuleBase
{
    /// <summary>
    ///     设置此模块基类的上下文。
    /// </summary>
    /// <param name="context"> 此模块基类的上下文。 </param>
    void SetContext(ICommandContext context);

    /// <summary>
    ///     当此模块中的命令在运行之前异步执行。
    /// </summary>
    /// <param name="command"> 即将运行的命令。 </param>
    /// <returns> 一个表示异步操作的任务。 </returns>
    Task BeforeExecuteAsync(CommandInfo command);

    /// <summary>
    ///     当此模块中的命令在运行之前执行。
    /// </summary>
    /// <param name="command"> 即将运行的命令。 </param>
    void BeforeExecute(CommandInfo command);

    /// <summary>
    ///     当此模块中的命令运行后执行。
    /// </summary>
    /// <param name="command"> 执行的命令。 </param>
    void AfterExecute(CommandInfo command);

    /// <summary>
    ///     当此模块中的命令运行后异步执行。
    /// </summary>
    /// <param name="command"> 执行的命令。 </param>
    /// <returns> 一个表示异步操作的任务。 </returns>
    Task AfterExecuteAsync(CommandInfo command);

    /// <summary>
    ///     当构建此模块时执行。
    /// </summary>
    /// <param name="commandService"> 创建此模块的命令服务。 </param>
    /// <param name="builder"> 用于构建此模块的构建器。 </param>
    void OnModuleBuilding(CommandService commandService, ModuleBuilder builder);
}

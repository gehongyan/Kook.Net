namespace Kook.Commands;

/// <summary>
///     表示一个命令中的参数在其所属命令执行前需要符合的先决条件的基类。
/// </summary>
/// <seealso cref="Kook.Commands.PreconditionAttribute"/>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
public abstract class ParameterPreconditionAttribute : Attribute
{
    /// <summary>
    ///     检查命令执行上下文中的此参数是否满足此先决条件。
    /// </summary>
    /// <param name="context"> 命令执行上下文。 </param>
    /// <param name="parameter"> 要检查的参数信息。 </param>
    /// <param name="value"> 参数的原始值。 </param>
    /// <param name="services"> 依赖注入服务容器。 </param>
    /// <returns> 一个表示异步检查操作的任务。任务的结果包含先决条件的结果。 </returns>
    public abstract Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
        ParameterInfo parameter, object? value, IServiceProvider services);
}

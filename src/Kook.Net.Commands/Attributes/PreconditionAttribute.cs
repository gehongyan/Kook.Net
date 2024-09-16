namespace Kook.Commands;

/// <summary>
///     表示一个模块或类中的命令在执行前需要符合的先决条件的基类。
/// </summary>
/// <seealso cref="Kook.Commands.ParameterPreconditionAttribute"/>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public abstract class PreconditionAttribute : Attribute
{
    /// <summary>
    ///     获取或设置此先决条件所属的分组。
    /// </summary>
    /// <remarks>
    ///     此属性所设置的先决条件分组用于先决条件的析取。
    ///     同属于相同分组的所有先决条件中，存在一个满足条件的先决条件，则该组先决条件满足条件，组内的其它先决条件将被忽略（A || B）。
    ///     不设置此属性，或设置为 <c>null</c>，则表示该先决条件不属于任何分组，与其它的无分组先决条件或先决条件组的结果合取（A &amp;&amp; B）。
    /// </remarks>
    public string? Group { get; set; }

    /// <summary>
    ///     获取或设置错误消息。
    /// </summary>
    /// <remarks>
    ///     当在派生类中重写此虚属性时，用户代码设置在此属性的值将用于在报告错误时携带的错误，在未重写此属性的派生类中设置此属性为空操作。
    /// </remarks>
    public virtual string? ErrorMessage
    {
        get => null;
        set { }
    }

    /// <summary>
    ///     检查命令执行上下文中的此命令是否满足此先决条件。
    /// </summary>
    /// <param name="context"> 命令执行上下文。 </param>
    /// <param name="command"> 要执行的命令。 </param>
    /// <param name="services"> 依赖注入服务容器。 </param>
    /// <returns> 一个表示异步检查操作的任务。任务的结果包含先决条件的结果。 </returns>
    public abstract Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services);
}

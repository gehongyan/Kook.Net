namespace Kook.Commands;

/// <summary>
///     标记指定的命令的优先级。
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class PriorityAttribute : Attribute
{
    /// <summary>
    ///     获取为命令设置的优先级。
    /// </summary>
    public int Priority { get; }

    /// <summary>
    ///     初始化一个 <see cref="PriorityAttribute"/> 类的新实例。
    /// </summary>
    /// <param name="priority"> 为命令设置的优先级。 </param>
    /// <remarks>
    ///     更大的数值表示更高的优先级。 <br />
    ///     命令系统在命令匹配时，会根据输入参数的相似度和优先级来选择最匹配的命令。
    ///     例如，通过提及用户传入用户类型的参数，可以为该参数贡献 1.0 的匹配相似分，而仅通过用户昵称传入用户类型的参数则最低只贡献 0.5 的匹配相似分。
    ///     一般地，各个参数的匹配相似分均为不超过 1.0 的正数，命令系统将会对各个匹配的匹配相似分统计算术平均值，此平均值通常不会超过 1.0。
    ///     在此基础上，优先级分数值会被累加到匹配相似分算术平均值的 0.99 倍之上，这表示，通常情况下，为命令之间设置量级超过 1 的优先级均可以使自定义匹配优先级生效。
    /// </remarks>
    public PriorityAttribute(int priority)
    {
        Priority = priority;
    }
}

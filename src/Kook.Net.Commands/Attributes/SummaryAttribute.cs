namespace Kook.Commands;

/// <summary>
///     标记命令的摘要。
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter)]
public class SummaryAttribute : Attribute
{
    /// <summary>
    ///     获取命令的摘要。
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///     初始化一个 <see cref="SummaryAttribute"/> 类的新实例。
    /// </summary>
    /// <param name="text"> 命令的摘要。 </param>
    public SummaryAttribute(string text)
    {
        Text = text;
    }
}

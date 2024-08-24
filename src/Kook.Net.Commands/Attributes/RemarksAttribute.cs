namespace Kook.Commands;

/// <summary>
///     标记命令的备注。
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RemarksAttribute : Attribute
{
    /// <summary>
    ///     获取命令的备注。
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///     初始化一个 <see cref="RemarksAttribute"/> 类的新实例。
    /// </summary>
    /// <param name="text"> 命令的备注。 </param>
    public RemarksAttribute(string text)
    {
        Text = text;
    }
}

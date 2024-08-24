namespace Kook.Commands;

/// <summary>
///     标记命令、模块或参数的基本名称。
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter)]
public class NameAttribute : Attribute
{
    /// <summary>
    ///     获取基本名称。
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///     标记命令、模块或参数的基本名称。
    /// </summary>
    /// <param name="text"> 基本名称。 </param>
    public NameAttribute(string text)
    {
        Text = text;
    }
}

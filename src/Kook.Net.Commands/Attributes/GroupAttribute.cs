namespace Kook.Commands;

/// <summary>
///     标记模块为命令组。
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class GroupAttribute : Attribute
{
    /// <summary>
    ///     获取模块组命令的前缀。
    /// </summary>
    public string? Prefix { get; }

    /// <summary>
    ///     初始化一个 <see cref="GroupAttribute"/> 类的新实例。
    /// </summary>
    public GroupAttribute()
    {
        Prefix = null;
    }

    /// <summary>
    ///     初始化一个 <see cref="GroupAttribute"/> 类的新实例。
    /// </summary>
    /// <param name="prefix"> 模块组命令的前缀。 </param>
    public GroupAttribute(string prefix)
    {
        Prefix = prefix;
    }
}

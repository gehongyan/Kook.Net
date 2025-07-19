namespace Kook.Commands;

/// <summary>
///     标记指定的参数将接收剩余未解析的所有输入值。
/// </summary>
/// <remarks>
///     此特性仅可以标记在命令方法的最后一个参数上。
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter)]
public class RemainderAttribute : Attribute;

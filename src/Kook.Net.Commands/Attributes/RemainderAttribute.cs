namespace Kook.Commands;

/// <summary>
///     标记指定的参数将接收剩余未解析的所有输入值。
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class RemainderAttribute : Attribute;

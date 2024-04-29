namespace Kook.Commands;

/// <summary>
///     Marks the input to not be parsed by the parser.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class RemainderAttribute : Attribute;

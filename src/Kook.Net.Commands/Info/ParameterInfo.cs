using Kook.Commands.Builders;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook.Commands;

/// <summary>
///     表示一个参数的信息。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ParameterInfo
{
    private readonly TypeReader _reader;

    /// <summary>
    ///     获取此参数所属的命令。
    /// </summary>
    public CommandInfo Command { get; }

    /// <summary>
    ///     获取此参数的名称。
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     获取此参数的摘要。
    /// </summary>
    public string? Summary { get; }

    /// <summary>
    ///     获取此参数是否为可选参数。
    /// </summary>
    public bool IsOptional { get; }

    /// <summary>
    ///     获取此参数是否接收全部剩余参数。
    /// </summary>
    public bool IsRemainder { get; }

    /// <summary>
    ///     获取此参数是否为多值参数。
    /// </summary>
    public bool IsMultiple { get; }

    /// <summary>
    ///     获取此参数的类型。
    /// </summary>
    public Type? Type { get; }

    /// <summary>
    ///     获取此参数的默认值。
    /// </summary>
    public object? DefaultValue { get; }

    /// <summary>
    ///     获取此参数的所有先决条件。
    /// </summary>
    public IReadOnlyList<ParameterPreconditionAttribute> Preconditions { get; }

    /// <summary>
    ///     获取此参数的所有特性。
    /// </summary>
    public IReadOnlyList<Attribute> Attributes { get; }

    internal ParameterInfo(ParameterBuilder builder, CommandInfo command, CommandService service)
    {
        Command = command;

        Name = builder.Name;
        Summary = builder.Summary;
        IsOptional = builder.IsOptional;
        IsRemainder = builder.IsRemainder;
        IsMultiple = builder.IsMultiple;

        Type = builder.ParameterType;
        DefaultValue = builder.DefaultValue;

        Preconditions = builder.Preconditions.ToImmutableArray();
        Attributes = builder.Attributes.ToImmutableArray();

        _reader = builder.TypeReader!; // TypeReader is always set before building
    }

    /// <summary>
    ///     检查此参数是否满足其先决条件。
    /// </summary>
    /// <param name="context"> 命令的上下文。 </param>
    /// <param name="arg"> 参数的值。 </param>
    /// <param name="services"> 用于解析服务的服务提供程序。 </param>
    /// <returns> 一个表示异步检查操作的任务。任务的结果包含先决条件的结果。 </returns>
    public async Task<PreconditionResult> CheckPreconditionsAsync(ICommandContext context,
        object? arg, IServiceProvider? services = null)
    {
        services ??= EmptyServiceProvider.Instance;
        foreach (ParameterPreconditionAttribute precondition in Preconditions)
        {
            PreconditionResult result = await precondition
                .CheckPermissionsAsync(context, this, arg, services)
                .ConfigureAwait(false);
            if (!result.IsSuccess) return result;
        }
        return PreconditionResult.FromSuccess();
    }

    /// <summary>
    ///     解析字符串输入为此参数目标类型的值。
    /// </summary>
    /// <param name="context"> 命令的上下文。 </param>
    /// <param name="input"> 要解析的字符串输入。 </param>
    /// <param name="services"> 用于解析服务的服务提供程序。 </param>
    /// <returns> 一个表示异步解析操作的任务。任务的结果包含参数解析的结果。 </returns>
    public async Task<TypeReaderResult> ParseAsync(ICommandContext context,
        string input, IServiceProvider? services = null)
    {
        services ??= EmptyServiceProvider.Instance;
        return await _reader.ReadAsync(context, input, services).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override string ToString() => Name;

    private string DebuggerDisplay => $"{Name}{(IsOptional ? " (Optional)" : "")}{(IsRemainder ? " (Remainder)" : "")}";
}

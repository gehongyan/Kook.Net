namespace Kook.Commands;

/// <summary>
///     指示命令系统将此类型的命令参数视为与其属性匹配的命名参数集合。
/// </summary>
/// <example>
///     以下示例代码中，<c>ArgumentType</c> 中的各个属性将被视为所支持的命名参数。
///     <code language="cs">
///     [NamedArgumentType]
///     public sealed class ArgumentType
///     {
///         public int? Foo { get; set; }
///         public string? Bar { get; set; }
///         public IEnumerable&lt;int&gt;? ManyInts { get; set; }
///     }
///     </code>
///     命令的参数中可以定义 ArgumentType 类型的参数。
///     <code language="cs">
///     [Command("test")]
///     public Task TestCommand(ArgumentType arg) =&gt; Task.CompletedTask;
///     </code>
///     则可以以以下的命名参数的形式传递参数，调用命令：
///     <code>
///     foo: 3 bar: hello manyints: "1, 2, 3, 4, 5, 6, 7"
///     </code>
/// </example>
[AttributeUsage(AttributeTargets.Class)]
public sealed class NamedArgumentTypeAttribute : Attribute;

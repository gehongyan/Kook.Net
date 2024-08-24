namespace Kook.Commands;

/// <summary>
///     标记指定的成员不应被注入到其父模块。
/// </summary>
/// <remarks>
///     此特性阻止被标记的成员被注入到其父模块。当存在一个公共属性但不希望为该属性自动注入服务时，请标记此特性。
/// </remarks>
/// <example>
///     以下示例代码中，<c>DatabaseService</c> 将不会自动注入服务，如果依赖项无法解析，也不会抛出错误消息。
///     <code language="cs">
///     public class MyModule : ModuleBase
///     {
///         [DontInject]
///         public DatabaseService DatabaseService { get; }
///         public MyModule()
///         {
///             DatabaseService = DatabaseFactory.Generate();
///         }
///     }
///     </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public class DontInjectAttribute : Attribute;

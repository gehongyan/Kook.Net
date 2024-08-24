namespace Kook.Commands;

/// <summary>
///     标记指定的模块不应被自动加载。
/// </summary>
/// <remarks>
///     此属性告诉 <see cref="T:Kook.Commands.CommandService" /> 在自动加载模块是忽略被此特性标记的模块。
///     需要注意的是，如果尝试手动加载被此特性标记的非公共模块，加载过程也会失败。
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class DontAutoLoadAttribute : Attribute;

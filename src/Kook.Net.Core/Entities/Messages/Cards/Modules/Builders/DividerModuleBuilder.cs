namespace Kook;

/// <summary>
///     用来构建 <see cref="DividerModule"/> 模块的构建器。
/// </summary>
public record DividerModuleBuilder : IModuleBuilder
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Divider;

    /// <summary>
    ///     初始化一个 <see cref="DividerModuleBuilder"/> 类的新实例。
    /// </summary>
    public DividerModuleBuilder()
    {
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="DividerModule"/> 对象。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="DividerModule"/> 对象。 </returns>
    public DividerModule Build() => new();

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}

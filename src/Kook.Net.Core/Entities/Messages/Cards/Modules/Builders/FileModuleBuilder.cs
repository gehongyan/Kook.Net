using System.Diagnostics.CodeAnalysis;
using Kook.Utils;

namespace Kook;

/// <summary>
///     用来构建 <see cref="FileModule"/> 模块的构建器。
/// </summary>
public class FileModuleBuilder : IModuleBuilder, IEquatable<FileModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.File;

    /// <summary>
    ///     初始化一个 <see cref="FileModuleBuilder"/> 类的新实例。
    /// </summary>
    public FileModuleBuilder()
    {
    }

    /// <summary>
    ///     初始化一个 <see cref="DividerModuleBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="source"> 文件的 URL。 </param>
    /// <param name="title"> 文件名。 </param>
    public FileModuleBuilder(string source, string? title = null)
    {
        Source = source;
        Title = title;
    }

    /// <summary>
    ///     获取或设置文件的 URL。
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    ///     获取或设置文件名。
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    ///     设置文件的 URL。
    /// </summary>
    /// <param name="source"> 文件的 URL。 </param>
    /// <returns> 当前构建器。 </returns>
    public FileModuleBuilder WithSource(string? source)
    {
        Source = source;
        return this;
    }

    /// <summary>
    ///     设置文件名。
    /// </summary>
    /// <param name="title"> 文件名。 </param>
    /// <returns> 当前构建器。 </returns>
    public FileModuleBuilder WithTitle(string title)
    {
        Title = title;
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="FileModule"/> 对象。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="FileModule"/> 对象。 </returns>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Source"/> 为 <c>null</c>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Source"/> 为空字符串。
    /// </exception>
    /// <exception cref="UriFormatException">
    ///     <see cref="Source"/> 不是有效的 URL。
    /// </exception>
    [MemberNotNull(nameof(Source))]
    public FileModule Build()
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source), "The source url cannot be null or empty.");
        if (string.IsNullOrEmpty(Source))
            throw new ArgumentException("The source url cannot be null or empty.", nameof(Source));

        UrlValidation.Validate(Source);

        return new FileModule(Source, Title);
    }

    /// <inheritdoc />
    [MemberNotNull(nameof(Source))]
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     判定两个 <see cref="FileModuleBuilder"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="FileModuleBuilder"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(FileModuleBuilder? left, FileModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="FileModuleBuilder"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="FileModuleBuilder"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(FileModuleBuilder? left, FileModuleBuilder? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is FileModuleBuilder builder && Equals(builder);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] FileModuleBuilder? fileModuleBuilder)
    {
        if (fileModuleBuilder is null) return false;

        return Type == fileModuleBuilder.Type
            && Source == fileModuleBuilder.Source
            && Title == fileModuleBuilder.Title;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IModuleBuilder>.Equals([NotNullWhen(true)] IModuleBuilder? moduleBuilder) =>
        Equals(moduleBuilder as FileModuleBuilder);
}

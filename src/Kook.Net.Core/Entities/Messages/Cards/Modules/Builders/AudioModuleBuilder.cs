using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     用来构建 <see cref="AudioModuleBuilder"/> 模块的构建器。
/// </summary>
public class AudioModuleBuilder : IModuleBuilder, IEquatable<AudioModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Audio;

    /// <summary>
    ///     初始化一个 <see cref="AudioModuleBuilder"/> 类的新实例。
    /// </summary>
    public AudioModuleBuilder()
    {
    }

    /// <summary>
    ///     初始化一个 <see cref="AudioModuleBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="source"> 音频的 URL。 </param>
    /// <param name="cover"> 音频的封面的 URL。 </param>
    /// <param name="title"> 音频的标题。 </param>
    public AudioModuleBuilder(string source, string? cover = null, string? title = null)
    {
        Source = source;
        Cover = cover;
        Title = title;
    }

    /// <summary>
    ///     获取或设置音频的 URL。
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    ///     获取或设置音频的封面的 URL。
    /// </summary>
    public string? Cover { get; set; }

    /// <summary>
    ///     获取或设置音频的标题。
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    ///     设置音频的 URL。
    /// </summary>
    /// <param name="source"> 音频的 URL。 </param>
    /// <returns> 当前构建器。 </returns>
    public AudioModuleBuilder WithSource(string? source)
    {
        Source = source;
        return this;
    }

    /// <summary>
    ///     设置音频的封面的 URL。
    /// </summary>
    /// <param name="cover"> 音频的封面的 URL。 </param>
    /// <returns> 当前构建器。 </returns>
    public AudioModuleBuilder WithCover(string? cover)
    {
        Cover = cover;
        return this;
    }

    /// <summary>
    ///     设置音频的标题。
    /// </summary>
    /// <param name="title"> 音频的标题。 </param>
    /// <returns> 当前构建器。 </returns>
    public AudioModuleBuilder WithTitle(string? title)
    {
        Title = title;
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="AudioModule"/> 对象。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="AudioModule"/> 对象。 </returns>
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
    public AudioModule Build()
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source), "The source url cannot be null.");
        if (string.IsNullOrEmpty(Source))
            throw new ArgumentException("The source url cannot be empty.", nameof(Source));
        UrlValidation.Validate(Source);
        if (Cover != null)
            UrlValidation.Validate(Cover);
        return new AudioModule(Source, Title, Cover);
    }

    /// <inheritdoc />
    [MemberNotNull(nameof(Source))]
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     判定两个 <see cref="AudioModuleBuilder"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="AudioModuleBuilder"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(AudioModuleBuilder? left, AudioModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="AudioModuleBuilder"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="AudioModuleBuilder"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(AudioModuleBuilder? left, AudioModuleBuilder? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is AudioModuleBuilder builder && Equals(builder);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] AudioModuleBuilder? audioModuleBuilder)
    {
        if (audioModuleBuilder is null)
            return false;

        return Type == audioModuleBuilder.Type
            && Source == audioModuleBuilder.Source
            && Title == audioModuleBuilder.Title
            && Cover == audioModuleBuilder.Cover;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IModuleBuilder>.Equals([NotNullWhen(true)] IModuleBuilder? moduleBuilder) =>
        Equals(moduleBuilder as AudioModuleBuilder);
}

using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     用来构建 <see cref="SectionModule"/> 模块的构建器。
/// </summary>
public class VideoModuleBuilder : IModuleBuilder, IEquatable<VideoModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Video;

    /// <summary>
    ///     初始化一个 <see cref="VideoModuleBuilder"/> 类的新实例。
    /// </summary>
    public VideoModuleBuilder()
    {
    }

    /// <summary>
    ///     初始化一个 <see cref="VideoModuleBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="source"> 视频的 URL。 </param>
    /// <param name="title"> 视频标题。 </param>
    public VideoModuleBuilder(string source, string? title = null)
    {
        Source = source;
        Title = title;
    }

    /// <summary>
    ///     获取或设置视频的 URL。
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    ///     获取或设置视频标题。
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    ///     设置视频的 URL。
    /// </summary>
    /// <param name="source"> 视频的 URL。 </param>
    /// <returns> 当前构建器。 </returns>
    public VideoModuleBuilder WithSource(string? source)
    {
        Source = source;
        return this;
    }

    /// <summary>
    ///     设置视频标题。
    /// </summary>
    /// <param name="title"> 视频标题。 </param>
    /// <returns> 当前构建器。 </returns>
    public VideoModuleBuilder WithTitle(string title)
    {
        Title = title;
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="VideoModule"/> 对象。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="VideoModule"/> 对象。 </returns>
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
    public VideoModule Build()
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source), "The source url cannot be null or empty.");
        if (string.IsNullOrEmpty(Source))
            throw new ArgumentException("The source url cannot be null or empty.", nameof(Source));

        UrlValidation.Validate(Source);

        return new VideoModule(Source, Title);
    }

    /// <inheritdoc />
    [MemberNotNull(nameof(Source))]
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     判定两个 <see cref="VideoModuleBuilder"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="VideoModuleBuilder"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(VideoModuleBuilder? left, VideoModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="VideoModuleBuilder"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="VideoModuleBuilder"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(VideoModuleBuilder? left, VideoModuleBuilder? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is VideoModuleBuilder builder && Equals(builder);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] VideoModuleBuilder? videoModuleBuilder)
    {
        if (videoModuleBuilder is null) return false;

        return Type == videoModuleBuilder.Type
            && Source == videoModuleBuilder.Source
            && Title == videoModuleBuilder.Title;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IModuleBuilder>.Equals([NotNullWhen(true)] IModuleBuilder? moduleBuilder) =>
        Equals(moduleBuilder as VideoModuleBuilder);
}

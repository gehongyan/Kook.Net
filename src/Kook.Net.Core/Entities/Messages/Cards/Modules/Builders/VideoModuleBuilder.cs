using System.Diagnostics.CodeAnalysis;
using Kook.Utils;

namespace Kook;

/// <summary>
///     Represents a video module builder for creating a <see cref="VideoModule"/>.
/// </summary>
public class VideoModuleBuilder : IModuleBuilder, IEquatable<VideoModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Video;

    /// <summary>
    ///     Initializes a new instance of the <see cref="VideoModuleBuilder"/> class.
    /// </summary>
    public VideoModuleBuilder()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="VideoModuleBuilder"/> class.
    /// </summary>
    /// <param name="source"> The source URL of the video. </param>
    /// <param name="title"> The title of the video. </param>
    public VideoModuleBuilder(string source, string? title = null)
    {
        Source = source;
        Title = title;
    }

    /// <summary>
    ///     Gets or sets the source URL of the video.
    /// </summary>
    /// <returns>
    ///     The source URL of the video.
    /// </returns>
    public string? Source { get; set; }

    /// <summary>
    ///     Gets or sets the title of the video.
    /// </summary>
    /// <returns>
    ///     The title of the video.
    /// </returns>
    public string? Title { get; set; }

    /// <summary>
    ///     Sets the source URL of the video.
    /// </summary>
    /// <param name="source">
    ///     The source URL of the video to be set.
    /// </param>
    /// <returns> 当前构建器。 </returns>
    public VideoModuleBuilder WithSource(string? source)
    {
        Source = source;
        return this;
    }

    /// <summary>
    ///     Sets the title of the video.
    /// </summary>
    /// <param name="title">
    ///     The title of the video to be set.
    /// </param>
    /// <returns> 当前构建器。 </returns>
    public VideoModuleBuilder WithTitle(string title)
    {
        Title = title;
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="VideoModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="VideoModule"/> representing the built video module object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     The <see cref="Source"/> url is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     The <see cref="Source"/> url is empty.
    /// </exception>
    /// <exception cref="UriFormatException">
    ///     The <see cref="Source"/> url does not include a protocol (either HTTP or HTTPS).
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

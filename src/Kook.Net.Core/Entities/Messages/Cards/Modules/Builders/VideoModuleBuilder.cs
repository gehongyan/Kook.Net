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
    /// <returns>
    ///     The current builder.
    /// </returns>
    public VideoModuleBuilder WithSource(string source)
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
    /// <returns>
    ///     The current builder.
    /// </returns>
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
    /// <exception cref="InvalidOperationException">
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
    ///     Determines whether the specified <see cref="VideoModuleBuilder"/> is equal to the current <see cref="VideoModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="VideoModuleBuilder"/> is equal to the current <see cref="VideoModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(VideoModuleBuilder? left, VideoModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="VideoModuleBuilder"/> is not equal to the current <see cref="VideoModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="VideoModuleBuilder"/> is not equal to the current <see cref="VideoModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(VideoModuleBuilder? left, VideoModuleBuilder? right) =>
        !(left == right);

    /// <summary>Determines whether the specified <see cref="VideoModuleBuilder"/> is equal to the current <see cref="VideoModuleBuilder"/>.</summary>
    /// <remarks>If the object passes is an <see cref="VideoModuleBuilder"/>, <see cref="Equals(VideoModuleBuilder)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="VideoModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="VideoModuleBuilder"/> is equal to the current <see cref="VideoModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is VideoModuleBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="VideoModuleBuilder"/> is equal to the current <see cref="VideoModuleBuilder"/>.</summary>
    /// <param name="videoModuleBuilder">The <see cref="VideoModuleBuilder"/> to compare with the current <see cref="VideoModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="VideoModuleBuilder"/> is equal to the current <see cref="VideoModuleBuilder"/>; otherwise, <c>false</c>.</returns>
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

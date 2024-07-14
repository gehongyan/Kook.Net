using System.Diagnostics.CodeAnalysis;
using Kook.Utils;

namespace Kook;

/// <summary>
///     Represents an audio module builder for creating an <see cref="AudioModule"/>.
/// </summary>
public class AudioModuleBuilder : IModuleBuilder, IEquatable<AudioModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Audio;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AudioModuleBuilder"/> class.
    /// </summary>
    public AudioModuleBuilder()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AudioModuleBuilder"/> class.
    /// </summary>
    /// <param name="source"> The source URL of the video. </param>
    /// <param name="cover"> The cover URL of the video. </param>
    /// <param name="title"> The title of the video. </param>
    public AudioModuleBuilder(string source, string? cover = null, string? title = null)
    {
        Source = source;
        Cover = cover;
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
    ///     Gets or sets the cover URL of the video.
    /// </summary>
    /// <returns>
    ///     The cover URL of the video.
    /// </returns>
    public string? Cover { get; set; }

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
    public AudioModuleBuilder WithSource(string? source)
    {
        Source = source;
        return this;
    }

    /// <summary>
    ///     Sets the cover URL of the video.
    /// </summary>
    /// <param name="cover">
    ///     The cover URL of the video to be set.
    /// </param>
    /// <returns> 当前构建器。 </returns>
    public AudioModuleBuilder WithCover(string? cover)
    {
        Cover = cover;
        return this;
    }

    /// <summary>
    ///     Sets the title of the video.
    /// </summary>
    /// <param name="title">
    ///     The title of the video to be set.
    /// </param>
    /// <returns> 当前构建器。 </returns>
    public AudioModuleBuilder WithTitle(string? title)
    {
        Title = title;
        return this;
    }

    /// <summary>
    ///     Builds this builder into an <see cref="AudioModule"/>.
    /// </summary>
    /// <returns>
    ///     An <see cref="AudioModule"/> representing the built audio module object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Source"/> cannot be null
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Source"/> cannot be empty
    /// </exception>
    /// <exception cref="UriFormatException">
    ///     <see cref="Source"/> is not a valid URL
    /// </exception>
    /// <exception cref="UriFormatException">
    ///     <see cref="Cover"/> is not a valid URL
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

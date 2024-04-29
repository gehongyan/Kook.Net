using System.Diagnostics.CodeAnalysis;
using Kook.Utils;

namespace Kook;

/// <summary>
///     Represents a file module builder for creating a <see cref="FileModule"/>.
/// </summary>
public class FileModuleBuilder : IModuleBuilder, IEquatable<FileModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.File;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileModuleBuilder"/> class.
    /// </summary>
    public FileModuleBuilder()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileModuleBuilder"/> class.
    /// </summary>
    /// <param name="source"> The source URL of the file. </param>
    /// <param name="title"> The title of the file. </param>
    public FileModuleBuilder(string source, string? title = null)
    {
        Source = source;
        Title = title;
    }

    /// <summary>
    ///     Gets or sets the source URL of the file.
    /// </summary>
    /// <returns>
    ///     The source URL of the file.
    /// </returns>
    public string? Source { get; set; }

    /// <summary>
    ///     Gets or sets the title of the file.
    /// </summary>
    /// <returns>
    ///     The title of the file.
    /// </returns>
    public string? Title { get; set; }

    /// <summary>
    ///     Sets the source URL of the file.
    /// </summary>
    /// <param name="source">
    ///     The source URL of the file to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public FileModuleBuilder WithSource(string? source)
    {
        Source = source;
        return this;
    }

    /// <summary>
    ///     Sets the title of the file.
    /// </summary>
    /// <param name="title">
    ///     The title of the file to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public FileModuleBuilder WithTitle(string title)
    {
        Title = title;
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="FileModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="FileModule"/> representing the built file module object.
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
    ///     Determines whether the specified <see cref="FileModuleBuilder"/> is equal to the current <see cref="FileModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="FileModuleBuilder"/> is equal to the current <see cref="FileModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(FileModuleBuilder? left, FileModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="FileModuleBuilder"/> is not equal to the current <see cref="FileModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="FileModuleBuilder"/> is not equal to the current <see cref="FileModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(FileModuleBuilder? left, FileModuleBuilder? right) =>
        !(left == right);

    /// <summary>Determines whether the specified <see cref="FileModuleBuilder"/> is equal to the current <see cref="FileModuleBuilder"/>.</summary>
    /// <remarks>If the object passes is an <see cref="FileModuleBuilder"/>, <see cref="Equals(FileModuleBuilder)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="FileModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="FileModuleBuilder"/> is equal to the current <see cref="FileModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is FileModuleBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="FileModuleBuilder"/> is equal to the current <see cref="FileModuleBuilder"/>.</summary>
    /// <param name="fileModuleBuilder">The <see cref="FileModuleBuilder"/> to compare with the current <see cref="FileModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="FileModuleBuilder"/> is equal to the current <see cref="FileModuleBuilder"/>; otherwise, <c>false</c>.</returns>
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

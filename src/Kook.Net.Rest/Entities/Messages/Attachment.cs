using System.Diagnostics;
using Model = Kook.API.Attachment;

namespace Kook;

/// <inheritdoc cref="IAttachment"/>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Attachment : IAttachment
{
    /// <inheritdoc />
    public AttachmentType Type { get; }

    /// <inheritdoc />
    public string Url { get; }

    /// <inheritdoc />
    public string Filename { get; }

    /// <inheritdoc />
    public int? Size { get; }

    /// <inheritdoc />
    public string FileType { get; }

    /// <inheritdoc />
    public TimeSpan? Duration { get; }

    /// <inheritdoc />
    public int? Width { get; }

    /// <inheritdoc />
    public int? Height { get; }

    internal Attachment(AttachmentType type, string url, string filename, int? size, string fileType, TimeSpan? duration, int? width, int? height)
    {
        Type = type;
        Url = url;
        Filename = filename;
        Size = size;
        FileType = fileType;
        Duration = duration;
        Width = width;
        Height = height;
    }

    internal static Attachment Create(Model model)
    {
        AttachmentType type = model.Type switch
        {
            "image" => AttachmentType.Image,
            "video" => AttachmentType.Video,
            "file" => AttachmentType.File,
            _ => throw new ArgumentOutOfRangeException(nameof(model.Type))
        };
        TimeSpan? duration = model.Duration.HasValue
            ? TimeSpan.FromSeconds(model.Duration.Value)
            : null;
        return new Attachment(type, model.Url, model.Name, model.Size, model.FileType, duration, model.Width, model.Height);
    }

    /// <summary>
    ///     Returns the filename of this attachment.
    /// </summary>
    /// <returns>
    ///     A string containing the filename of this attachment.
    /// </returns>
    public override string ToString() => Filename;

    private string DebuggerDisplay => $"{Filename}{(Size.HasValue ? $" ({Size} bytes)" : "")}";
}

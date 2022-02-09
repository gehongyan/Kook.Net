using System.Diagnostics;
using Model = KaiHeiLa.API.Attachment;

namespace KaiHeiLa;

[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class Attachment : IAttachment
{
    public AttachmentType Type { get; }
    public string Url { get; }
    public string Filename { get; }
    public int? Size { get; }
    public string FileType { get; }
    public TimeSpan? Duration { get; }
    public int? Width { get; }
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
        TimeSpan? duration = model.Duration is null
            ? null
            : TimeSpan.FromSeconds(model.Duration.Value);
        return new Attachment(type, model.Url, model.Name, model.Size, model.FileType, duration, model.Width, model.Height);
    }
    
    /// <summary>
    ///     Returns the filename of this attachment.
    /// </summary>
    /// <returns>
    ///     A string containing the filename of this attachment.
    /// </returns>
    public override string ToString() => Filename;
    private string DebuggerDisplay => $"{Filename}{(Size is null ? "" : $" ({Size} bytes)")}";
}
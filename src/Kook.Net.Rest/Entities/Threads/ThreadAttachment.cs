using System.Diagnostics;
using Model = Kook.API.ThreadMedia;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的帖子内的附件。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ThreadAttachment : IThreadAttachment
{
    /// <inheritdoc />
    public ThreadAttachmentType Type { get; }

    /// <inheritdoc />
    public string Url { get; }

    /// <inheritdoc />
    public string Filename { get; }

    /// <inheritdoc />
    public string? Cover { get; }

    internal ThreadAttachment(ThreadAttachmentType type, string url, string filename, string? cover)
    {
        Type = type;
        Url = url;
        Filename = filename;
        Cover = cover;
    }

    internal static ThreadAttachment Create(Model model) =>
        new(model.Type, model.Source, model.Title, model.Cover);

    /// <inheritdoc cref="Kook.Rest.ThreadAttachment.Filename" />
    /// <returns> 此附件的文件名。 </returns>
    public override string ToString() => Filename;

    private string DebuggerDisplay => $"{Filename} ({Type})";
}

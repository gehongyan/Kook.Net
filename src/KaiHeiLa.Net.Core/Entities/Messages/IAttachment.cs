namespace KaiHeiLa;

public interface IAttachment
{
    AttachmentType Type { get; }

    string Url { get; }

    string Filename { get; }

    int Size { get; }
    
    string FileType { get; }
    
    TimeSpan? Duration { get; }
    
    int? Width { get; }
    
    int? Height { get; }
}
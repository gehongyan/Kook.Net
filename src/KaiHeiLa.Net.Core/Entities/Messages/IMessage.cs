namespace KaiHeiLa;

public interface IMessage : IGuidEntity, IDeletable
{
    MessageType Type { get; }
    
    MessageSource Source { get; }
    
    bool? IsPinned { get; }
    
    IMessageChannel Channel { get; }
    
    IUser Author { get; }
    
    string Content { get; }
    
    string CleanContent { get; }
    
    DateTimeOffset Timestamp { get; }
    
    DateTimeOffset? EditedTimestamp { get; }
    
    IReadOnlyCollection<ulong> MentionedUserIds { get; }
    
    IReadOnlyCollection<uint> MentionedRoleIds { get; }
    
    bool? MentionedEveryone { get; }
    
    bool? MentionedHere { get; }
    
    IReadOnlyCollection<ITag> Tags { get; }
    
    IAttachment Attachment { get; }
    
    IReadOnlyCollection<ICard> Cards { get; }
}
namespace KaiHeiLa;

public interface IMessage : IGuidEntity, IDeletable
{
    MessageType Type { get; }
    
    MessageSource Source { get; }
    
    IMessageChannel Channel { get; }
    
    IUser Author { get; }
    
    string Content { get; }
    
    DateTimeOffset Timestamp { get; }
    
    IReadOnlyCollection<ulong> MentionedUserIds { get; }
    
    IReadOnlyCollection<uint> MentionedRoleIds { get; }
    
    bool? MentionedEveryone { get; }
    
    bool? MentionedHere { get; }
    
    IAttachment Attachment { get; }
    
    IReadOnlyCollection<ICard> Cards { get; }
}
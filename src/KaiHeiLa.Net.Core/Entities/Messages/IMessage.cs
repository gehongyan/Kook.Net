namespace KaiHeiLa;

public interface IMessage : IGuidEntity
{
    ChannelType ChannelType { get; }
    
    MessageType Type { get; }
    
    MessageSource Source { get; }
    
    IMessageChannel Channel { get; }
    
    string Content { get; }
    
    DateTimeOffset Timestamp { get; }
    
    IReadOnlyCollection<uint> MentionedUserIds { get; }
    
    IReadOnlyCollection<uint> MentionedRoleIds { get; }
    
    bool MentionedEveryone { get; }
    
    bool MentionedHere { get; }
    
    IAttachment Attachment { get; }
    
    IReadOnlyCollection<ICard> Cards { get; }
}
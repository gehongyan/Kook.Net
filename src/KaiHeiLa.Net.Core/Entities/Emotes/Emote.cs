using System.Globalization;

namespace KaiHeiLa;

public class Emote : IEmote
{
    public string Id { get; }
    
    public string Name { get; }
    
    public bool Animated { get; }
    
    internal Emote(string id, string name, bool animated)
    {
        Id = id;
        Name = name;
        Animated = animated;
    }
    
    public override bool Equals(object other)
    {
        if (other == null) return false;
        if (other == this) return true;

        if (other is not Emote otherEmote) 
            return false;

        return Id == otherEmote.Id;
    }
    
    public override int GetHashCode()
        => Id.GetHashCode();
}
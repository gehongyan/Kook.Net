using System.Diagnostics;

namespace KaiHeiLa;

[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class UserTag : IEquatable<UserTag>
{
    public Color Color { get; }
    public string Text { get; }

    private UserTag(Color color, string text)
    {
        Color = color;
        Text = text;
    }

    public static UserTag Create(Color color, string text)
    {
        return new UserTag(color, text);
    }
    
    private string DebuggerDisplay => Text;

    #region IEquatable

    public bool Equals(UserTag other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Text == other.Text;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((UserTag) obj);
    }

    public override int GetHashCode()
    {
        return (Text != null ? Text.GetHashCode() : 0);
    }
    
    #endregion
    
}
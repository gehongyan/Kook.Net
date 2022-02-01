using System.Drawing;

namespace KaiHeiLa;

public interface IRole : IEntity<uint>, IMentionable
{
    string Name { get; }

    Color Color { get; }

    int Position { get; }

    bool IsHoisted { get; }

    bool IsMentionable { get; }

    GuildPermission Permissions { get; }
}
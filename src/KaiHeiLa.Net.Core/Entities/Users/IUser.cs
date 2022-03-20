namespace KaiHeiLa;

public interface IUser : IEntity<ulong>, IMentionable
{
    string Username { get; }

    string IdentifyNumber { get; }
    
    ushort? IdentifyNumberValue { get; }

    bool? IsOnline { get; }

    bool? IsBot { get; }

    bool? IsBanned { get; }

    bool? IsVIP { get; }

    string Avatar { get; }

    string VIPAvatar { get; }
    
    // TODO: CreateDMChannelAsync
}
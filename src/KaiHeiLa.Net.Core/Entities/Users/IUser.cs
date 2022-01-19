namespace KaiHeiLa;

public interface IUser : IEntity<uint>
{
    public string Username { get; set; }

    public string IdentifyNumber { get; set; }
    
    public ushort IdentifyNumberValue { get; set; }

    public bool IsOnline { get; set; }

    public bool IsBot { get; set; }

    public bool IsBanned { get; set; }

    public bool IsVIP { get; set; }

    public string Avatar { get; set; }

    public string VIPAvatar { get; set; }

    public bool IsMobileVerified { get; set; }
}
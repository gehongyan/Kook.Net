namespace KaiHeiLa;

public interface ISelfUser : IUser
{
    string MobilePrefix { get; }
    
    string Mobile { get; }
    
    int InvitedCount { get; }

    bool IsMobileVerified { get; }
}
namespace KaiHeiLa;

public class UserPermissionOverwrite : IPermissionOverwrite<IUser>
{
    public IUser Target { get; }
    
    public OverwritePermissions Permissions { get; }
    
    public UserPermissionOverwrite(IUser target, OverwritePermissions permissions)
    {
        Target = target;
        Permissions = permissions;
    }
}
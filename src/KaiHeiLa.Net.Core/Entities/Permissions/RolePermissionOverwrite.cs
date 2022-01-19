namespace KaiHeiLa;

public class RolePermissionOverwrite : IPermissionOverwrite<int>
{
    public int Target { get; }
    
    public OverwritePermissions Permissions { get; }
    
    public RolePermissionOverwrite(int target, OverwritePermissions permissions)
    {
        Target = target;
        Permissions = permissions;
    }
}
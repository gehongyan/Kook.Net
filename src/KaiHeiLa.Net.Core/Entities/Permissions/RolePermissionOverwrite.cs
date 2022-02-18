namespace KaiHeiLa;

public class RolePermissionOverwrite : IPermissionOverwrite<uint>
{
    public uint Target { get; }
    
    public OverwritePermissions Permissions { get; }
    
    public RolePermissionOverwrite(uint targetId, OverwritePermissions permissions)
    {
        Target = targetId;
        Permissions = permissions;
    }
}
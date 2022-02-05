namespace KaiHeiLa;

public class RolePermissionOverwrite : IPermissionOverwrite<uint>
{
    public uint Target { get; }
    
    public OverwritePermissions Permissions { get; }
    
    public RolePermissionOverwrite(uint target, OverwritePermissions permissions)
    {
        Target = target;
        Permissions = permissions;
    }
}
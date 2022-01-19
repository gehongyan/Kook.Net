namespace KaiHeiLa;

public interface IPermissionOverwrite<TTarget>
{
    TTarget Target { get; }
    
    OverwritePermissions Permissions { get; }
}
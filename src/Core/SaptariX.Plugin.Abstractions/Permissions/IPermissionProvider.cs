namespace SaptariX.Plugin.Abstractions.Permissions;

public interface IPermissionProvider
{
    void RegisterPermissions(IPermissionRegistry registry);
}

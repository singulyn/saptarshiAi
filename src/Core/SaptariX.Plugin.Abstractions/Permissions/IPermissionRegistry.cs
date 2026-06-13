namespace SaptariX.Plugin.Abstractions.Permissions;

public interface IPermissionRegistry
{
    void Add(PermissionDefinition permission);
    IReadOnlyList<PermissionDefinition> GetPermissions();
}

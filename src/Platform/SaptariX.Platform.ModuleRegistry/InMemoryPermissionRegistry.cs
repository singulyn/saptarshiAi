using SaptariX.Plugin.Abstractions.Permissions;

namespace SaptariX.Platform.ModuleRegistry;

public sealed class InMemoryPermissionRegistry : IPermissionRegistry
{
    private readonly List<PermissionDefinition> _permissions = [];

    public void Add(PermissionDefinition permission)
    {
        _permissions.RemoveAll(x => x.Name.Equals(permission.Name, StringComparison.OrdinalIgnoreCase));
        _permissions.Add(permission);
    }

    public IReadOnlyList<PermissionDefinition> GetPermissions()
    {
        return _permissions.OrderBy(x => x.Group).ThenBy(x => x.Name).ToList();
    }
}

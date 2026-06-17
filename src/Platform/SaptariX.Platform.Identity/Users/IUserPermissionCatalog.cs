using SaptariX.Plugin.Abstractions.Permissions;

namespace SaptariX.Platform.Identity.Users;

public interface IUserPermissionCatalog
{
    IReadOnlyList<PermissionDefinition> GetPermissions();
}

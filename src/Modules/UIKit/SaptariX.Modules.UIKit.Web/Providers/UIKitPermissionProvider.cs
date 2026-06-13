using SaptariX.Modules.UIKit.Web.Permissions;
using SaptariX.Plugin.Abstractions.Permissions;

namespace SaptariX.Modules.UIKit.Web.Providers;

public sealed class UIKitPermissionProvider : IPermissionProvider
{
    public void RegisterPermissions(IPermissionRegistry registry)
    {
        registry.Add(new PermissionDefinition(UIKitPermissions.View, "View UI component catalogue", "Developer Tools"));
    }
}

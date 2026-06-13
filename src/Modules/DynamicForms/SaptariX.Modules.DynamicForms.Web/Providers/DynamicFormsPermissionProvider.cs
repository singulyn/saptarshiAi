using SaptariX.Modules.DynamicForms.Web.Permissions;
using SaptariX.Plugin.Abstractions.Permissions;

namespace SaptariX.Modules.DynamicForms.Web.Providers;

public sealed class DynamicFormsPermissionProvider : IPermissionProvider
{
    public void RegisterPermissions(IPermissionRegistry registry)
    {
        registry.Add(new PermissionDefinition(DynamicFormsPermissions.View, "View dynamic forms", "DynamicForms"));
        registry.Add(new PermissionDefinition(DynamicFormsPermissions.Manage, "Manage dynamic forms", "DynamicForms"));
    }
}

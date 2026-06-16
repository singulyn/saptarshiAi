using SaptariX.Platform.AccessControl.Permissions;
using SaptariX.Plugin.Abstractions.Permissions;

namespace SaptariX.Platform.AccessControl;

public sealed class AccessControlPermissionProvider : IPermissionProvider
{
    public void RegisterPermissions(IPermissionRegistry registry)
    {
        registry.Add(new PermissionDefinition(PlatformPermissions.DashboardView, "View dashboard", "Platform"));
        registry.Add(new PermissionDefinition(PlatformPermissions.OrganizationsView, "View organizations", "Platform"));
        registry.Add(new PermissionDefinition(PlatformPermissions.OrganizationsCreate, "Create organizations", "Platform"));
        registry.Add(new PermissionDefinition(PlatformPermissions.OrganizationsUpdate, "Update organizations", "Platform"));
        registry.Add(new PermissionDefinition(PlatformPermissions.OrganizationsDelete, "Delete organizations", "Platform"));
        registry.Add(new PermissionDefinition(PlatformPermissions.OrganizationsManageApps, "Manage organization apps and products", "Platform"));
        registry.Add(new PermissionDefinition(PlatformPermissions.OrganizationsManageModules, "Manage organization modules", "Platform"));
        registry.Add(new PermissionDefinition(PlatformPermissions.OrganizationsManageDomains, "Manage organization domains", "Platform"));
        registry.Add(new PermissionDefinition(PlatformPermissions.OrganizationsManageSettings, "Manage organization settings", "Platform"));
        registry.Add(new PermissionDefinition(PlatformPermissions.ModulesView, "View modules", "Platform"));
        registry.Add(new PermissionDefinition(PlatformPermissions.UsersView, "View users", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.UsersCreate, "Create users", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.UsersUpdate, "Update users", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.UsersDelete, "Delete users", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.UsersManageRoles, "Manage user roles", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.UsersResetPassword, "Reset user passwords", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.UsersManagePermissions, "Manage user permissions", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.RolesView, "View roles", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.RolesCreate, "Create roles", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.RolesUpdate, "Update roles", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.RolesDelete, "Delete roles", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.RolesAssign, "Assign roles", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.RolesManagePermissions, "Manage role permissions", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.PermissionsView, "View permissions", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.PermissionsCreate, "Create permissions", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.PermissionsUpdate, "Update permissions", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.PermissionsDelete, "Delete permissions", "Identity"));
        registry.Add(new PermissionDefinition(PlatformPermissions.WorkflowView, "View workflow", "Workflow"));
        registry.Add(new PermissionDefinition(PlatformPermissions.ReportsView, "View reports", "Reports"));
        registry.Add(new PermissionDefinition(PlatformPermissions.AuditLogsView, "View audit logs", "Audit"));
        registry.Add(new PermissionDefinition(PlatformPermissions.SettingsView, "View settings", "Configuration"));
        registry.Add(new PermissionDefinition(PlatformPermissions.DeveloperUIComponentsView, "View UI component catalogue", "Developer Tools"));
    }
}

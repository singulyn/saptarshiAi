using SaptariX.Platform.AccessControl.Permissions;

namespace SaptariX.Persistence.SqlServer;

internal static class AccessControlFallbackSeed
{
    internal static readonly IReadOnlyList<PermissionSeed> Permissions =
    [
        new(PlatformPermissions.DashboardView, "View dashboard", "Platform"),
        new(PlatformPermissions.OrganizationsView, "View organizations", "Platform"),
        new(PlatformPermissions.OrganizationsCreate, "Create organizations", "Platform"),
        new(PlatformPermissions.OrganizationsUpdate, "Update organizations", "Platform"),
        new(PlatformPermissions.OrganizationsDelete, "Delete organizations", "Platform"),
        new(PlatformPermissions.OrganizationsManageApps, "Manage organization apps and products", "Platform"),
        new(PlatformPermissions.OrganizationsManageModules, "Manage organization modules", "Platform"),
        new(PlatformPermissions.OrganizationsManageDomains, "Manage organization domains", "Platform"),
        new(PlatformPermissions.OrganizationsManageSettings, "Manage organization settings", "Platform"),
        new(PlatformPermissions.ModulesView, "View modules", "Platform"),
        new(PlatformPermissions.UsersView, "View users", "Identity"),
        new(PlatformPermissions.UsersCreate, "Create users", "Identity"),
        new(PlatformPermissions.UsersUpdate, "Update users", "Identity"),
        new(PlatformPermissions.UsersDelete, "Delete users", "Identity"),
        new(PlatformPermissions.UsersManageRoles, "Manage user roles", "Identity"),
        new(PlatformPermissions.UsersResetPassword, "Reset user passwords", "Identity"),
        new(PlatformPermissions.UsersManagePermissions, "Manage user permissions", "Identity"),
        new(PlatformPermissions.RolesView, "View roles", "Identity"),
        new(PlatformPermissions.RolesCreate, "Create roles", "Identity"),
        new(PlatformPermissions.RolesUpdate, "Update roles", "Identity"),
        new(PlatformPermissions.RolesDelete, "Delete roles", "Identity"),
        new(PlatformPermissions.RolesAssign, "Assign roles", "Identity"),
        new(PlatformPermissions.RolesManagePermissions, "Manage role permissions", "Identity"),
        new(PlatformPermissions.PermissionsView, "View permissions", "Identity"),
        new(PlatformPermissions.PermissionsCreate, "Create permissions", "Identity"),
        new(PlatformPermissions.PermissionsUpdate, "Update permissions", "Identity"),
        new(PlatformPermissions.PermissionsDelete, "Delete permissions", "Identity"),
        new(PlatformPermissions.WorkflowView, "View workflow", "Workflow"),
        new(PlatformPermissions.ReportsView, "View reports", "Reports"),
        new(PlatformPermissions.AuditLogsView, "View audit logs", "Audit"),
        new(PlatformPermissions.SettingsView, "View settings", "Configuration"),
        new(PlatformPermissions.DeveloperUIComponentsView, "View UI component catalogue", "Developer Tools")
    ];

    internal static readonly IReadOnlyList<RoleSeed> Roles =
    [
        new(Guid.Parse("20000000-0000-0000-0000-000000000001"), "SuperAdmin", "Super Admin", "Full platform administration.", true),
        new(Guid.Parse("20000000-0000-0000-0000-000000000002"), "Developer", "Developer", "Developer and UI kit access.", true),
        new(Guid.Parse("20000000-0000-0000-0000-000000000003"), "Admin", "Admin", "Organization administration.", true),
        new(Guid.Parse("20000000-0000-0000-0000-000000000004"), "Manager", "Manager", "Operational management access.", true),
        new(Guid.Parse("20000000-0000-0000-0000-000000000005"), "User", "User", "Standard application user.", true)
    ];

    internal static IReadOnlyList<string> DefaultPermissionsForRole(string roleName)
    {
        if (roleName.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
        {
            return Permissions.Select(x => x.Name).ToList();
        }

        if (roleName.Equals("Developer", StringComparison.OrdinalIgnoreCase))
        {
            return Permissions.Select(x => x.Name).ToList();
        }

        if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            return Permissions
                .Where(x => x.Group is "Platform" or "Identity" or "Configuration")
                .Select(x => x.Name)
                .ToList();
        }

        if (roleName.Equals("Manager", StringComparison.OrdinalIgnoreCase))
        {
            return
            [
                PlatformPermissions.DashboardView,
                PlatformPermissions.OrganizationsView,
                PlatformPermissions.UsersView,
                PlatformPermissions.ReportsView
            ];
        }

        return
        [
            PlatformPermissions.DashboardView,
            PlatformPermissions.ReportsView
        ];
    }

    internal sealed record PermissionSeed(string Name, string DisplayName, string Group);
    internal sealed record RoleSeed(Guid Id, string Name, string DisplayName, string Description, bool IsSystem);
}

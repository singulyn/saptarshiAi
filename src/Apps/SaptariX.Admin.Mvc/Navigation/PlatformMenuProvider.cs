using SaptariX.Platform.AccessControl.Permissions;
using SaptariX.Plugin.Abstractions.Navigation;

namespace SaptariX.Admin.Mvc.Navigation;

public sealed class PlatformMenuProvider : IMenuProvider
{
    public void RegisterMenus(IMenuRegistry registry)
    {
        registry.Add(new MenuItemDefinition("Dashboard", "/Dashboard", PlatformPermissions.DashboardView, "fa-solid fa-gauge", 10));
        registry.Add(new MenuItemDefinition("Organizations", "/Organizations", PlatformPermissions.OrganizationsView, "fa-solid fa-building", 20));
        registry.Add(new MenuItemDefinition("Modules", "/Modules", PlatformPermissions.ModulesView, "fa-solid fa-cubes", 30));
        registry.Add(new MenuItemDefinition("User Management", "#", string.Empty, "fa-solid fa-users-gear", 40));
        registry.Add(new MenuItemDefinition("Users", "/Users", PlatformPermissions.UsersView, "fa-solid fa-users", 41, "User Management"));
        registry.Add(new MenuItemDefinition("Roles", "/Roles", PlatformPermissions.RolesView, "fa-solid fa-user-shield", 42, "User Management"));
        registry.Add(new MenuItemDefinition("Permissions", "/Permissions", PlatformPermissions.PermissionsView, "fa-solid fa-key", 43, "User Management"));
        registry.Add(new MenuItemDefinition("Workflow", "/Workflow", PlatformPermissions.WorkflowView, "fa-solid fa-diagram-project", 70));
        registry.Add(new MenuItemDefinition("Reports", "/Reports", PlatformPermissions.ReportsView, "fa-solid fa-chart-line", 80));
        registry.Add(new MenuItemDefinition("Audit Logs", "/AuditLogs", PlatformPermissions.AuditLogsView, "fa-solid fa-clipboard-list", 90));
        registry.Add(new MenuItemDefinition("Settings", "/Settings", PlatformPermissions.SettingsView, "fa-solid fa-gear", 100));
    }
}

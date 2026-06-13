using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using SaptariX.Admin.Mvc.Models;
using SaptariX.Platform.AccessControl.Permissions;
using SaptariX.Platform.Organization.Services;
using SaptariX.Plugin.Abstractions.Modules;
using SaptariX.Plugin.Abstractions.Permissions;
using SaptariX.Plugin.Abstractions.Workflows;

namespace SaptariX.Admin.Mvc.Services;

public interface IDashboardPageService
{
    Task<DashboardViewModel> GetAsync(CancellationToken cancellationToken = default);
}

public interface IOrganizationsPageService : IAdminPageService { }
public interface IModulesPageService : IAdminPageService { }
public interface IUsersPageService : IAdminPageService { }
public interface IRolesPageService : IAdminPageService { }
public interface IPermissionsPageService : IAdminPageService { }
public interface IWorkflowPageService : IAdminPageService { }
public interface IReportsPageService : IAdminPageService { }
public interface IAuditLogsPageService : IAdminPageService { }
public interface ISettingsPageService : IAdminPageService { }

public interface IAdminPageService
{
    Task<AdminPageViewModel> GetAsync(CancellationToken cancellationToken = default);
}

public interface IAdminAuthenticationService
{
    Task SignInAsync(HttpContext httpContext, LoginViewModel model);
    Task SignOutAsync(HttpContext httpContext);
}

public sealed class DashboardPageService : IDashboardPageService
{
    private readonly IOrganizationService _organizationService;
    private readonly IModuleLoader _moduleLoader;
    private readonly IPermissionRegistry _permissionRegistry;
    private readonly IWorkflowActivityRegistry _workflowActivityRegistry;

    public DashboardPageService(
        IOrganizationService organizationService,
        IModuleLoader moduleLoader,
        IPermissionRegistry permissionRegistry,
        IWorkflowActivityRegistry workflowActivityRegistry)
    {
        _organizationService = organizationService;
        _moduleLoader = moduleLoader;
        _permissionRegistry = permissionRegistry;
        _workflowActivityRegistry = workflowActivityRegistry;
    }

    public async Task<DashboardViewModel> GetAsync(CancellationToken cancellationToken = default)
    {
        var organizations = await _organizationService.GetOrganizationsAsync(cancellationToken);
        var permissions = _permissionRegistry.GetPermissions();
        var activities = _workflowActivityRegistry.GetActivities();

        return new DashboardViewModel(
            "Dashboard",
            "Platform",
            "Operational snapshot for organizations, modules, workflow, and access control.",
            "/Dashboard",
            PlatformPermissions.DashboardView,
            ["Admin MVC shell", "SQL Server persistence", "DynamicForms module", "Elsa workflow boundary"],
            organizations.Count,
            _moduleLoader.LoadEnabledModules().Count,
            activities.Count,
            permissions,
            activities);
    }
}

public sealed class OrganizationsPageService : StaticAdminPageService, IOrganizationsPageService
{
    public OrganizationsPageService() : base("Organizations", "SaaS Model", "Create and manage customer workspaces and account-level configuration.", "/Organizations", PlatformPermissions.OrganizationsView, ["org.Organizations", "org.OrganizationUsers", "org.OrganizationSettings"]) { }
}

public sealed class ModulesPageService : StaticAdminPageService, IModulesPageService
{
    public ModulesPageService() : base("Modules", "Plugin Registry", "Enable, disable, install, and extract platform modules through stable contracts.", "/Modules", PlatformPermissions.ModulesView, ["DynamicForms", "Workflow", "Reports", "Notifications"]) { }
}

public sealed class UsersPageService : StaticAdminPageService, IUsersPageService
{
    public UsersPageService() : base("Users", "Identity", "Manage users with organization-aware assignments.", "/Users", PlatformPermissions.UsersView, ["User profile", "Organization assignment", "Role membership"]) { }
}

public sealed class RolesPageService : StaticAdminPageService, IRolesPageService
{
    public RolesPageService() : base("Roles", "Access Control", "Group permissions into organization-aware roles.", "/Roles", PlatformPermissions.RolesView, ["Platform Admin", "Organization Admin", "Builder"]) { }
}

public sealed class PermissionsPageService : StaticAdminPageService, IPermissionsPageService
{
    public PermissionsPageService() : base("Permissions", "Access Control", "Register platform and module permissions from providers.", "/Permissions", PlatformPermissions.PermissionsView, ["Platform permissions", "Module permissions", "Workflow permissions"]) { }
}

public sealed class WorkflowPageService : StaticAdminPageService, IWorkflowPageService
{
    public WorkflowPageService() : base("Workflow", "Elsa", "Attach workflows to organizations, apps, and modules.", "/Workflow", PlatformPermissions.WorkflowView, ["Form Submitted", "Approval Required", "Notification Sent", "Audit Logged"]) { }
}

public sealed class ReportsPageService : StaticAdminPageService, IReportsPageService
{
    public ReportsPageService() : base("Reports", "Analytics", "Prepare reporting jobs and export workflows.", "/Reports", PlatformPermissions.ReportsView, ["Scheduled reports", "Exports", "Dashboards"]) { }
}

public sealed class AuditLogsPageService : StaticAdminPageService, IAuditLogsPageService
{
    public AuditLogsPageService() : base("Audit Logs", "Governance", "Track organization-scoped security and data changes.", "/AuditLogs", PlatformPermissions.AuditLogsView, ["User activity", "Module changes", "Data exports"]) { }
}

public sealed class SettingsPageService : StaticAdminPageService, ISettingsPageService
{
    public SettingsPageService() : base("Settings", "Configuration", "Manage platform defaults and organization-level settings.", "/Settings", PlatformPermissions.SettingsView, ["General", "Security", "Integrations"]) { }
}

public abstract class StaticAdminPageService : IAdminPageService
{
    private readonly AdminPageViewModel _model;

    protected StaticAdminPageService(string title, string kicker, string summary, string activeMenu, string permission, IReadOnlyList<string> items)
    {
        _model = new AdminPageViewModel(title, kicker, summary, activeMenu, permission, items);
    }

    public Task<AdminPageViewModel> GetAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_model);
    }
}

public sealed class AdminAuthenticationService : IAdminAuthenticationService
{
    public async Task SignInAsync(HttpContext httpContext, LoginViewModel model)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, model.Email),
            new("sub", Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, "PlatformAdmin")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }

    public Task SignOutAsync(HttpContext httpContext)
    {
        return httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}

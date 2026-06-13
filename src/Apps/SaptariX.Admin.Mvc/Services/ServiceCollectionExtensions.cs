using SaptariX.Admin.Mvc.Services;

namespace SaptariX.Admin.Mvc.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAdminMvcServices(this IServiceCollection services)
    {
        services.AddScoped<IDashboardPageService, DashboardPageService>();
        services.AddScoped<IOrganizationsPageService, OrganizationsPageService>();
        services.AddScoped<IModulesPageService, ModulesPageService>();
        services.AddScoped<IUsersPageService, UsersPageService>();
        services.AddScoped<IRolesPageService, RolesPageService>();
        services.AddScoped<IPermissionsPageService, PermissionsPageService>();
        services.AddScoped<IWorkflowPageService, WorkflowPageService>();
        services.AddScoped<IReportsPageService, ReportsPageService>();
        services.AddScoped<IAuditLogsPageService, AuditLogsPageService>();
        services.AddScoped<ISettingsPageService, SettingsPageService>();
        services.AddScoped<IAdminAuthenticationService, AdminAuthenticationService>();
        return services;
    }
}

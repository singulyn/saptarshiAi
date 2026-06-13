using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaptariX.Modules.DynamicForms.Application;
using SaptariX.Modules.DynamicForms.Infrastructure;
using SaptariX.Modules.DynamicForms.Web.Providers;
using SaptariX.Plugin.Abstractions.Modules;
using SaptariX.Plugin.Abstractions.Navigation;
using SaptariX.Plugin.Abstractions.Permissions;
using SaptariX.Plugin.Abstractions.Workflows;

namespace SaptariX.Modules.DynamicForms.Web;

public sealed class DynamicFormsModule : IPlatformModule
{
    public string Name => "DynamicForms";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDynamicFormService, DynamicFormService>();
        services.AddScoped<IDynamicFormRepository, DynamicFormRepository>();
        services.AddSingleton<IMenuProvider, DynamicFormsMenuProvider>();
        services.AddSingleton<IPermissionProvider, DynamicFormsPermissionProvider>();
        services.AddSingleton<IWorkflowActivityProvider, DynamicFormsWorkflowActivityProvider>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }

    public void RegisterMenus(IMenuRegistry menus)
    {
        new DynamicFormsMenuProvider().RegisterMenus(menus);
    }

    public void RegisterPermissions(IPermissionRegistry permissions)
    {
        new DynamicFormsPermissionProvider().RegisterPermissions(permissions);
    }

    public void RegisterWorkflowActivities(IWorkflowActivityRegistry workflows)
    {
        new DynamicFormsWorkflowActivityProvider().RegisterWorkflowActivities(workflows);
    }
}

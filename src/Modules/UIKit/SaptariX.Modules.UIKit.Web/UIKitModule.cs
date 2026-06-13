using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaptariX.Modules.UIKit.Application;
using SaptariX.Modules.UIKit.Web.Providers;
using SaptariX.Plugin.Abstractions.Modules;
using SaptariX.Plugin.Abstractions.Navigation;
using SaptariX.Plugin.Abstractions.Permissions;
using SaptariX.Plugin.Abstractions.Workflows;

namespace SaptariX.Modules.UIKit.Web;

public sealed class UIKitModule : IPlatformModule
{
    public string Name => "UIKit";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUiComponentCatalogService, UiComponentCatalogService>();
        services.AddSingleton<IMenuProvider, UIKitMenuProvider>();
        services.AddSingleton<IPermissionProvider, UIKitPermissionProvider>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }

    public void RegisterMenus(IMenuRegistry menus)
    {
        new UIKitMenuProvider().RegisterMenus(menus);
    }

    public void RegisterPermissions(IPermissionRegistry permissions)
    {
        new UIKitPermissionProvider().RegisterPermissions(permissions);
    }

    public void RegisterWorkflowActivities(IWorkflowActivityRegistry workflows)
    {
    }
}

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaptariX.Plugin.Abstractions.Navigation;
using SaptariX.Plugin.Abstractions.Permissions;
using SaptariX.Plugin.Abstractions.Workflows;

namespace SaptariX.Plugin.Abstractions.Modules;

public interface IPlatformModule
{
    string Name { get; }
    void AddServices(IServiceCollection services, IConfiguration configuration);
    void MapEndpoints(IEndpointRouteBuilder endpoints);
    void RegisterMenus(IMenuRegistry menus);
    void RegisterPermissions(IPermissionRegistry permissions);
    void RegisterWorkflowActivities(IWorkflowActivityRegistry workflows);
}

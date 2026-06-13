using Microsoft.Extensions.DependencyInjection;
using SaptariX.Plugin.Abstractions.Modules;
using SaptariX.Plugin.Abstractions.Navigation;
using SaptariX.Plugin.Abstractions.Permissions;
using SaptariX.Plugin.Abstractions.Workflows;

namespace SaptariX.Platform.ModuleRegistry;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlatformModuleRegistry(this IServiceCollection services)
    {
        services.AddSingleton<IMenuRegistry, InMemoryMenuRegistry>();
        services.AddSingleton<IPermissionRegistry, InMemoryPermissionRegistry>();
        services.AddSingleton<IWorkflowActivityRegistry, InMemoryWorkflowActivityRegistry>();
        services.AddSingleton<IModuleLoader, StaticModuleLoader>();
        services.AddScoped<IModuleInstaller, ModuleInstaller>();
        return services;
    }
}

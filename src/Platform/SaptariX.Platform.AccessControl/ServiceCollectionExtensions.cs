using Microsoft.Extensions.DependencyInjection;
using SaptariX.Plugin.Abstractions.Permissions;

namespace SaptariX.Platform.AccessControl;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccessControlPlatform(this IServiceCollection services)
    {
        services.AddSingleton<IPermissionProvider, AccessControlPermissionProvider>();
        services.AddScoped<IPermissionAuthorizationService, PermissionAuthorizationService>();
        return services;
    }
}

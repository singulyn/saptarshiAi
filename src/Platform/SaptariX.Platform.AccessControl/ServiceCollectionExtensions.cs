using Microsoft.Extensions.DependencyInjection;
using SaptariX.Platform.AccessControl.EffectivePermissions;
using SaptariX.Platform.AccessControl.Permissions;
using SaptariX.Platform.AccessControl.Roles;
using SaptariX.Platform.AccessControl.UserRoles;
using SaptariX.Plugin.Abstractions.Permissions;

namespace SaptariX.Platform.AccessControl;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccessControlPlatform(this IServiceCollection services)
    {
        services.AddSingleton<IPermissionProvider, AccessControlPermissionProvider>();
        services.AddScoped<IPermissionAuthorizationService, PermissionAuthorizationService>();
        services.AddScoped<IEffectivePermissionService, EffectivePermissionService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        return services;
    }
}

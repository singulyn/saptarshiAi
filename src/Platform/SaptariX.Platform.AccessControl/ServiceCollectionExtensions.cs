using Microsoft.Extensions.DependencyInjection;
using SaptariX.Platform.AccessControl.Permissions;
using SaptariX.Platform.AccessControl.Roles;
using SaptariX.Platform.AccessControl.UserRoles;
using SaptariX.Platform.Identity.Users;
using SaptariX.Plugin.Abstractions.Permissions;

namespace SaptariX.Platform.AccessControl;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccessControlPlatform(this IServiceCollection services)
    {
        services.AddSingleton<IPermissionProvider, AccessControlPermissionProvider>();
        services.AddScoped<IPermissionAuthorizationService, PermissionAuthorizationService>();
        services.AddSingleton<InMemoryAccessControlStore>();
        services.AddSingleton<IUserPermissionCatalog, AccessControlUserPermissionCatalog>();
        services.AddScoped<IRoleRepository, InMemoryRoleRepository>();
        services.AddScoped<IPermissionRepository, InMemoryPermissionRepository>();
        services.AddScoped<IUserRoleRepository, InMemoryUserRoleRepository>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        return services;
    }
}

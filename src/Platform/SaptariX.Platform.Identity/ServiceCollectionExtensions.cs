using Microsoft.Extensions.DependencyInjection;
using SaptariX.Plugin.Abstractions.Identity;
using SaptariX.Platform.Identity.Users;

namespace SaptariX.Platform.Identity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityPlatform(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserPermissionService, UserPermissionService>();
        return services;
    }
}

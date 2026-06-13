using Microsoft.Extensions.DependencyInjection;
using SaptariX.Plugin.Abstractions.Organization;
using SaptariX.Platform.Organization.Services;

namespace SaptariX.Platform.Organization;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrganizationPlatform(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IOrganizationResolver, HeaderOrganizationResolver>();
        services.AddScoped<ICurrentOrganizationService, CurrentOrganizationService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        return services;
    }
}

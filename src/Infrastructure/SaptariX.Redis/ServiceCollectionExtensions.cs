using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SaptariX.Redis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSaptariXRedisReadyCache(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
        services.AddSingleton<ICacheClient, InMemoryCacheClient>();
        return services;
    }
}

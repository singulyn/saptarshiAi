using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaptariX.Persistence.Abstractions;
using SaptariX.Platform.AccessControl.Permissions;
using SaptariX.Platform.AccessControl.Roles;
using SaptariX.Platform.AccessControl.UserRoles;
using SaptariX.Platform.Identity.Users;

namespace SaptariX.Persistence.SqlServer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSaptariXSqlServerPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SqlServerOptions>(configuration.GetSection(SqlServerOptions.SectionName));
        services.AddScoped<IDbConnectionFactory, SqlServerConnectionFactory>();
        services.AddScoped<IDapperRepository, DapperRepository>();
        services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();
        services.AddScoped<ITransactionManager, SqlTransactionManager>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        return services;
    }
}

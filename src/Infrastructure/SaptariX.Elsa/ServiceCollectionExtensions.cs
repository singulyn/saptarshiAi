using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaptariX.Elsa.Activities;
using SaptariX.Plugin.Abstractions.Workflows;

namespace SaptariX.Elsa;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSaptariXElsa(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ElsaWorkflowOptions>(configuration.GetSection(ElsaWorkflowOptions.SectionName));
        services.AddScoped<IWorkflowRuntime, WorkflowRuntime>();
        services.AddScoped<SendNotificationActivity>();
        services.AddScoped<UpdateFormStatusActivity>();
        services.AddScoped<CreateAuditLogActivity>();
        services.AddScoped<FormSubmittedActivity>();
        services.AddSingleton<IWorkflowActivityProvider, CoreWorkflowActivityProvider>();
        return services;
    }
}

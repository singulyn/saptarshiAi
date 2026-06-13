using Microsoft.AspNetCore.Builder;
using Serilog;

namespace SaptariX.Observability;

public static class SerilogExtensions
{
    public static WebApplicationBuilder UseSaptariXSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/saptarix-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14);
        });

        return builder;
    }
}

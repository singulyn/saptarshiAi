using SaptariX.Plugin.Abstractions.Modules;

namespace SaptariX.Platform.ModuleRegistry;

public sealed class ModuleInstaller : IModuleInstaller
{
    public Task InstallAsync(IModuleManifest manifest, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task EnableAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task DisableAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

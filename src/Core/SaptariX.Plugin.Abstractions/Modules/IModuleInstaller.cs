namespace SaptariX.Plugin.Abstractions.Modules;

public interface IModuleInstaller
{
    Task InstallAsync(IModuleManifest manifest, CancellationToken cancellationToken = default);
    Task EnableAsync(string moduleName, CancellationToken cancellationToken = default);
    Task DisableAsync(string moduleName, CancellationToken cancellationToken = default);
}

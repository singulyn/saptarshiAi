namespace SaptariX.Plugin.Abstractions.Modules;

public interface IModuleLoader
{
    IReadOnlyList<IPlatformModule> LoadEnabledModules();
}

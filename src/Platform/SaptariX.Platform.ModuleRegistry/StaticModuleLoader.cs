using SaptariX.Plugin.Abstractions.Modules;

namespace SaptariX.Platform.ModuleRegistry;

public sealed class StaticModuleLoader : IModuleLoader
{
    private readonly IEnumerable<IPlatformModule> _modules;

    public StaticModuleLoader(IEnumerable<IPlatformModule> modules)
    {
        _modules = modules;
    }

    public IReadOnlyList<IPlatformModule> LoadEnabledModules()
    {
        return _modules.OrderBy(x => x.Name).ToList();
    }
}

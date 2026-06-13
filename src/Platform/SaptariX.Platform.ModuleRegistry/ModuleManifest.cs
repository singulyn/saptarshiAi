using SaptariX.Plugin.Abstractions.Modules;

namespace SaptariX.Platform.ModuleRegistry;

public sealed record ModuleManifest(
    string Name,
    string DisplayName,
    string Version,
    bool Enabled,
    IReadOnlyList<string> DependsOn) : IModuleManifest;

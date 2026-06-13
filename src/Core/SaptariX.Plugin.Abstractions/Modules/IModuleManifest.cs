namespace SaptariX.Plugin.Abstractions.Modules;

public interface IModuleManifest
{
    string Name { get; }
    string DisplayName { get; }
    string Version { get; }
    bool Enabled { get; }
    IReadOnlyList<string> DependsOn { get; }
}

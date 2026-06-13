namespace SaptariX.Plugin.Abstractions.Navigation;

public sealed record MenuItemDefinition(
    string Text,
    string Url,
    string Permission,
    string IconCssClass,
    int Order,
    string? Parent = null,
    bool Enabled = true);

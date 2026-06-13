using SaptariX.Plugin.Abstractions.Permissions;
using SaptariX.Plugin.Abstractions.Workflows;

namespace SaptariX.Admin.Mvc.Models;

public record AdminPageViewModel(
    string Title,
    string Kicker,
    string Summary,
    string ActiveMenu,
    string Permission,
    IReadOnlyList<string> Items);

public sealed record DashboardViewModel(
    string Title,
    string Kicker,
    string Summary,
    string ActiveMenu,
    string Permission,
    IReadOnlyList<string> Items,
    int OrganizationCount,
    int EnabledModuleCount,
    int WorkflowActivityCount,
    IReadOnlyList<PermissionDefinition> Permissions,
    IReadOnlyList<WorkflowActivityDefinition> WorkflowActivities)
    : AdminPageViewModel(Title, Kicker, Summary, ActiveMenu, Permission, Items);

public sealed class LoginViewModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
}

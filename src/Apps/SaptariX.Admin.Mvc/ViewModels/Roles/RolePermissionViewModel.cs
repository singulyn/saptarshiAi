using SaptariX.Platform.AccessControl.Roles;

namespace SaptariX.Admin.Mvc.ViewModels.Roles;

public sealed class RolePermissionViewModel
{
    public Guid RoleId { get; init; }
    public Guid OrganizationId { get; init; }
    public string RoleName { get; init; } = "selected-role";
    public string DisplayName { get; init; } = "Selected role";
    public IReadOnlyList<RolePermissionGroupViewModel> Groups { get; init; } = [];
    public bool HasRole => RoleId != Guid.Empty;

    public static RolePermissionViewModel Empty(Guid organizationId)
    {
        return new RolePermissionViewModel { OrganizationId = organizationId };
    }

    public static RolePermissionViewModel FromDto(RolePermissionSetDto dto)
    {
        return new RolePermissionViewModel
        {
            RoleId = dto.RoleId,
            OrganizationId = dto.OrganizationId,
            RoleName = dto.RoleName,
            DisplayName = dto.DisplayName,
            Groups = dto.Groups.Select(RolePermissionGroupViewModel.FromDto).ToList()
        };
    }
}

public sealed class RolePermissionGroupViewModel
{
    public string ModuleName { get; init; } = string.Empty;
    public IReadOnlyList<RolePermissionItemViewModel> Permissions { get; init; } = [];

    public static RolePermissionGroupViewModel FromDto(RolePermissionGroupDto dto)
    {
        return new RolePermissionGroupViewModel
        {
            ModuleName = dto.ModuleName,
            Permissions = dto.Permissions.Select(RolePermissionItemViewModel.FromDto).ToList()
        };
    }
}

public sealed class RolePermissionItemViewModel
{
    public string PermissionName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public bool IsGranted { get; init; }

    public static RolePermissionItemViewModel FromDto(RolePermissionItemDto dto)
    {
        return new RolePermissionItemViewModel
        {
            PermissionName = dto.PermissionName,
            DisplayName = dto.DisplayName,
            IsGranted = dto.IsGranted
        };
    }
}

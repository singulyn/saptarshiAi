using SaptariX.Platform.Identity.Users;

namespace SaptariX.Admin.Mvc.ViewModels.Users;

public sealed class UserPermissionViewModel
{
    public Guid UserId { get; init; }
    public Guid OrganizationId { get; init; }
    public string UserFullName { get; init; } = "Selected user";
    public string Email { get; init; } = string.Empty;
    public IReadOnlyList<UserPermissionGroupViewModel> Groups { get; init; } = [];
    public bool HasUser => UserId != Guid.Empty;

    public static UserPermissionViewModel Empty(Guid organizationId)
    {
        return new UserPermissionViewModel { OrganizationId = organizationId };
    }

    public static UserPermissionViewModel FromDto(UserPermissionSetDto dto)
    {
        return new UserPermissionViewModel
        {
            UserId = dto.UserId,
            OrganizationId = dto.OrganizationId,
            UserFullName = dto.UserFullName,
            Email = dto.Email,
            Groups = dto.Groups.Select(UserPermissionGroupViewModel.FromDto).ToList()
        };
    }
}

public sealed class UserPermissionGroupViewModel
{
    public string ModuleName { get; init; } = string.Empty;
    public IReadOnlyList<UserPermissionItemViewModel> Permissions { get; init; } = [];

    public static UserPermissionGroupViewModel FromDto(UserPermissionGroupDto dto)
    {
        return new UserPermissionGroupViewModel
        {
            ModuleName = dto.ModuleName,
            Permissions = dto.Permissions.Select(UserPermissionItemViewModel.FromDto).ToList()
        };
    }
}

public sealed class UserPermissionItemViewModel
{
    public string PermissionName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public bool IsRoleGranted { get; init; }
    public bool HasDirectOverride { get; init; }
    public bool DirectGrant { get; init; }
    public bool IsChecked { get; init; }
    public string SourceLabel => HasDirectOverride ? "Direct" : IsRoleGranted ? "Role" : "Not granted";

    public static UserPermissionItemViewModel FromDto(UserPermissionItemDto dto)
    {
        return new UserPermissionItemViewModel
        {
            PermissionName = dto.PermissionName,
            DisplayName = dto.DisplayName,
            IsRoleGranted = dto.IsRoleGranted,
            HasDirectOverride = dto.DirectGrant.HasValue,
            DirectGrant = dto.DirectGrant == true,
            IsChecked = dto.EffectiveGrant
        };
    }
}

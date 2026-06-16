using SaptariX.Platform.AccessControl.UserRoles;

namespace SaptariX.Admin.Mvc.ViewModels.Users;

public sealed class UserRoleViewModel
{
    public Guid UserId { get; init; }
    public Guid OrganizationId { get; init; }
    public string UserFullName { get; init; } = "Selected user";
    public string Email { get; init; } = string.Empty;
    public IReadOnlyList<UserRoleItemViewModel> Roles { get; init; } = [];
    public bool HasUser => UserId != Guid.Empty;

    public static UserRoleViewModel Empty(Guid organizationId)
    {
        return new UserRoleViewModel { OrganizationId = organizationId };
    }

    public static UserRoleViewModel FromDto(UserRoleSetDto dto)
    {
        return new UserRoleViewModel
        {
            UserId = dto.UserId,
            OrganizationId = dto.OrganizationId,
            UserFullName = dto.UserFullName,
            Email = dto.Email,
            Roles = dto.Roles.Select(UserRoleItemViewModel.FromDto).ToList()
        };
    }
}

public sealed class UserRoleItemViewModel
{
    public Guid RoleId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsSystem { get; init; }
    public bool IsAssigned { get; init; }

    public static UserRoleItemViewModel FromDto(UserRoleItemDto dto)
    {
        return new UserRoleItemViewModel
        {
            RoleId = dto.RoleId,
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            Description = dto.Description,
            IsSystem = dto.IsSystem,
            IsAssigned = dto.IsAssigned
        };
    }
}

using System.ComponentModel.DataAnnotations;
using SaptariX.Platform.AccessControl.Permissions;

namespace SaptariX.Admin.Mvc.ViewModels.Permissions;

public sealed class PermissionFormViewModel
{
    public Guid? Id { get; init; }

    [Required(ErrorMessage = "Permission name is required.")]
    [StringLength(160)]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Display name is required.")]
    [StringLength(200)]
    public string DisplayName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Group is required.")]
    [StringLength(100)]
    public string Group { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }

    public bool IsSystem { get; init; }

    public static PermissionFormViewModel Empty()
    {
        return new PermissionFormViewModel();
    }

    public static PermissionFormViewModel FromDto(PermissionDetailDto dto)
    {
        return new PermissionFormViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            Group = dto.Group,
            Description = dto.Description,
            IsSystem = dto.IsSystem
        };
    }
}

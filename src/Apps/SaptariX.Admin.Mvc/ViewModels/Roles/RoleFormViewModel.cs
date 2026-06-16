using System.ComponentModel.DataAnnotations;
using SaptariX.Platform.AccessControl.Roles;

namespace SaptariX.Admin.Mvc.ViewModels.Roles;

public sealed class RoleFormViewModel
{
    public Guid? Id { get; init; }
    public Guid OrganizationId { get; init; }

    [Required(ErrorMessage = "Role name is required.")]
    [StringLength(120)]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Display name is required.")]
    [StringLength(160)]
    public string DisplayName { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }

    [Required]
    [StringLength(40)]
    public string Status { get; init; } = "Active";

    public bool IsSystem { get; init; }

    public IReadOnlyList<string> Statuses { get; init; } = ["Active", "Inactive"];

    public static RoleFormViewModel Empty(Guid organizationId)
    {
        return new RoleFormViewModel { OrganizationId = organizationId };
    }

    public static RoleFormViewModel FromDto(RoleDetailDto dto)
    {
        return new RoleFormViewModel
        {
            Id = dto.Id,
            OrganizationId = dto.OrganizationId,
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            Description = dto.Description,
            Status = dto.Status,
            IsSystem = dto.IsSystem
        };
    }
}

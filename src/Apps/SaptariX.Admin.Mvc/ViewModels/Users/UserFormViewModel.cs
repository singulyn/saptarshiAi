using System.ComponentModel.DataAnnotations;
using SaptariX.Platform.Identity.Users;

namespace SaptariX.Admin.Mvc.ViewModels.Users;

public sealed class UserFormViewModel
{
    public Guid? Id { get; init; }
    public Guid OrganizationId { get; init; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100)]
    public string LastName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(320)]
    public string Email { get; init; } = string.Empty;

    [Phone(ErrorMessage = "Enter a valid mobile number.")]
    [StringLength(30)]
    public string? MobileNumber { get; init; }

    [Required(ErrorMessage = "Role is required.")]
    [StringLength(100)]
    public string Role { get; init; } = "Member";

    [Required(ErrorMessage = "Status is required.")]
    [StringLength(40)]
    public string Status { get; init; } = "Active";

    [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    public string? Password { get; init; }

    [Compare(nameof(Password), ErrorMessage = "Password and confirmation password do not match.")]
    public string? ConfirmPassword { get; init; }

    public IReadOnlyList<string> Roles { get; init; } =
    [
        "Platform Admin",
        "Organization Admin",
        "Builder",
        "Reporter",
        "Member"
    ];

    public IReadOnlyList<string> Statuses { get; init; } =
    [
        "Active",
        "Pending",
        "Inactive"
    ];

    public static UserFormViewModel Empty(Guid organizationId)
    {
        return new UserFormViewModel { OrganizationId = organizationId };
    }

    public static UserFormViewModel FromDto(UserDetailDto dto)
    {
        return new UserFormViewModel
        {
            Id = dto.Id,
            OrganizationId = dto.OrganizationId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            MobileNumber = dto.MobileNumber,
            Role = dto.Role,
            Status = dto.Status
        };
    }
}

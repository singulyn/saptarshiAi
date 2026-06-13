namespace SaptariX.Platform.Identity.Users;

public sealed class UserListFilterRequest
{
    public Guid OrganizationId { get; set; }
    public string? Search { get; set; }
    public string? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortColumn { get; set; } = "CreatedDate";
    public string SortDirection { get; set; } = "desc";
}

public sealed class UserListResultDto
{
    public IReadOnlyList<UserListItemDto> Items { get; init; } = [];
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public long TotalCount { get; init; }
    public long TotalPages => PageSize <= 0 ? 0 : (long)Math.Ceiling(TotalCount / (double)PageSize);
}

public sealed class UserListItemDto
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? MobileNumber { get; init; }
    public string Role { get; init; } = string.Empty;
    public string Status { get; init; } = "Active";
    public DateTimeOffset CreatedDateUtc { get; init; }
    public DateTimeOffset? LastLoginAtUtc { get; init; }
}

public sealed class UserDetailDto
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? MobileNumber { get; init; }
    public string Role { get; init; } = string.Empty;
    public string Status { get; init; } = "Active";
    public DateTimeOffset CreatedDateUtc { get; init; }
    public DateTimeOffset? LastLoginAtUtc { get; init; }
}

public sealed class UserCreateRequest
{
    public Guid OrganizationId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public string Role { get; set; } = "Member";
    public string Status { get; set; } = "Active";
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public Guid? CreatedBy { get; set; }
}

public sealed class UserUpdateRequest
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public string Role { get; set; } = "Member";
    public string Status { get; set; } = "Active";
    public Guid? UpdatedBy { get; set; }
}

public sealed class UserSoftDeleteRequest
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid? DeletedBy { get; set; }
}

public sealed class UserCreateCommand
{
    public Guid OrganizationId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? MobileNumber { get; init; }
    public string Role { get; init; } = "Member";
    public string Status { get; init; } = "Active";
    public string PasswordHash { get; init; } = string.Empty;
    public Guid? CreatedBy { get; init; }
}

public sealed class UserUpdateCommand
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? MobileNumber { get; init; }
    public string Role { get; init; } = "Member";
    public string Status { get; init; } = "Active";
    public Guid? UpdatedBy { get; init; }
}

public sealed class UserCommandResult
{
    private UserCommandResult(bool succeeded, string message, Guid? userId)
    {
        Succeeded = succeeded;
        Message = message;
        UserId = userId;
    }

    public bool Succeeded { get; }
    public string Message { get; }
    public Guid? UserId { get; }

    public static UserCommandResult Success(string message, Guid? userId = null) => new(true, message, userId);
    public static UserCommandResult Failure(string message) => new(false, message, null);
}

public sealed class UserPermissionRequest
{
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
}

public sealed class UserPermissionSaveRequest
{
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public IReadOnlyList<UserPermissionSelectionDto> Permissions { get; set; } = [];
    public Guid? ChangedBy { get; set; }
}

public sealed class UserPermissionSelectionDto
{
    public string PermissionName { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
}

public sealed class UserPermissionSetDto
{
    public Guid UserId { get; init; }
    public Guid OrganizationId { get; init; }
    public string UserFullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public IReadOnlyList<UserPermissionGroupDto> Groups { get; init; } = [];
}

public sealed class UserPermissionGroupDto
{
    public string ModuleName { get; init; } = string.Empty;
    public IReadOnlyList<UserPermissionItemDto> Permissions { get; init; } = [];
}

public sealed class UserPermissionItemDto
{
    public string PermissionName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string ModuleName { get; init; } = string.Empty;
    public bool IsRoleGranted { get; init; }
    public bool? DirectGrant { get; init; }
    public bool EffectiveGrant => DirectGrant ?? IsRoleGranted;
}

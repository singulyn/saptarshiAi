namespace SaptariX.Platform.AccessControl.UserRoles;

public sealed class UserRoleRequest
{
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
}

public sealed class UserRoleSaveRequest
{
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public IReadOnlyList<UserRoleSelectionDto> Roles { get; set; } = [];
    public Guid? ChangedBy { get; set; }
}

public sealed class UserRoleSelectionDto
{
    public Guid RoleId { get; set; }
    public bool IsAssigned { get; set; }
}

public sealed class UserRoleSetDto
{
    public Guid UserId { get; init; }
    public Guid OrganizationId { get; init; }
    public string UserFullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public IReadOnlyList<UserRoleItemDto> Roles { get; init; } = [];
}

public sealed class UserRoleItemDto
{
    public Guid RoleId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsSystem { get; init; }
    public bool IsAssigned { get; init; }
}

public sealed class UserRoleCommandResult
{
    private UserRoleCommandResult(bool succeeded, string message, Guid? userId)
    {
        Succeeded = succeeded;
        Message = message;
        UserId = userId;
    }

    public bool Succeeded { get; }
    public string Message { get; }
    public Guid? UserId { get; }

    public static UserRoleCommandResult Success(string message, Guid? userId = null) => new(true, message, userId);
    public static UserRoleCommandResult Failure(string message) => new(false, message, null);
}

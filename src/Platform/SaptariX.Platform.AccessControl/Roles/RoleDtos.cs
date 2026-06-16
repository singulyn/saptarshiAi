namespace SaptariX.Platform.AccessControl.Roles;

public sealed class RoleListFilterRequest
{
    public Guid OrganizationId { get; set; }
    public string? Search { get; set; }
    public string? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortColumn { get; set; } = "Name";
    public string SortDirection { get; set; } = "asc";
}

public sealed class RoleListResultDto
{
    public IReadOnlyList<RoleListItemDto> Items { get; init; } = [];
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public long TotalCount { get; init; }
    public long TotalPages => PageSize <= 0 ? 0 : (long)Math.Ceiling(TotalCount / (double)PageSize);
}

public sealed class RoleListItemDto
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = "Active";
    public bool IsSystem { get; init; }
    public int UserCount { get; init; }
    public int PermissionCount { get; init; }
    public DateTimeOffset CreatedDateUtc { get; init; }
}

public sealed class RoleDetailDto
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = "Active";
    public bool IsSystem { get; init; }
}

public sealed class RoleCreateRequest
{
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Active";
    public Guid? CreatedBy { get; set; }
}

public sealed class RoleUpdateRequest
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Active";
    public Guid? UpdatedBy { get; set; }
}

public sealed class RoleSoftDeleteRequest
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid? DeletedBy { get; set; }
}

public sealed class RoleCommandResult
{
    private RoleCommandResult(bool succeeded, string message, Guid? roleId)
    {
        Succeeded = succeeded;
        Message = message;
        RoleId = roleId;
    }

    public bool Succeeded { get; }
    public string Message { get; }
    public Guid? RoleId { get; }

    public static RoleCommandResult Success(string message, Guid? roleId = null) => new(true, message, roleId);
    public static RoleCommandResult Failure(string message) => new(false, message, null);
}

public sealed class RolePermissionRequest
{
    public Guid RoleId { get; set; }
    public Guid OrganizationId { get; set; }
}

public sealed class RolePermissionSaveRequest
{
    public Guid RoleId { get; set; }
    public Guid OrganizationId { get; set; }
    public IReadOnlyList<RolePermissionSelectionDto> Permissions { get; set; } = [];
    public Guid? ChangedBy { get; set; }
}

public sealed class RolePermissionSelectionDto
{
    public string PermissionName { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
}

public sealed class RolePermissionSetDto
{
    public Guid RoleId { get; init; }
    public Guid OrganizationId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public IReadOnlyList<RolePermissionGroupDto> Groups { get; init; } = [];
}

public sealed class RolePermissionGroupDto
{
    public string ModuleName { get; init; } = string.Empty;
    public IReadOnlyList<RolePermissionItemDto> Permissions { get; init; } = [];
}

public sealed class RolePermissionItemDto
{
    public string PermissionName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string ModuleName { get; init; } = string.Empty;
    public bool IsGranted { get; init; }
}

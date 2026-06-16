namespace SaptariX.Platform.AccessControl.Permissions;

public sealed class PermissionListFilterRequest
{
    public string? Search { get; set; }
    public string? Group { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortColumn { get; set; } = "Name";
    public string SortDirection { get; set; } = "asc";
}

public sealed class PermissionListResultDto
{
    public IReadOnlyList<PermissionListItemDto> Items { get; init; } = [];
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public long TotalCount { get; init; }
    public long TotalPages => PageSize <= 0 ? 0 : (long)Math.Ceiling(TotalCount / (double)PageSize);
}

public sealed class PermissionListItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Group { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsSystem { get; init; }
    public DateTimeOffset CreatedDateUtc { get; init; }
}

public sealed class PermissionDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Group { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsSystem { get; init; }
}

public sealed class PermissionCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public Guid? CreatedBy { get; set; }
}

public sealed class PermissionUpdateRequest
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? UpdatedBy { get; set; }
}

public sealed class PermissionDeleteRequest
{
    public Guid Id { get; set; }
    public Guid? DeletedBy { get; set; }
}

public sealed class PermissionCommandResult
{
    private PermissionCommandResult(bool succeeded, string message, Guid? permissionId)
    {
        Succeeded = succeeded;
        Message = message;
        PermissionId = permissionId;
    }

    public bool Succeeded { get; }
    public string Message { get; }
    public Guid? PermissionId { get; }

    public static PermissionCommandResult Success(string message, Guid? permissionId = null) => new(true, message, permissionId);
    public static PermissionCommandResult Failure(string message) => new(false, message, null);
}

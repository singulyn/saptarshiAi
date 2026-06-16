using SaptariX.Platform.AccessControl.Roles;

namespace SaptariX.Admin.Mvc.ViewModels.Roles;

public sealed class RoleListViewModel
{
    public Guid OrganizationId { get; init; }
    public string? Search { get; init; }
    public string? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SortColumn { get; init; } = "Name";
    public string SortDirection { get; init; } = "asc";
    public long TotalCount { get; init; }
    public long TotalPages => PageSize <= 0 ? 0 : (long)Math.Ceiling(TotalCount / (double)PageSize);
    public IReadOnlyList<RoleListItemViewModel> Items { get; init; } = [];
    public bool HasItems => Items.Count > 0;

    public static RoleListViewModel Empty(Guid organizationId)
    {
        return new RoleListViewModel { OrganizationId = organizationId };
    }

    public static RoleListViewModel FromDto(RoleListResultDto result, RoleListFilterRequest request)
    {
        return new RoleListViewModel
        {
            OrganizationId = request.OrganizationId,
            Search = request.Search,
            Status = request.Status,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            SortColumn = request.SortColumn,
            SortDirection = request.SortDirection,
            TotalCount = result.TotalCount,
            Items = result.Items.Select(RoleListItemViewModel.FromDto).ToList()
        };
    }
}

public sealed class RoleListItemViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = "Active";
    public bool IsSystem { get; init; }
    public int UserCount { get; init; }
    public int PermissionCount { get; init; }
    public string CreatedDate { get; init; } = string.Empty;
    public string StatusBadgeClass => Status.Equals("Active", StringComparison.OrdinalIgnoreCase) ? "text-bg-success" : "text-bg-secondary";

    public static RoleListItemViewModel FromDto(RoleListItemDto dto)
    {
        return new RoleListItemViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            Description = dto.Description,
            Status = dto.Status,
            IsSystem = dto.IsSystem,
            UserCount = dto.UserCount,
            PermissionCount = dto.PermissionCount,
            CreatedDate = dto.CreatedDateUtc.ToLocalTime().ToString("dd MMM yyyy")
        };
    }
}

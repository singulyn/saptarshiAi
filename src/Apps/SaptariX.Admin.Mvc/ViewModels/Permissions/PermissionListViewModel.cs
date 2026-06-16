using SaptariX.Platform.AccessControl.Permissions;

namespace SaptariX.Admin.Mvc.ViewModels.Permissions;

public sealed class PermissionListViewModel
{
    public string? Search { get; init; }
    public string? Group { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SortColumn { get; init; } = "Name";
    public string SortDirection { get; init; } = "asc";
    public long TotalCount { get; init; }
    public long TotalPages => PageSize <= 0 ? 0 : (long)Math.Ceiling(TotalCount / (double)PageSize);
    public IReadOnlyList<PermissionListItemViewModel> Items { get; init; } = [];
    public IReadOnlyList<string> Groups { get; init; } = [];
    public bool HasItems => Items.Count > 0;

    public static PermissionListViewModel Empty()
    {
        return new PermissionListViewModel();
    }

    public static PermissionListViewModel FromDto(PermissionListResultDto result, PermissionListFilterRequest request)
    {
        var items = result.Items.Select(PermissionListItemViewModel.FromDto).ToList();
        return new PermissionListViewModel
        {
            Search = request.Search,
            Group = request.Group,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            SortColumn = request.SortColumn,
            SortDirection = request.SortDirection,
            TotalCount = result.TotalCount,
            Items = items,
            Groups = items.Select(x => x.Group).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList()
        };
    }
}

public sealed class PermissionListItemViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Group { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsSystem { get; init; }
    public string CreatedDate { get; init; } = string.Empty;

    public static PermissionListItemViewModel FromDto(PermissionListItemDto dto)
    {
        return new PermissionListItemViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            Group = dto.Group,
            Description = dto.Description,
            IsSystem = dto.IsSystem,
            CreatedDate = dto.CreatedDateUtc.ToLocalTime().ToString("dd MMM yyyy")
        };
    }
}

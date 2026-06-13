using SaptariX.Platform.Identity.Users;

namespace SaptariX.Admin.Mvc.ViewModels.Users;

public sealed class UserListViewModel
{
    public Guid OrganizationId { get; init; }
    public string? Search { get; init; }
    public string? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SortColumn { get; init; } = "CreatedDate";
    public string SortDirection { get; init; } = "desc";
    public long TotalCount { get; init; }
    public long TotalPages => PageSize <= 0 ? 0 : (long)Math.Ceiling(TotalCount / (double)PageSize);
    public IReadOnlyList<UserListItemViewModel> Items { get; init; } = [];
    public bool HasItems => Items.Count > 0;

    public static UserListViewModel Empty(Guid organizationId)
    {
        return new UserListViewModel { OrganizationId = organizationId };
    }

    public static UserListViewModel FromDto(UserListResultDto result, UserListFilterRequest request)
    {
        return new UserListViewModel
        {
            OrganizationId = request.OrganizationId,
            Search = request.Search,
            Status = request.Status,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            SortColumn = request.SortColumn,
            SortDirection = request.SortDirection,
            TotalCount = result.TotalCount,
            Items = result.Items.Select(UserListItemViewModel.FromDto).ToList()
        };
    }
}

public sealed class UserListItemViewModel
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Mobile { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string CreatedDate { get; init; } = string.Empty;
    public string LastLogin { get; init; } = string.Empty;
    public string StatusBadgeClass => Status.Equals("Active", StringComparison.OrdinalIgnoreCase)
        ? "text-bg-success"
        : Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)
            ? "text-bg-warning"
            : "text-bg-secondary";

    public static UserListItemViewModel FromDto(UserListItemDto dto)
    {
        return new UserListItemViewModel
        {
            Id = dto.Id,
            FullName = string.IsNullOrWhiteSpace(dto.FullName) ? $"{dto.FirstName} {dto.LastName}".Trim() : dto.FullName,
            Email = dto.Email,
            Mobile = string.IsNullOrWhiteSpace(dto.MobileNumber) ? "-" : dto.MobileNumber,
            Role = dto.Role,
            Status = dto.Status,
            CreatedDate = dto.CreatedDateUtc.ToLocalTime().ToString("dd MMM yyyy"),
            LastLogin = dto.LastLoginAtUtc.HasValue ? dto.LastLoginAtUtc.Value.ToLocalTime().ToString("dd MMM yyyy, HH:mm") : "-"
        };
    }
}

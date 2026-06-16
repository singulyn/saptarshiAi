using System.Text.RegularExpressions;
using SaptariX.Plugin.Abstractions.Permissions;

namespace SaptariX.Platform.AccessControl.Permissions;

public sealed partial class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _repository;
    private readonly IPermissionRegistry _permissionRegistry;

    public PermissionService(IPermissionRepository repository, IPermissionRegistry permissionRegistry)
    {
        _repository = repository;
        _permissionRegistry = permissionRegistry;
    }

    public Task<PermissionListResultDto> ListAsync(PermissionListFilterRequest request, CancellationToken cancellationToken = default)
    {
        request.PageNumber = Math.Max(1, request.PageNumber);
        request.PageSize = Math.Clamp(request.PageSize, 5, 100);
        request.Search = NormalizeOptional(request.Search);
        request.Group = NormalizeOptional(request.Group);
        request.SortColumn = NormalizeSortColumn(request.SortColumn);
        request.SortDirection = string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
        return _repository.ListAsync(request, cancellationToken);
    }

    public Task<PermissionDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return id == Guid.Empty
            ? Task.FromResult<PermissionDetailDto?>(null)
            : _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<PermissionCommandResult> CreateAsync(PermissionCreateRequest request, CancellationToken cancellationToken = default)
    {
        var validation = ValidateCreate(request);
        if (validation is not null)
        {
            return PermissionCommandResult.Failure(validation);
        }

        request.Name = request.Name.Trim();
        request.DisplayName = request.DisplayName.Trim();
        request.Group = request.Group.Trim();
        request.Description = NormalizeOptional(request.Description);
        var id = await _repository.CreateAsync(request, cancellationToken);
        return PermissionCommandResult.Success("Permission created successfully.", id);
    }

    public async Task<PermissionCommandResult> UpdateAsync(PermissionUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var validation = ValidateUpdate(request);
        if (validation is not null)
        {
            return PermissionCommandResult.Failure(validation);
        }

        request.DisplayName = request.DisplayName.Trim();
        request.Group = request.Group.Trim();
        request.Description = NormalizeOptional(request.Description);
        await _repository.UpdateAsync(request, cancellationToken);
        return PermissionCommandResult.Success("Permission updated successfully.", request.Id);
    }

    public async Task<PermissionCommandResult> DeleteAsync(PermissionDeleteRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Id == Guid.Empty)
        {
            return PermissionCommandResult.Failure("Permission id is required.");
        }

        await _repository.DeleteAsync(request, cancellationToken);
        return PermissionCommandResult.Success("Permission deleted successfully.", request.Id);
    }

    private string? ValidateCreate(PermissionCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || !PermissionNamePattern().IsMatch(request.Name.Trim()))
        {
            return "Permission name must follow Module.Action format.";
        }

        if (_permissionRegistry.GetPermissions().Any(x => x.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            return "Permission name already exists in the platform registry.";
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return "Display name is required.";
        }

        return string.IsNullOrWhiteSpace(request.Group) ? "Group is required." : null;
    }

    private static string? ValidateUpdate(PermissionUpdateRequest request)
    {
        if (request.Id == Guid.Empty)
        {
            return "Permission id is required.";
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return "Display name is required.";
        }

        return string.IsNullOrWhiteSpace(request.Group) ? "Group is required." : null;
    }

    private static string NormalizeSortColumn(string? sortColumn)
    {
        return sortColumn?.Trim() switch
        {
            "Name" => "Name",
            "DisplayName" => "DisplayName",
            "Group" => "Group",
            "CreatedDate" => "CreatedDate",
            _ => "Name"
        };
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    [GeneratedRegex(@"^[A-Za-z][A-Za-z0-9]+(\.[A-Za-z][A-Za-z0-9]+)+$")]
    private static partial Regex PermissionNamePattern();
}

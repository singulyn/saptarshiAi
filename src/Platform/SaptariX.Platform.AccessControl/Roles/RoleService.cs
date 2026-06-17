using System.Text.RegularExpressions;
using SaptariX.Plugin.Abstractions.Permissions;

namespace SaptariX.Platform.AccessControl.Roles;

public sealed partial class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;
    private readonly IPermissionRegistry _permissionRegistry;

    public RoleService(IRoleRepository repository, IPermissionRegistry permissionRegistry)
    {
        _repository = repository;
        _permissionRegistry = permissionRegistry;
    }

    public Task<RoleListResultDto> ListAsync(RoleListFilterRequest request, CancellationToken cancellationToken = default)
    {
        request.PageNumber = Math.Max(1, request.PageNumber);
        request.PageSize = Math.Clamp(request.PageSize, 5, 100);
        request.Search = NormalizeOptional(request.Search);
        request.Status = NormalizeOptional(request.Status);
        request.SortColumn = NormalizeSortColumn(request.SortColumn);
        request.SortDirection = string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
        return _repository.ListAsync(request, cancellationToken);
    }

    public Task<RoleDetailDto?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty || id == Guid.Empty)
        {
            return Task.FromResult<RoleDetailDto?>(null);
        }

        return _repository.GetByIdAsync(organizationId, id, cancellationToken);
    }

    public async Task<RoleCommandResult> CreateAsync(RoleCreateRequest request, CancellationToken cancellationToken = default)
    {
        var validation = ValidateCreate(request);
        if (validation is not null)
        {
            return RoleCommandResult.Failure(validation);
        }

        request.Name = NormalizeRoleName(request.Name);
        request.DisplayName = request.DisplayName.Trim();
        request.Description = NormalizeOptional(request.Description);
        request.Status = NormalizeStatus(request.Status);

        var id = await _repository.CreateAsync(request, cancellationToken);
        return RoleCommandResult.Success("Role created successfully.", id);
    }

    public async Task<RoleCommandResult> UpdateAsync(RoleUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var validation = ValidateUpdate(request);
        if (validation is not null)
        {
            return RoleCommandResult.Failure(validation);
        }

        request.DisplayName = request.DisplayName.Trim();
        request.Description = NormalizeOptional(request.Description);
        request.Status = NormalizeStatus(request.Status);

        await _repository.UpdateAsync(request, cancellationToken);
        return RoleCommandResult.Success("Role updated successfully.", request.Id);
    }

    public async Task<RoleCommandResult> SoftDeleteAsync(RoleSoftDeleteRequest request, CancellationToken cancellationToken = default)
    {
        if (request.OrganizationId == Guid.Empty || request.Id == Guid.Empty)
        {
            return RoleCommandResult.Failure("Organization and role id are required.");
        }

        await _repository.SoftDeleteAsync(request, cancellationToken);
        return RoleCommandResult.Success("Role deleted successfully.", request.Id);
    }

    public async Task<RolePermissionSetDto> GetPermissionsAsync(RolePermissionRequest request, CancellationToken cancellationToken = default)
    {
        var role = await _repository.GetByIdAsync(request.OrganizationId, request.RoleId, cancellationToken);
        var stored = await _repository.GetPermissionsAsync(request.OrganizationId, request.RoleId, cancellationToken);
        var storedByName = stored.ToDictionary(x => x.PermissionName, StringComparer.OrdinalIgnoreCase);

        var catalog = _permissionRegistry.GetPermissions()
            .Concat(stored.Select(permission => new PermissionDefinition(permission.PermissionName, permission.DisplayName, permission.ModuleName)))
            .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First())
            .OrderBy(x => x.Group)
            .ThenBy(x => x.Name)
            .ToList();

        var groups = catalog
            .Select(permission => storedByName.TryGetValue(permission.Name, out var selected)
                ? selected
                : new RolePermissionItemDto
                {
                    PermissionName = permission.Name,
                    DisplayName = permission.DisplayName,
                    ModuleName = permission.Group,
                    IsGranted = false
                })
            .GroupBy(x => x.ModuleName)
            .Select(group => new RolePermissionGroupDto
            {
                ModuleName = group.Key,
                Permissions = group.OrderBy(x => x.PermissionName).ToList()
            })
            .ToList();

        return new RolePermissionSetDto
        {
            RoleId = request.RoleId,
            OrganizationId = request.OrganizationId,
            RoleName = role?.Name ?? "selected-role",
            DisplayName = role?.DisplayName ?? "Selected role",
            Groups = groups
        };
    }

    public async Task<RoleCommandResult> SavePermissionsAsync(RolePermissionSaveRequest request, CancellationToken cancellationToken = default)
    {
        if (request.OrganizationId == Guid.Empty || request.RoleId == Guid.Empty)
        {
            return RoleCommandResult.Failure("Organization and role id are required.");
        }

        request.Permissions = request.Permissions
            .Where(x => !string.IsNullOrWhiteSpace(x.PermissionName))
            .GroupBy(x => x.PermissionName.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(x => new RolePermissionSelectionDto
            {
                PermissionName = x.Key,
                IsGranted = x.Last().IsGranted
            })
            .ToList();

        await _repository.SavePermissionsAsync(request, cancellationToken);
        return RoleCommandResult.Success("Role permissions saved successfully.", request.RoleId);
    }

    private static string? ValidateCreate(RoleCreateRequest request)
    {
        if (request.OrganizationId == Guid.Empty)
        {
            return "Organization context is required.";
        }

        if (string.IsNullOrWhiteSpace(request.Name) || !RoleNamePattern().IsMatch(request.Name.Trim()))
        {
            return "Role name can contain letters, numbers, spaces, dots, dashes, and underscores.";
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return "Display name is required.";
        }

        return IsSupportedStatus(request.Status) ? null : "Status must be Active or Inactive.";
    }

    private static string? ValidateUpdate(RoleUpdateRequest request)
    {
        if (request.OrganizationId == Guid.Empty || request.Id == Guid.Empty)
        {
            return "Organization and role id are required.";
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return "Display name is required.";
        }

        return IsSupportedStatus(request.Status) ? null : "Status must be Active or Inactive.";
    }

    private static string NormalizeSortColumn(string? sortColumn)
    {
        return sortColumn?.Trim() switch
        {
            "Name" => "Name",
            "DisplayName" => "DisplayName",
            "Status" => "Status",
            "UserCount" => "UserCount",
            "PermissionCount" => "PermissionCount",
            "CreatedDate" => "CreatedDate",
            _ => "Name"
        };
    }

    private static string NormalizeRoleName(string name)
    {
        return Regex.Replace(name.Trim(), @"\s+", ".");
    }

    private static string NormalizeStatus(string status)
    {
        return string.Equals(status, "Inactive", StringComparison.OrdinalIgnoreCase) ? "Inactive" : "Active";
    }

    private static bool IsSupportedStatus(string? status)
    {
        return string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Inactive", StringComparison.OrdinalIgnoreCase);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    [GeneratedRegex(@"^[A-Za-z0-9][A-Za-z0-9 ._-]{1,98}[A-Za-z0-9]$")]
    private static partial Regex RoleNamePattern();
}

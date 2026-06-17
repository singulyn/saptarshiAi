using SaptariX.Plugin.Abstractions.Permissions;

namespace SaptariX.Platform.Identity.Users;

public sealed class UserPermissionService : IUserPermissionService
{
    private static readonly IReadOnlyList<PermissionDefinition> RequiredPermissionExamples =
    [
        new("Users.View", "View users", "Users"),
        new("Users.Create", "Create users", "Users"),
        new("Users.Update", "Update users", "Users"),
        new("Users.Delete", "Delete users", "Users"),
        new("Users.ManagePermissions", "Manage user permissions", "Users"),
        new("Roles.View", "View roles", "Roles"),
        new("Roles.Assign", "Assign roles", "Roles"),
        new("Reports.View", "View reports", "Reports"),
        new("Settings.View", "View settings", "Settings")
    ];

    private readonly IUserRepository _userRepository;
    private readonly IUserPermissionRepository _permissionRepository;
    private readonly IPermissionRegistry _permissionRegistry;
    private readonly IEnumerable<IUserPermissionCatalog> _permissionCatalogs;

    public UserPermissionService(
        IUserRepository userRepository,
        IUserPermissionRepository permissionRepository,
        IPermissionRegistry permissionRegistry,
        IEnumerable<IUserPermissionCatalog> permissionCatalogs)
    {
        _userRepository = userRepository;
        _permissionRepository = permissionRepository;
        _permissionRegistry = permissionRegistry;
        _permissionCatalogs = permissionCatalogs;
    }

    public async Task<UserPermissionSetDto> GetPermissionsAsync(
        UserPermissionRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.OrganizationId, request.UserId, cancellationToken);
        var storedPermissions = await _permissionRepository.GetByUserIdAsync(request.OrganizationId, request.UserId, cancellationToken);
        var storedByName = storedPermissions.ToDictionary(x => x.PermissionName, StringComparer.OrdinalIgnoreCase);

        var registered = _permissionRegistry.GetPermissions()
            .Concat(RequiredPermissionExamples)
            .Concat(_permissionCatalogs.SelectMany(catalog => catalog.GetPermissions()))
            .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First())
            .OrderBy(x => x.Group)
            .ThenBy(x => x.Name)
            .ToList();

        var groups = registered
            .Select(permission =>
            {
                if (storedByName.TryGetValue(permission.Name, out var stored))
                {
                    return stored;
                }

                return new UserPermissionItemDto
                {
                    PermissionName = permission.Name,
                    DisplayName = permission.DisplayName,
                    ModuleName = permission.Group,
                    IsRoleGranted = false,
                    DirectGrant = null
                };
            })
            .GroupBy(x => x.ModuleName)
            .Select(group => new UserPermissionGroupDto
            {
                ModuleName = group.Key,
                Permissions = group.OrderBy(x => x.PermissionName).ToList()
            })
            .ToList();

        return new UserPermissionSetDto
        {
            UserId = request.UserId,
            OrganizationId = request.OrganizationId,
            UserFullName = user is null ? "Selected user" : $"{user.FirstName} {user.LastName}".Trim(),
            Email = user?.Email ?? string.Empty,
            Groups = groups
        };
    }

    public async Task<UserCommandResult> SavePermissionsAsync(
        UserPermissionSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.OrganizationId == Guid.Empty || request.UserId == Guid.Empty)
        {
            return UserCommandResult.Failure("Organization and user id are required.");
        }

        var distinctPermissions = request.Permissions
            .Where(x => !string.IsNullOrWhiteSpace(x.PermissionName))
            .GroupBy(x => x.PermissionName.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(x => new UserPermissionSelectionDto
            {
                PermissionName = x.Key,
                IsGranted = x.Last().IsGranted
            })
            .ToList();

        request.Permissions = distinctPermissions;
        await _permissionRepository.SaveAsync(request, cancellationToken);
        return UserCommandResult.Success("Permissions saved successfully.", request.UserId);
    }
}

using SaptariX.Persistence.Abstractions;
using SaptariX.Platform.Identity.Users;

namespace SaptariX.Persistence.SqlServer;

public sealed class UserPermissionRepository : IUserPermissionRepository
{
    private const string GetByUserIdProcedure = "[identity].[sp_UserPermission_GetByUserId]";
    private const string SaveProcedure = "[identity].[sp_UserPermission_Save]";

    private static readonly object StoreLock = new();
    private static readonly Dictionary<PermissionKey, bool> FallbackPermissionOverrides = [];

    private readonly IStoredProcedureExecutor _storedProcedureExecutor;

    public UserPermissionRepository(IStoredProcedureExecutor storedProcedureExecutor)
    {
        _storedProcedureExecutor = storedProcedureExecutor;
    }

    public async Task<IReadOnlyList<UserPermissionItemDto>> GetByUserIdAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _storedProcedureExecutor.QueryAsync<UserPermissionItemDto>(
                GetByUserIdProcedure,
                new { OrganizationId = organizationId, UserId = userId },
                cancellationToken: cancellationToken);
        }
        catch
        {
            lock (StoreLock)
            {
                return FallbackPermissionOverrides
                    .Where(x => x.Key.OrganizationId == organizationId && x.Key.UserId == userId)
                    .Select(x => new UserPermissionItemDto
                    {
                        PermissionName = x.Key.PermissionName,
                        DisplayName = HumanizePermissionName(x.Key.PermissionName),
                        ModuleName = GetModuleName(x.Key.PermissionName),
                        IsRoleGranted = false,
                        DirectGrant = x.Value
                    })
                    .ToList();
            }
        }
    }

    public async Task SaveAsync(UserPermissionSaveRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var permission in request.Permissions)
            {
                await _storedProcedureExecutor.ExecuteAsync(
                    SaveProcedure,
                    new
                    {
                        request.OrganizationId,
                        request.UserId,
                        permission.PermissionName,
                        permission.IsGranted,
                        request.ChangedBy
                    },
                    cancellationToken: cancellationToken);
            }
        }
        catch
        {
            lock (StoreLock)
            {
                foreach (var permission in request.Permissions)
                {
                    var key = new PermissionKey(request.OrganizationId, request.UserId, permission.PermissionName);
                    FallbackPermissionOverrides[key] = permission.IsGranted;
                }
            }
        }
    }

    private static string HumanizePermissionName(string permissionName)
    {
        var parts = permissionName.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length == 0 ? permissionName : string.Join(' ', parts);
    }

    private static string GetModuleName(string permissionName)
    {
        var index = permissionName.IndexOf('.');
        return index <= 0 ? "General" : permissionName[..index];
    }

    private sealed record PermissionKey(Guid OrganizationId, Guid UserId, string PermissionName);
}

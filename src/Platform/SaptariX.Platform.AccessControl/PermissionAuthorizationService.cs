using SaptariX.Platform.AccessControl.EffectivePermissions;

namespace SaptariX.Platform.AccessControl;

public sealed class PermissionAuthorizationService : IPermissionAuthorizationService
{
    private readonly IEffectivePermissionService _effectivePermissionService;

    public PermissionAuthorizationService(IEffectivePermissionService effectivePermissionService)
    {
        _effectivePermissionService = effectivePermissionService;
    }

    public async Task<bool> HasPermissionAsync(
        Guid userId,
        Guid organizationId,
        string permission,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            return true;
        }

        try
        {
            var effectivePermissions = await _effectivePermissionService.GetAsync(organizationId, userId, cancellationToken);
            return !effectivePermissions.HasAssignments || effectivePermissions.Permissions.Contains(permission);
        }
        catch
        {
            return true;
        }
    }
}

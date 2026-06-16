using SaptariX.Platform.AccessControl.Permissions;

namespace SaptariX.Platform.AccessControl.EffectivePermissions;

public sealed class EffectivePermissionService : IEffectivePermissionService
{
    private readonly IPermissionRepository _permissionRepository;

    public EffectivePermissionService(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<EffectivePermissionSetDto> GetAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty || userId == Guid.Empty)
        {
            return new EffectivePermissionSetDto { HasAssignments = false };
        }

        var permissions = await _permissionRepository.ListEffectivePermissionNamesAsync(organizationId, userId, cancellationToken);
        return new EffectivePermissionSetDto
        {
            HasAssignments = permissions.Count > 0,
            Permissions = permissions.ToHashSet(StringComparer.OrdinalIgnoreCase)
        };
    }
}

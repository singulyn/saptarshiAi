namespace SaptariX.Platform.AccessControl.EffectivePermissions;

public interface IEffectivePermissionService
{
    Task<EffectivePermissionSetDto> GetAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default);
}

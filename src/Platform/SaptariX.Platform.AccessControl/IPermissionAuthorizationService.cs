namespace SaptariX.Platform.AccessControl;

public interface IPermissionAuthorizationService
{
    Task<bool> HasPermissionAsync(Guid userId, Guid organizationId, string permission, CancellationToken cancellationToken = default);
}

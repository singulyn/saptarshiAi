namespace SaptariX.Platform.AccessControl;

public sealed class PermissionAuthorizationService : IPermissionAuthorizationService
{
    public Task<bool> HasPermissionAsync(
        Guid userId,
        Guid organizationId,
        string permission,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }
}

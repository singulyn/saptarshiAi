namespace SaptariX.Platform.Identity.Users;

public interface IUserPermissionRepository
{
    Task<IReadOnlyList<UserPermissionItemDto>> GetByUserIdAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task SaveAsync(UserPermissionSaveRequest request, CancellationToken cancellationToken = default);
}

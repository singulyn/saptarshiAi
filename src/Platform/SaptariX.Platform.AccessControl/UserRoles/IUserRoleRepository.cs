namespace SaptariX.Platform.AccessControl.UserRoles;

public interface IUserRoleRepository
{
    Task<UserRoleSetDto> GetByUserIdAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default);
    Task SaveAsync(UserRoleSaveRequest request, CancellationToken cancellationToken = default);
}

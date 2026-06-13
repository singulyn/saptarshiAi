namespace SaptariX.Platform.Identity.Users;

public interface IUserRepository
{
    Task<UserListResultDto> ListAsync(UserListFilterRequest request, CancellationToken cancellationToken = default);
    Task<UserDetailDto?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(UserCreateCommand command, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserUpdateCommand command, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(UserSoftDeleteRequest request, CancellationToken cancellationToken = default);
}

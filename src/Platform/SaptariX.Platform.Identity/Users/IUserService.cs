namespace SaptariX.Platform.Identity.Users;

public interface IUserService
{
    Task<UserListResultDto> ListAsync(UserListFilterRequest request, CancellationToken cancellationToken = default);
    Task<UserDetailDto?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken cancellationToken = default);
    Task<UserCommandResult> CreateAsync(UserCreateRequest request, CancellationToken cancellationToken = default);
    Task<UserCommandResult> UpdateAsync(UserUpdateRequest request, CancellationToken cancellationToken = default);
    Task<UserCommandResult> SoftDeleteAsync(UserSoftDeleteRequest request, CancellationToken cancellationToken = default);
}

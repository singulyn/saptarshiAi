namespace SaptariX.Platform.AccessControl.Roles;

public interface IRoleRepository
{
    Task<RoleListResultDto> ListAsync(RoleListFilterRequest request, CancellationToken cancellationToken = default);
    Task<RoleDetailDto?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(RoleCreateRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(RoleUpdateRequest request, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(RoleSoftDeleteRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RolePermissionItemDto>> GetPermissionsAsync(Guid organizationId, Guid roleId, CancellationToken cancellationToken = default);
    Task SavePermissionsAsync(RolePermissionSaveRequest request, CancellationToken cancellationToken = default);
}

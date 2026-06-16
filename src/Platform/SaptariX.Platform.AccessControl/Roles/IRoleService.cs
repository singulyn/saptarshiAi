namespace SaptariX.Platform.AccessControl.Roles;

public interface IRoleService
{
    Task<RoleListResultDto> ListAsync(RoleListFilterRequest request, CancellationToken cancellationToken = default);
    Task<RoleDetailDto?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken cancellationToken = default);
    Task<RoleCommandResult> CreateAsync(RoleCreateRequest request, CancellationToken cancellationToken = default);
    Task<RoleCommandResult> UpdateAsync(RoleUpdateRequest request, CancellationToken cancellationToken = default);
    Task<RoleCommandResult> SoftDeleteAsync(RoleSoftDeleteRequest request, CancellationToken cancellationToken = default);
    Task<RolePermissionSetDto> GetPermissionsAsync(RolePermissionRequest request, CancellationToken cancellationToken = default);
    Task<RoleCommandResult> SavePermissionsAsync(RolePermissionSaveRequest request, CancellationToken cancellationToken = default);
}

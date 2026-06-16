namespace SaptariX.Platform.AccessControl.Permissions;

public interface IPermissionRepository
{
    Task<PermissionListResultDto> ListAsync(PermissionListFilterRequest request, CancellationToken cancellationToken = default);
    Task<PermissionDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(PermissionCreateRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(PermissionUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(PermissionDeleteRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ListEffectivePermissionNamesAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default);
}

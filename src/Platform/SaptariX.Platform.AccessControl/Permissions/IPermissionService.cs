namespace SaptariX.Platform.AccessControl.Permissions;

public interface IPermissionService
{
    Task<PermissionListResultDto> ListAsync(PermissionListFilterRequest request, CancellationToken cancellationToken = default);
    Task<PermissionDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PermissionCommandResult> CreateAsync(PermissionCreateRequest request, CancellationToken cancellationToken = default);
    Task<PermissionCommandResult> UpdateAsync(PermissionUpdateRequest request, CancellationToken cancellationToken = default);
    Task<PermissionCommandResult> DeleteAsync(PermissionDeleteRequest request, CancellationToken cancellationToken = default);
}

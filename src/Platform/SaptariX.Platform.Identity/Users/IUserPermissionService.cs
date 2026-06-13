namespace SaptariX.Platform.Identity.Users;

public interface IUserPermissionService
{
    Task<UserPermissionSetDto> GetPermissionsAsync(UserPermissionRequest request, CancellationToken cancellationToken = default);
    Task<UserCommandResult> SavePermissionsAsync(UserPermissionSaveRequest request, CancellationToken cancellationToken = default);
}

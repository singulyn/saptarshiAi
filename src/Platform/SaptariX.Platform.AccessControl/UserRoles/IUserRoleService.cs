namespace SaptariX.Platform.AccessControl.UserRoles;

public interface IUserRoleService
{
    Task<UserRoleSetDto> GetRolesAsync(UserRoleRequest request, CancellationToken cancellationToken = default);
    Task<UserRoleCommandResult> SaveRolesAsync(UserRoleSaveRequest request, CancellationToken cancellationToken = default);
}

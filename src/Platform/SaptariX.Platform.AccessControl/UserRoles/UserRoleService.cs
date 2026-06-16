namespace SaptariX.Platform.AccessControl.UserRoles;

public sealed class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _repository;

    public UserRoleService(IUserRoleRepository repository)
    {
        _repository = repository;
    }

    public Task<UserRoleSetDto> GetRolesAsync(UserRoleRequest request, CancellationToken cancellationToken = default)
    {
        if (request.OrganizationId == Guid.Empty || request.UserId == Guid.Empty)
        {
            return Task.FromResult(new UserRoleSetDto
            {
                OrganizationId = request.OrganizationId,
                UserId = request.UserId,
                UserFullName = "Selected user"
            });
        }

        return _repository.GetByUserIdAsync(request.OrganizationId, request.UserId, cancellationToken);
    }

    public async Task<UserRoleCommandResult> SaveRolesAsync(UserRoleSaveRequest request, CancellationToken cancellationToken = default)
    {
        if (request.OrganizationId == Guid.Empty || request.UserId == Guid.Empty)
        {
            return UserRoleCommandResult.Failure("Organization and user id are required.");
        }

        request.Roles = request.Roles
            .GroupBy(x => x.RoleId)
            .Where(x => x.Key != Guid.Empty)
            .Select(x => new UserRoleSelectionDto
            {
                RoleId = x.Key,
                IsAssigned = x.Last().IsAssigned
            })
            .ToList();

        await _repository.SaveAsync(request, cancellationToken);
        return UserRoleCommandResult.Success("User roles saved successfully.", request.UserId);
    }
}

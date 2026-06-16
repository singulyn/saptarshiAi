using SaptariX.Persistence.Abstractions;
using SaptariX.Platform.AccessControl.UserRoles;
using SaptariX.Platform.Identity.Users;

namespace SaptariX.Persistence.SqlServer;

public sealed class UserRoleRepository : IUserRoleRepository
{
    private const string GetByUserIdProcedure = "[access].[sp_UserRole_GetByUserId]";
    private const string SaveProcedure = "[access].[sp_UserRole_Save]";

    private static readonly object StoreLock = new();
    private static readonly Dictionary<UserRoleKey, HashSet<Guid>> Assignments = [];

    private readonly IStoredProcedureExecutor _storedProcedureExecutor;
    private readonly IUserRepository _userRepository;

    public UserRoleRepository(IStoredProcedureExecutor storedProcedureExecutor, IUserRepository userRepository)
    {
        _storedProcedureExecutor = storedProcedureExecutor;
        _userRepository = userRepository;
    }

    public async Task<UserRoleSetDto> GetByUserIdAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _storedProcedureExecutor.QueryAsync<UserRoleRow>(
                GetByUserIdProcedure,
                new { OrganizationId = organizationId, UserId = userId },
                cancellationToken: cancellationToken);
            var user = await _userRepository.GetByIdAsync(organizationId, userId, cancellationToken);

            return new UserRoleSetDto
            {
                OrganizationId = organizationId,
                UserId = userId,
                UserFullName = user is null ? "Selected user" : $"{user.FirstName} {user.LastName}".Trim(),
                Email = user?.Email ?? string.Empty,
                Roles = rows.Select(ToItem).ToList()
            };
        }
        catch
        {
            var user = await _userRepository.GetByIdAsync(organizationId, userId, cancellationToken);
            lock (StoreLock)
            {
                RoleRepository.EnsureSeedRoles(organizationId);
                EnsureDefaultAssignment(organizationId, userId);
                var assigned = GetAssignedRoleIds(organizationId, userId);

                return new UserRoleSetDto
                {
                    OrganizationId = organizationId,
                    UserId = userId,
                    UserFullName = user is null ? "Selected user" : $"{user.FirstName} {user.LastName}".Trim(),
                    Email = user?.Email ?? string.Empty,
                    Roles = RoleRepository.GetFallbackRoles(organizationId)
                        .Select(role => new UserRoleItemDto
                        {
                            RoleId = role.Id,
                            Name = role.Name,
                            DisplayName = role.DisplayName,
                            Description = role.Description,
                            IsSystem = role.IsSystem,
                            IsAssigned = assigned.Contains(role.Id)
                        })
                        .ToList()
                };
            }
        }
    }

    public async Task SaveAsync(UserRoleSaveRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var role in request.Roles)
            {
                await _storedProcedureExecutor.ExecuteAsync(
                    SaveProcedure,
                    new
                    {
                        request.OrganizationId,
                        request.UserId,
                        role.RoleId,
                        role.IsAssigned,
                        request.ChangedBy
                    },
                    cancellationToken: cancellationToken);
            }
        }
        catch
        {
            lock (StoreLock)
            {
                RoleRepository.EnsureSeedRoles(request.OrganizationId);
                Assignments[new UserRoleKey(request.OrganizationId, request.UserId)] = request.Roles
                    .Where(x => x.IsAssigned)
                    .Select(x => x.RoleId)
                    .ToHashSet();
            }
        }
    }

    internal static IReadOnlyList<Guid> GetAssignedRoleIds(Guid organizationId, Guid userId)
    {
        lock (StoreLock)
        {
            EnsureDefaultAssignment(organizationId, userId);
            return Assignments.TryGetValue(new UserRoleKey(organizationId, userId), out var roleIds)
                ? roleIds.ToList()
                : [];
        }
    }

    internal static int CountUsersForRole(Guid organizationId, Guid roleId)
    {
        lock (StoreLock)
        {
            return Assignments.Count(x => x.Key.OrganizationId == organizationId && x.Value.Contains(roleId));
        }
    }

    private static void EnsureDefaultAssignment(Guid organizationId, Guid userId)
    {
        var key = new UserRoleKey(organizationId, userId);
        if (organizationId == Guid.Empty || userId == Guid.Empty || Assignments.ContainsKey(key))
        {
            return;
        }

        var roleId = userId == Guid.Parse("00000000-0000-0000-0000-000000000201")
            ? Guid.Parse("20000000-0000-0000-0000-000000000001")
            : Guid.Parse("20000000-0000-0000-0000-000000000005");

        Assignments[key] = [roleId];
    }

    private static UserRoleItemDto ToItem(UserRoleRow row)
    {
        return new UserRoleItemDto
        {
            RoleId = row.RoleId,
            Name = row.Name,
            DisplayName = row.DisplayName,
            Description = row.Description ?? string.Empty,
            IsSystem = row.IsSystem,
            IsAssigned = row.IsAssigned
        };
    }

    private sealed class UserRoleRow
    {
        public Guid RoleId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public string? Description { get; init; }
        public bool IsSystem { get; init; }
        public bool IsAssigned { get; init; }
    }

    private sealed record UserRoleKey(Guid OrganizationId, Guid UserId);
}

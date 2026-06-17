using SaptariX.Platform.AccessControl.Permissions;
using SaptariX.Platform.AccessControl.Roles;
using SaptariX.Platform.AccessControl.UserRoles;
using SaptariX.Platform.Identity.Users;
using SaptariX.Plugin.Abstractions.Permissions;

namespace SaptariX.Platform.AccessControl;

public sealed class InMemoryAccessControlStore
{
    private readonly object _lock = new();
    private readonly List<RoleRecord> _roles = [];
    private readonly List<PermissionRecord> _permissions = [];
    private readonly Dictionary<Guid, HashSet<string>> _rolePermissions = [];
    private readonly Dictionary<(Guid OrganizationId, Guid UserId), HashSet<Guid>> _userRoles = [];

    public InMemoryAccessControlStore()
    {
        Seed();
    }

    public object SyncRoot => _lock;
    public List<RoleRecord> Roles => _roles;
    public List<PermissionRecord> Permissions => _permissions;
    public Dictionary<Guid, HashSet<string>> RolePermissions => _rolePermissions;
    public Dictionary<(Guid OrganizationId, Guid UserId), HashSet<Guid>> UserRoles => _userRoles;

    private void Seed()
    {
        var organizationId = Guid.Parse("00000000-0000-0000-0000-000000000101");
        _roles.AddRange(
        [
            new RoleRecord(Guid.Parse("20000000-0000-0000-0000-000000000001"), organizationId, "Admin", "Admin", "Can manage users, roles, and permissions.", "Active", true, DateTimeOffset.UtcNow.AddDays(-12)),
            new RoleRecord(Guid.Parse("20000000-0000-0000-0000-000000000002"), organizationId, "Manager", "Manager", "Can manage operational records.", "Active", true, DateTimeOffset.UtcNow.AddDays(-10)),
            new RoleRecord(Guid.Parse("20000000-0000-0000-0000-000000000003"), organizationId, "User", "User", "Standard platform user.", "Active", true, DateTimeOffset.UtcNow.AddDays(-8))
        ]);

        _permissions.AddRange(
        [
            new PermissionRecord(Guid.Parse("30000000-0000-0000-0000-000000000001"), "Users.View", "View users", "Users", "Open the users module.", true, DateTimeOffset.UtcNow.AddDays(-12)),
            new PermissionRecord(Guid.Parse("30000000-0000-0000-0000-000000000002"), "Users.Manage", "Manage users", "Users", "Create and update users.", true, DateTimeOffset.UtcNow.AddDays(-12)),
            new PermissionRecord(Guid.Parse("30000000-0000-0000-0000-000000000003"), "Roles.Manage", "Manage roles", "Roles", "Create and update roles.", true, DateTimeOffset.UtcNow.AddDays(-12)),
            new PermissionRecord(Guid.Parse("30000000-0000-0000-0000-000000000004"), "Permissions.Manage", "Manage permissions", "Permissions", "Create and update permissions.", true, DateTimeOffset.UtcNow.AddDays(-12))
        ]);

        _rolePermissions[_roles[0].Id] = _permissions.Select(x => x.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        _rolePermissions[_roles[1].Id] = ["Users.View", "Users.Manage"];
        _rolePermissions[_roles[2].Id] = ["Users.View"];
    }

    public sealed class RoleRecord
    {
        public RoleRecord(Guid id, Guid organizationId, string name, string displayName, string description, string status, bool isSystem, DateTimeOffset createdDateUtc)
        {
            Id = id;
            OrganizationId = organizationId;
            Name = name;
            DisplayName = displayName;
            Description = description;
            Status = status;
            IsSystem = isSystem;
            CreatedDateUtc = createdDateUtc;
        }

        public Guid Id { get; }
        public Guid OrganizationId { get; }
        public string Name { get; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsSystem { get; }
        public DateTimeOffset CreatedDateUtc { get; }
        public bool IsDeleted { get; set; }
    }

    public sealed class PermissionRecord
    {
        public PermissionRecord(Guid id, string name, string displayName, string group, string description, bool isSystem, DateTimeOffset createdDateUtc)
        {
            Id = id;
            Name = name;
            DisplayName = displayName;
            Group = group;
            Description = description;
            IsSystem = isSystem;
            CreatedDateUtc = createdDateUtc;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string DisplayName { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; }
        public DateTimeOffset CreatedDateUtc { get; }
        public bool IsDeleted { get; set; }
    }
}

public sealed class InMemoryRoleRepository : IRoleRepository
{
    private readonly InMemoryAccessControlStore _store;

    public InMemoryRoleRepository(InMemoryAccessControlStore store)
    {
        _store = store;
    }

    public Task<RoleListResultDto> ListAsync(RoleListFilterRequest request, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            var query = _store.Roles.Where(x => x.OrganizationId == request.OrganizationId && !x.IsDeleted).AsEnumerable();
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(x => x.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                    || x.DisplayName.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                    || x.Description.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                query = query.Where(x => x.Status.Equals(request.Status, StringComparison.OrdinalIgnoreCase));
            }

            query = request.SortDirection == "desc" ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
            var total = query.LongCount();
            var items = query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).Select(ToListItem).ToList();
            return Task.FromResult(new RoleListResultDto { Items = items, PageNumber = request.PageNumber, PageSize = request.PageSize, TotalCount = total });
        }
    }

    public Task<RoleDetailDto?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            var role = _store.Roles.FirstOrDefault(x => x.OrganizationId == organizationId && x.Id == id && !x.IsDeleted);
            return Task.FromResult(role is null ? null : ToDetail(role));
        }
    }

    public Task<Guid> CreateAsync(RoleCreateRequest request, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            var id = Guid.NewGuid();
            _store.Roles.Add(new InMemoryAccessControlStore.RoleRecord(id, request.OrganizationId, request.Name, request.DisplayName, request.Description ?? string.Empty, request.Status, false, DateTimeOffset.UtcNow));
            _store.RolePermissions[id] = [];
            return Task.FromResult(id);
        }
    }

    public Task UpdateAsync(RoleUpdateRequest request, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            var role = _store.Roles.FirstOrDefault(x => x.OrganizationId == request.OrganizationId && x.Id == request.Id && !x.IsDeleted);
            if (role is not null)
            {
                role.DisplayName = request.DisplayName;
                role.Description = request.Description ?? string.Empty;
                role.Status = request.Status;
            }
        }

        return Task.CompletedTask;
    }

    public Task SoftDeleteAsync(RoleSoftDeleteRequest request, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            var role = _store.Roles.FirstOrDefault(x => x.OrganizationId == request.OrganizationId && x.Id == request.Id && !x.IsDeleted);
            if (role is not null)
            {
                role.IsDeleted = true;
            }
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<RolePermissionItemDto>> GetPermissionsAsync(Guid organizationId, Guid roleId, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            var selected = _store.RolePermissions.TryGetValue(roleId, out var permissions) ? permissions : [];
            return Task.FromResult<IReadOnlyList<RolePermissionItemDto>>(_store.Permissions.Where(x => !x.IsDeleted).Select(x => new RolePermissionItemDto
            {
                PermissionName = x.Name,
                DisplayName = x.DisplayName,
                ModuleName = x.Group,
                IsGranted = selected.Contains(x.Name)
            }).ToList());
        }
    }

    public Task SavePermissionsAsync(RolePermissionSaveRequest request, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            _store.RolePermissions[request.RoleId] = request.Permissions.Where(x => x.IsGranted).Select(x => x.PermissionName).ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        return Task.CompletedTask;
    }

    private RoleListItemDto ToListItem(InMemoryAccessControlStore.RoleRecord role) => new()
    {
        Id = role.Id,
        OrganizationId = role.OrganizationId,
        Name = role.Name,
        DisplayName = role.DisplayName,
        Description = role.Description,
        Status = role.Status,
        IsSystem = role.IsSystem,
        UserCount = _store.UserRoles.Count(x => x.Key.OrganizationId == role.OrganizationId && x.Value.Contains(role.Id)),
        PermissionCount = _store.RolePermissions.TryGetValue(role.Id, out var permissions) ? permissions.Count : 0,
        CreatedDateUtc = role.CreatedDateUtc
    };

    private static RoleDetailDto ToDetail(InMemoryAccessControlStore.RoleRecord role) => new()
    {
        Id = role.Id,
        OrganizationId = role.OrganizationId,
        Name = role.Name,
        DisplayName = role.DisplayName,
        Description = role.Description,
        Status = role.Status,
        IsSystem = role.IsSystem
    };
}

public sealed class InMemoryPermissionRepository : IPermissionRepository
{
    private readonly InMemoryAccessControlStore _store;

    public InMemoryPermissionRepository(InMemoryAccessControlStore store)
    {
        _store = store;
    }

    public Task<PermissionListResultDto> ListAsync(PermissionListFilterRequest request, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            var query = _store.Permissions.Where(x => !x.IsDeleted).AsEnumerable();
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(x => x.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase) || x.DisplayName.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.Group))
            {
                query = query.Where(x => x.Group.Equals(request.Group, StringComparison.OrdinalIgnoreCase));
            }

            var total = query.LongCount();
            var items = query.OrderBy(x => x.Group).ThenBy(x => x.Name).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).Select(ToListItem).ToList();
            return Task.FromResult(new PermissionListResultDto { Items = items, PageNumber = request.PageNumber, PageSize = request.PageSize, TotalCount = total });
        }
    }

    public Task<PermissionDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            var permission = _store.Permissions.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            return Task.FromResult(permission is null ? null : ToDetail(permission));
        }
    }

    public Task<Guid> CreateAsync(PermissionCreateRequest request, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            var id = Guid.NewGuid();
            _store.Permissions.Add(new InMemoryAccessControlStore.PermissionRecord(id, request.Name, request.DisplayName, request.Group, request.Description ?? string.Empty, request.IsSystem, DateTimeOffset.UtcNow));
            return Task.FromResult(id);
        }
    }

    public Task UpdateAsync(PermissionUpdateRequest request, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            var permission = _store.Permissions.FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);
            if (permission is not null)
            {
                permission.DisplayName = request.DisplayName;
                permission.Group = request.Group;
                permission.Description = request.Description ?? string.Empty;
            }
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(PermissionDeleteRequest request, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            var permission = _store.Permissions.FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);
            if (permission is not null)
            {
                permission.IsDeleted = true;
            }
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<string>> ListEffectivePermissionNamesAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            return Task.FromResult<IReadOnlyList<string>>(_store.Permissions.Where(x => !x.IsDeleted).Select(x => x.Name).ToList());
        }
    }

    private static PermissionListItemDto ToListItem(InMemoryAccessControlStore.PermissionRecord permission) => new()
    {
        Id = permission.Id,
        Name = permission.Name,
        DisplayName = permission.DisplayName,
        Group = permission.Group,
        Description = permission.Description,
        IsSystem = permission.IsSystem,
        CreatedDateUtc = permission.CreatedDateUtc
    };

    private static PermissionDetailDto ToDetail(InMemoryAccessControlStore.PermissionRecord permission) => new()
    {
        Id = permission.Id,
        Name = permission.Name,
        DisplayName = permission.DisplayName,
        Group = permission.Group,
        Description = permission.Description,
        IsSystem = permission.IsSystem
    };
}

public sealed class InMemoryUserRoleRepository : IUserRoleRepository
{
    private readonly InMemoryAccessControlStore _store;

    public InMemoryUserRoleRepository(InMemoryAccessControlStore store)
    {
        _store = store;
    }

    public Task<UserRoleSetDto> GetByUserIdAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            var key = (organizationId, userId);
            if (!_store.UserRoles.ContainsKey(key))
            {
                _store.UserRoles[key] = [];
            }

            var assigned = _store.UserRoles[key];
            return Task.FromResult(new UserRoleSetDto
            {
                OrganizationId = organizationId,
                UserId = userId,
                UserFullName = "Selected user",
                Roles = _store.Roles.Where(x => x.OrganizationId == organizationId && !x.IsDeleted && x.Status == "Active").Select(x => new UserRoleItemDto
                {
                    RoleId = x.Id,
                    Name = x.Name,
                    DisplayName = x.DisplayName,
                    Description = x.Description,
                    IsSystem = x.IsSystem,
                    IsAssigned = assigned.Contains(x.Id)
                }).ToList()
            });
        }
    }

    public Task SaveAsync(UserRoleSaveRequest request, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            _store.UserRoles[(request.OrganizationId, request.UserId)] = request.Roles.Where(x => x.IsAssigned).Select(x => x.RoleId).ToHashSet();
        }

        return Task.CompletedTask;
    }
}

public sealed class AccessControlUserPermissionCatalog : IUserPermissionCatalog
{
    private readonly InMemoryAccessControlStore _store;

    public AccessControlUserPermissionCatalog(InMemoryAccessControlStore store)
    {
        _store = store;
    }

    public IReadOnlyList<PermissionDefinition> GetPermissions()
    {
        lock (_store.SyncRoot)
        {
            return _store.Permissions
                .Where(x => !x.IsDeleted)
                .Select(x => new PermissionDefinition(x.Name, x.DisplayName, x.Group))
                .ToList();
        }
    }
}

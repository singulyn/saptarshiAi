using SaptariX.Persistence.Abstractions;
using SaptariX.Platform.AccessControl.Roles;

namespace SaptariX.Persistence.SqlServer;

public sealed class RoleRepository : IRoleRepository
{
    private const string ListProcedure = "[access].[sp_Role_List]";
    private const string GetByIdProcedure = "[access].[sp_Role_GetById]";
    private const string CreateProcedure = "[access].[sp_Role_Create]";
    private const string UpdateProcedure = "[access].[sp_Role_Update]";
    private const string SoftDeleteProcedure = "[access].[sp_Role_SoftDelete]";
    private const string GetPermissionsProcedure = "[access].[sp_RolePermission_GetByRoleId]";
    private const string SavePermissionProcedure = "[access].[sp_RolePermission_Save]";

    private static readonly object StoreLock = new();
    private static readonly List<RoleRecord> FallbackRoles = [];
    internal static readonly Dictionary<Guid, HashSet<string>> RolePermissions = [];

    private readonly IStoredProcedureExecutor _storedProcedureExecutor;

    public RoleRepository(IStoredProcedureExecutor storedProcedureExecutor)
    {
        _storedProcedureExecutor = storedProcedureExecutor;
    }

    public async Task<RoleListResultDto> ListAsync(RoleListFilterRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _storedProcedureExecutor.QueryAsync<RoleListRow>(
                ListProcedure,
                new
                {
                    request.OrganizationId,
                    request.Search,
                    request.Status,
                    request.PageNumber,
                    request.PageSize,
                    request.SortColumn,
                    request.SortDirection
                },
                cancellationToken: cancellationToken);

            return new RoleListResultDto
            {
                Items = rows.Select(ToListItem).ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = rows.FirstOrDefault()?.TotalCount ?? 0
            };
        }
        catch
        {
            return ListFallback(request);
        }
    }

    public async Task<RoleDetailDto?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _storedProcedureExecutor.QuerySingleOrDefaultAsync<RoleDetailDto>(
                GetByIdProcedure,
                new { OrganizationId = organizationId, Id = id },
                cancellationToken: cancellationToken);
        }
        catch
        {
            lock (StoreLock)
            {
                EnsureSeedRoles(organizationId);
                return FallbackRoles
                    .Where(x => x.OrganizationId == organizationId && x.Id == id && !x.IsDeleted)
                    .Select(ToDetail)
                    .FirstOrDefault();
            }
        }
    }

    public async Task<Guid> CreateAsync(RoleCreateRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _storedProcedureExecutor.QuerySingleOrDefaultAsync<RoleIdRow>(
                CreateProcedure,
                new
                {
                    request.OrganizationId,
                    request.Name,
                    request.DisplayName,
                    request.Description,
                    request.Status,
                    request.CreatedBy
                },
                cancellationToken: cancellationToken);

            return row?.Id ?? Guid.Empty;
        }
        catch
        {
            lock (StoreLock)
            {
                EnsureSeedRoles(request.OrganizationId);
                var id = Guid.NewGuid();
                FallbackRoles.Add(new RoleRecord
                {
                    Id = id,
                    OrganizationId = request.OrganizationId,
                    Name = request.Name,
                    DisplayName = request.DisplayName,
                    Description = request.Description ?? string.Empty,
                    Status = request.Status,
                    IsSystem = false,
                    CreatedDateUtc = DateTimeOffset.UtcNow
                });
                RolePermissions[id] = [];
                return id;
            }
        }
    }

    public async Task UpdateAsync(RoleUpdateRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await _storedProcedureExecutor.ExecuteAsync(
                UpdateProcedure,
                new
                {
                    request.Id,
                    request.OrganizationId,
                    request.DisplayName,
                    request.Description,
                    request.Status,
                    request.UpdatedBy
                },
                cancellationToken: cancellationToken);
        }
        catch
        {
            lock (StoreLock)
            {
                EnsureSeedRoles(request.OrganizationId);
                var role = FallbackRoles.FirstOrDefault(x => x.OrganizationId == request.OrganizationId && x.Id == request.Id && !x.IsDeleted);
                if (role is null || role.IsSystem)
                {
                    return;
                }

                role.DisplayName = request.DisplayName;
                role.Description = request.Description ?? string.Empty;
                role.Status = request.Status;
            }
        }
    }

    public async Task SoftDeleteAsync(RoleSoftDeleteRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await _storedProcedureExecutor.ExecuteAsync(
                SoftDeleteProcedure,
                new { request.Id, request.OrganizationId, request.DeletedBy },
                cancellationToken: cancellationToken);
        }
        catch
        {
            lock (StoreLock)
            {
                EnsureSeedRoles(request.OrganizationId);
                var role = FallbackRoles.FirstOrDefault(x => x.OrganizationId == request.OrganizationId && x.Id == request.Id && !x.IsDeleted);
                if (role is null || role.IsSystem)
                {
                    return;
                }

                role.IsDeleted = true;
            }
        }
    }

    public async Task<IReadOnlyList<RolePermissionItemDto>> GetPermissionsAsync(Guid organizationId, Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _storedProcedureExecutor.QueryAsync<RolePermissionItemDto>(
                GetPermissionsProcedure,
                new { OrganizationId = organizationId, RoleId = roleId },
                cancellationToken: cancellationToken);
        }
        catch
        {
            lock (StoreLock)
            {
                EnsureSeedRoles(organizationId);
                var selected = RolePermissions.TryGetValue(roleId, out var values)
                    ? values
                    : [];

                return AccessControlFallbackSeed.Permissions
                    .Select(x => new RolePermissionItemDto
                    {
                        PermissionName = x.Name,
                        DisplayName = x.DisplayName,
                        ModuleName = x.Group,
                        IsGranted = selected.Contains(x.Name)
                    })
                    .ToList();
            }
        }
    }

    public async Task SavePermissionsAsync(RolePermissionSaveRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var permission in request.Permissions)
            {
                await _storedProcedureExecutor.ExecuteAsync(
                    SavePermissionProcedure,
                    new
                    {
                        request.OrganizationId,
                        request.RoleId,
                        permission.PermissionName,
                        permission.IsGranted,
                        request.ChangedBy
                    },
                    cancellationToken: cancellationToken);
            }
        }
        catch
        {
            lock (StoreLock)
            {
                EnsureSeedRoles(request.OrganizationId);
                RolePermissions[request.RoleId] = request.Permissions
                    .Where(x => x.IsGranted)
                    .Select(x => x.PermissionName)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
            }
        }
    }

    internal static void EnsureSeedRoles(Guid organizationId)
    {
        if (organizationId == Guid.Empty || FallbackRoles.Any(x => x.OrganizationId == organizationId))
        {
            return;
        }

        foreach (var seed in AccessControlFallbackSeed.Roles)
        {
            FallbackRoles.Add(new RoleRecord
            {
                Id = seed.Id,
                OrganizationId = organizationId,
                Name = seed.Name,
                DisplayName = seed.DisplayName,
                Description = seed.Description,
                Status = "Active",
                IsSystem = seed.IsSystem,
                CreatedDateUtc = DateTimeOffset.UtcNow.AddDays(-30)
            });

            RolePermissions[seed.Id] = AccessControlFallbackSeed.DefaultPermissionsForRole(seed.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
    }

    internal static IReadOnlyList<RoleRecord> GetFallbackRoles(Guid organizationId)
    {
        lock (StoreLock)
        {
            EnsureSeedRoles(organizationId);
            return FallbackRoles.Where(x => x.OrganizationId == organizationId && !x.IsDeleted).ToList();
        }
    }

    private static RoleListResultDto ListFallback(RoleListFilterRequest request)
    {
        lock (StoreLock)
        {
            EnsureSeedRoles(request.OrganizationId);
            var query = FallbackRoles
                .Where(x => x.OrganizationId == request.OrganizationId && !x.IsDeleted)
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(x =>
                    x.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                    || x.DisplayName.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                    || x.Description.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                query = query.Where(x => x.Status.Equals(request.Status, StringComparison.OrdinalIgnoreCase));
            }

            query = (request.SortColumn, request.SortDirection) switch
            {
                ("Name", "desc") => query.OrderByDescending(x => x.Name),
                ("DisplayName", "desc") => query.OrderByDescending(x => x.DisplayName),
                ("DisplayName", _) => query.OrderBy(x => x.DisplayName),
                ("Status", "desc") => query.OrderByDescending(x => x.Status),
                ("Status", _) => query.OrderBy(x => x.Status),
                ("PermissionCount", "desc") => query.OrderByDescending(x => RolePermissions.TryGetValue(x.Id, out var values) ? values.Count : 0),
                ("PermissionCount", _) => query.OrderBy(x => RolePermissions.TryGetValue(x.Id, out var values) ? values.Count : 0),
                ("CreatedDate", "desc") => query.OrderByDescending(x => x.CreatedDateUtc),
                ("CreatedDate", _) => query.OrderBy(x => x.CreatedDateUtc),
                _ => query.OrderBy(x => x.Name)
            };

            var totalCount = query.LongCount();
            var items = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(ToListItem)
                .ToList();

            return new RoleListResultDto
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }

    private static RoleListItemDto ToListItem(RoleListRow row)
    {
        return new RoleListItemDto
        {
            Id = row.Id,
            OrganizationId = row.OrganizationId,
            Name = row.Name,
            DisplayName = row.DisplayName,
            Description = row.Description ?? string.Empty,
            Status = row.Status,
            IsSystem = row.IsSystem,
            UserCount = row.UserCount,
            PermissionCount = row.PermissionCount,
            CreatedDateUtc = row.CreatedDateUtc
        };
    }

    private static RoleListItemDto ToListItem(RoleRecord record)
    {
        return new RoleListItemDto
        {
            Id = record.Id,
            OrganizationId = record.OrganizationId,
            Name = record.Name,
            DisplayName = record.DisplayName,
            Description = record.Description,
            Status = record.Status,
            IsSystem = record.IsSystem,
            UserCount = UserRoleRepository.CountUsersForRole(record.OrganizationId, record.Id),
            PermissionCount = RolePermissions.TryGetValue(record.Id, out var permissions) ? permissions.Count : 0,
            CreatedDateUtc = record.CreatedDateUtc
        };
    }

    private static RoleDetailDto ToDetail(RoleRecord record)
    {
        return new RoleDetailDto
        {
            Id = record.Id,
            OrganizationId = record.OrganizationId,
            Name = record.Name,
            DisplayName = record.DisplayName,
            Description = record.Description,
            Status = record.Status,
            IsSystem = record.IsSystem
        };
    }

    private sealed class RoleListRow
    {
        public Guid Id { get; init; }
        public Guid OrganizationId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string Status { get; init; } = "Active";
        public bool IsSystem { get; init; }
        public int UserCount { get; init; }
        public int PermissionCount { get; init; }
        public DateTimeOffset CreatedDateUtc { get; init; }
        public long TotalCount { get; init; }
    }

    private sealed class RoleIdRow
    {
        public Guid Id { get; init; }
    }

    internal sealed class RoleRecord
    {
        public Guid Id { get; init; }
        public Guid OrganizationId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public bool IsSystem { get; init; }
        public DateTimeOffset CreatedDateUtc { get; init; }
        public bool IsDeleted { get; set; }
    }
}

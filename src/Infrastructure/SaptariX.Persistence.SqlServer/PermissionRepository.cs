using SaptariX.Persistence.Abstractions;
using SaptariX.Platform.AccessControl.Permissions;

namespace SaptariX.Persistence.SqlServer;

public sealed class PermissionRepository : IPermissionRepository
{
    private const string ListProcedure = "[access].[sp_Permission_List]";
    private const string GetByIdProcedure = "[access].[sp_Permission_GetById]";
    private const string CreateProcedure = "[access].[sp_Permission_Create]";
    private const string UpdateProcedure = "[access].[sp_Permission_Update]";
    private const string DeleteProcedure = "[access].[sp_Permission_Delete]";
    private const string EffectiveProcedure = "[access].[sp_User_EffectivePermissions]";

    private static readonly object StoreLock = new();
    private static readonly List<PermissionRecord> FallbackPermissions = [];

    private readonly IStoredProcedureExecutor _storedProcedureExecutor;

    public PermissionRepository(IStoredProcedureExecutor storedProcedureExecutor)
    {
        _storedProcedureExecutor = storedProcedureExecutor;
    }

    public async Task<PermissionListResultDto> ListAsync(PermissionListFilterRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _storedProcedureExecutor.QueryAsync<PermissionListRow>(
                ListProcedure,
                new
                {
                    request.Search,
                    request.Group,
                    request.PageNumber,
                    request.PageSize,
                    request.SortColumn,
                    request.SortDirection
                },
                cancellationToken: cancellationToken);

            return new PermissionListResultDto
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

    public async Task<PermissionDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _storedProcedureExecutor.QuerySingleOrDefaultAsync<PermissionDetailDto>(
                GetByIdProcedure,
                new { Id = id },
                cancellationToken: cancellationToken);
        }
        catch
        {
            lock (StoreLock)
            {
                EnsureSeedPermissions();
                return FallbackPermissions
                    .Where(x => x.Id == id && !x.IsDeleted)
                    .Select(ToDetail)
                    .FirstOrDefault();
            }
        }
    }

    public async Task<Guid> CreateAsync(PermissionCreateRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _storedProcedureExecutor.QuerySingleOrDefaultAsync<PermissionIdRow>(
                CreateProcedure,
                new
                {
                    request.Name,
                    request.DisplayName,
                    request.Group,
                    request.Description,
                    request.IsSystem,
                    request.CreatedBy
                },
                cancellationToken: cancellationToken);

            return row?.Id ?? Guid.Empty;
        }
        catch
        {
            lock (StoreLock)
            {
                EnsureSeedPermissions();
                var id = Guid.NewGuid();
                FallbackPermissions.Add(new PermissionRecord
                {
                    Id = id,
                    Name = request.Name,
                    DisplayName = request.DisplayName,
                    Group = request.Group,
                    Description = request.Description ?? string.Empty,
                    IsSystem = request.IsSystem,
                    CreatedDateUtc = DateTimeOffset.UtcNow
                });
                return id;
            }
        }
    }

    public async Task UpdateAsync(PermissionUpdateRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await _storedProcedureExecutor.ExecuteAsync(
                UpdateProcedure,
                new
                {
                    request.Id,
                    request.DisplayName,
                    request.Group,
                    request.Description,
                    request.UpdatedBy
                },
                cancellationToken: cancellationToken);
        }
        catch
        {
            lock (StoreLock)
            {
                EnsureSeedPermissions();
                var permission = FallbackPermissions.FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);
                if (permission is null || permission.IsSystem)
                {
                    return;
                }

                permission.DisplayName = request.DisplayName;
                permission.Group = request.Group;
                permission.Description = request.Description ?? string.Empty;
            }
        }
    }

    public async Task DeleteAsync(PermissionDeleteRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await _storedProcedureExecutor.ExecuteAsync(
                DeleteProcedure,
                new { request.Id, request.DeletedBy },
                cancellationToken: cancellationToken);
        }
        catch
        {
            lock (StoreLock)
            {
                EnsureSeedPermissions();
                var permission = FallbackPermissions.FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);
                if (permission is null || permission.IsSystem)
                {
                    return;
                }

                permission.IsDeleted = true;
            }
        }
    }

    public async Task<IReadOnlyList<string>> ListEffectivePermissionNamesAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _storedProcedureExecutor.QueryAsync<PermissionNameRow>(
                EffectiveProcedure,
                new { OrganizationId = organizationId, UserId = userId },
                cancellationToken: cancellationToken);

            return rows.Select(x => x.PermissionName).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }
        catch
        {
            RoleRepository.EnsureSeedRoles(organizationId);
            var roleIds = UserRoleRepository.GetAssignedRoleIds(organizationId, userId);
            if (roleIds.Count == 0)
            {
                return AccessControlFallbackSeed.Permissions.Select(x => x.Name).ToList();
            }

            return roleIds
                .SelectMany(roleId => RoleRepository.RolePermissions.TryGetValue(roleId, out var permissions) ? permissions : [])
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }

    private static PermissionListResultDto ListFallback(PermissionListFilterRequest request)
    {
        lock (StoreLock)
        {
            EnsureSeedPermissions();
            var query = FallbackPermissions.Where(x => !x.IsDeleted).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(x =>
                    x.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                    || x.DisplayName.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                    || x.Group.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.Group))
            {
                query = query.Where(x => x.Group.Equals(request.Group, StringComparison.OrdinalIgnoreCase));
            }

            query = (request.SortColumn, request.SortDirection) switch
            {
                ("Name", "desc") => query.OrderByDescending(x => x.Name),
                ("DisplayName", "desc") => query.OrderByDescending(x => x.DisplayName),
                ("DisplayName", _) => query.OrderBy(x => x.DisplayName),
                ("Group", "desc") => query.OrderByDescending(x => x.Group),
                ("Group", _) => query.OrderBy(x => x.Group),
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

            return new PermissionListResultDto
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }

    private static void EnsureSeedPermissions()
    {
        if (FallbackPermissions.Count > 0)
        {
            return;
        }

        FallbackPermissions.AddRange(AccessControlFallbackSeed.Permissions.Select(seed => new PermissionRecord
        {
            Id = CreateStableGuid(seed.Name),
            Name = seed.Name,
            DisplayName = seed.DisplayName,
            Group = seed.Group,
            Description = seed.DisplayName,
            IsSystem = true,
            CreatedDateUtc = DateTimeOffset.UtcNow.AddDays(-30)
        }));
    }

    private static Guid CreateStableGuid(string value)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        return new Guid(md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value)));
    }

    private static PermissionListItemDto ToListItem(PermissionListRow row)
    {
        return new PermissionListItemDto
        {
            Id = row.Id,
            Name = row.Name,
            DisplayName = row.DisplayName,
            Group = row.Group,
            Description = row.Description ?? string.Empty,
            IsSystem = row.IsSystem,
            CreatedDateUtc = row.CreatedDateUtc
        };
    }

    private static PermissionListItemDto ToListItem(PermissionRecord record)
    {
        return new PermissionListItemDto
        {
            Id = record.Id,
            Name = record.Name,
            DisplayName = record.DisplayName,
            Group = record.Group,
            Description = record.Description,
            IsSystem = record.IsSystem,
            CreatedDateUtc = record.CreatedDateUtc
        };
    }

    private static PermissionDetailDto ToDetail(PermissionRecord record)
    {
        return new PermissionDetailDto
        {
            Id = record.Id,
            Name = record.Name,
            DisplayName = record.DisplayName,
            Group = record.Group,
            Description = record.Description,
            IsSystem = record.IsSystem
        };
    }

    private sealed class PermissionListRow
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public string Group { get; init; } = string.Empty;
        public string? Description { get; init; }
        public bool IsSystem { get; init; }
        public DateTimeOffset CreatedDateUtc { get; init; }
        public long TotalCount { get; init; }
    }

    private sealed class PermissionIdRow
    {
        public Guid Id { get; init; }
    }

    private sealed class PermissionNameRow
    {
        public string PermissionName { get; init; } = string.Empty;
    }

    private sealed class PermissionRecord
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsSystem { get; init; }
        public DateTimeOffset CreatedDateUtc { get; init; }
        public bool IsDeleted { get; set; }
    }
}

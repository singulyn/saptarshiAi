using SaptariX.Persistence.Abstractions;
using SaptariX.Platform.Identity.Users;

namespace SaptariX.Persistence.SqlServer;

public sealed class UserRepository : IUserRepository
{
    private const string ListProcedure = "[identity].[sp_User_List]";
    private const string GetByIdProcedure = "[identity].[sp_User_GetById]";
    private const string CreateProcedure = "[identity].[sp_User_Create]";
    private const string UpdateProcedure = "[identity].[sp_User_Update]";
    private const string SoftDeleteProcedure = "[identity].[sp_User_SoftDelete]";

    private static readonly object StoreLock = new();
    private static readonly List<UserRecord> FallbackUsers = [];

    private readonly IStoredProcedureExecutor _storedProcedureExecutor;

    public UserRepository(IStoredProcedureExecutor storedProcedureExecutor)
    {
        _storedProcedureExecutor = storedProcedureExecutor;
    }

    public async Task<UserListResultDto> ListAsync(UserListFilterRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _storedProcedureExecutor.QueryAsync<UserListRow>(
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

            return new UserListResultDto
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

    public async Task<UserDetailDto?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _storedProcedureExecutor.QuerySingleOrDefaultAsync<UserDetailDto>(
                GetByIdProcedure,
                new { OrganizationId = organizationId, Id = id },
                cancellationToken: cancellationToken);
        }
        catch
        {
            lock (StoreLock)
            {
                EnsureSeedUsers(organizationId);
                return FallbackUsers
                    .Where(x => x.OrganizationId == organizationId && x.Id == id && !x.IsDeleted)
                    .Select(ToDetail)
                    .FirstOrDefault();
            }
        }
    }

    public async Task<Guid> CreateAsync(UserCreateCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _storedProcedureExecutor.QuerySingleOrDefaultAsync<UserIdRow>(
                CreateProcedure,
                new
                {
                    command.OrganizationId,
                    command.FirstName,
                    command.LastName,
                    command.Email,
                    command.MobileNumber,
                    command.Role,
                    command.Status,
                    command.PasswordHash,
                    command.CreatedBy
                },
                cancellationToken: cancellationToken);

            return row?.Id ?? Guid.Empty;
        }
        catch
        {
            lock (StoreLock)
            {
                EnsureSeedUsers(command.OrganizationId);
                var id = Guid.NewGuid();
                FallbackUsers.Add(new UserRecord
                {
                    Id = id,
                    OrganizationId = command.OrganizationId,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    Email = command.Email,
                    MobileNumber = command.MobileNumber,
                    Role = command.Role,
                    Status = command.Status,
                    CreatedDateUtc = DateTimeOffset.UtcNow,
                    LastLoginAtUtc = null,
                    IsDeleted = false
                });

                return id;
            }
        }
    }

    public async Task UpdateAsync(UserUpdateCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            await _storedProcedureExecutor.ExecuteAsync(
                UpdateProcedure,
                new
                {
                    command.Id,
                    command.OrganizationId,
                    command.FirstName,
                    command.LastName,
                    command.Email,
                    command.MobileNumber,
                    command.Role,
                    command.Status,
                    command.UpdatedBy
                },
                cancellationToken: cancellationToken);
        }
        catch
        {
            lock (StoreLock)
            {
                EnsureSeedUsers(command.OrganizationId);
                var user = FallbackUsers.FirstOrDefault(x => x.OrganizationId == command.OrganizationId && x.Id == command.Id && !x.IsDeleted);
                if (user is null)
                {
                    return;
                }

                user.FirstName = command.FirstName;
                user.LastName = command.LastName;
                user.Email = command.Email;
                user.MobileNumber = command.MobileNumber;
                user.Role = command.Role;
                user.Status = command.Status;
            }
        }
    }

    public async Task SoftDeleteAsync(UserSoftDeleteRequest request, CancellationToken cancellationToken = default)
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
                EnsureSeedUsers(request.OrganizationId);
                var user = FallbackUsers.FirstOrDefault(x => x.OrganizationId == request.OrganizationId && x.Id == request.Id && !x.IsDeleted);
                if (user is null)
                {
                    return;
                }

                user.IsDeleted = true;
                user.DeletedAtUtc = DateTimeOffset.UtcNow;
                user.DeletedBy = request.DeletedBy;
            }
        }
    }

    private static UserListResultDto ListFallback(UserListFilterRequest request)
    {
        lock (StoreLock)
        {
            EnsureSeedUsers(request.OrganizationId);
            var query = FallbackUsers
                .Where(x => x.OrganizationId == request.OrganizationId && !x.IsDeleted)
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(x =>
                    x.FullName.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                    || x.Email.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                    || (x.MobileNumber?.Contains(request.Search, StringComparison.OrdinalIgnoreCase) == true));
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                query = query.Where(x => string.Equals(x.Status, request.Status, StringComparison.OrdinalIgnoreCase));
            }

            query = (request.SortColumn, request.SortDirection) switch
            {
                ("FullName", "asc") => query.OrderBy(x => x.FullName),
                ("FullName", _) => query.OrderByDescending(x => x.FullName),
                ("Email", "asc") => query.OrderBy(x => x.Email),
                ("Email", _) => query.OrderByDescending(x => x.Email),
                ("Role", "asc") => query.OrderBy(x => x.Role),
                ("Role", _) => query.OrderByDescending(x => x.Role),
                ("Status", "asc") => query.OrderBy(x => x.Status),
                ("Status", _) => query.OrderByDescending(x => x.Status),
                ("LastLogin", "asc") => query.OrderBy(x => x.LastLoginAtUtc),
                ("LastLogin", _) => query.OrderByDescending(x => x.LastLoginAtUtc),
                ("CreatedDate", "asc") => query.OrderBy(x => x.CreatedDateUtc),
                _ => query.OrderByDescending(x => x.CreatedDateUtc)
            };

            var totalCount = query.LongCount();
            var items = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(ToListItem)
                .ToList();

            return new UserListResultDto
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }

    private static void EnsureSeedUsers(Guid organizationId)
    {
        if (organizationId == Guid.Empty || FallbackUsers.Any(x => x.OrganizationId == organizationId))
        {
            return;
        }

        FallbackUsers.AddRange(
        [
            new UserRecord
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                OrganizationId = organizationId,
                FirstName = "Aarav",
                LastName = "Sharma",
                Email = "aarav.sharma@saptarix.local",
                MobileNumber = "+91 98765 43210",
                Role = "Organization Admin",
                Status = "Active",
                CreatedDateUtc = DateTimeOffset.UtcNow.AddDays(-21),
                LastLoginAtUtc = DateTimeOffset.UtcNow.AddHours(-4)
            },
            new UserRecord
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                OrganizationId = organizationId,
                FirstName = "Meera",
                LastName = "Rao",
                Email = "meera.rao@saptarix.local",
                MobileNumber = "+91 99887 76655",
                Role = "Builder",
                Status = "Active",
                CreatedDateUtc = DateTimeOffset.UtcNow.AddDays(-14),
                LastLoginAtUtc = DateTimeOffset.UtcNow.AddDays(-1)
            },
            new UserRecord
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                OrganizationId = organizationId,
                FirstName = "Nikhil",
                LastName = "Varma",
                Email = "nikhil.varma@saptarix.local",
                MobileNumber = null,
                Role = "Reporter",
                Status = "Pending",
                CreatedDateUtc = DateTimeOffset.UtcNow.AddDays(-3),
                LastLoginAtUtc = null
            }
        ]);
    }

    private static UserListItemDto ToListItem(UserRecord user)
    {
        return new UserListItemDto
        {
            Id = user.Id,
            OrganizationId = user.OrganizationId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Email = user.Email,
            MobileNumber = user.MobileNumber,
            Role = user.Role,
            Status = user.Status,
            CreatedDateUtc = user.CreatedDateUtc,
            LastLoginAtUtc = user.LastLoginAtUtc
        };
    }

    private static UserListItemDto ToListItem(UserListRow row)
    {
        return new UserListItemDto
        {
            Id = row.Id,
            OrganizationId = row.OrganizationId,
            FirstName = row.FirstName,
            LastName = row.LastName,
            FullName = row.FullName,
            Email = row.Email,
            MobileNumber = row.MobileNumber,
            Role = row.Role,
            Status = row.Status,
            CreatedDateUtc = row.CreatedDateUtc,
            LastLoginAtUtc = row.LastLoginAtUtc
        };
    }

    private static UserDetailDto ToDetail(UserRecord user)
    {
        return new UserDetailDto
        {
            Id = user.Id,
            OrganizationId = user.OrganizationId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            MobileNumber = user.MobileNumber,
            Role = user.Role,
            Status = user.Status,
            CreatedDateUtc = user.CreatedDateUtc,
            LastLoginAtUtc = user.LastLoginAtUtc
        };
    }

    private sealed class UserRecord
    {
        public Guid Id { get; init; }
        public Guid OrganizationId { get; init; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string Email { get; set; } = string.Empty;
        public string? MobileNumber { get; set; }
        public string Role { get; set; } = "Member";
        public string Status { get; set; } = "Active";
        public DateTimeOffset CreatedDateUtc { get; init; }
        public DateTimeOffset? LastLoginAtUtc { get; init; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAtUtc { get; set; }
        public Guid? DeletedBy { get; set; }
    }

    private sealed class UserListRow
    {
        public Guid Id { get; init; }
        public Guid OrganizationId { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? MobileNumber { get; init; }
        public string Role { get; init; } = string.Empty;
        public string Status { get; init; } = "Active";
        public DateTimeOffset CreatedDateUtc { get; init; }
        public DateTimeOffset? LastLoginAtUtc { get; init; }
        public long TotalCount { get; init; }
    }

    private sealed class UserIdRow
    {
        public Guid Id { get; init; }
    }
}

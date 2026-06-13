using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SaptariX.Platform.Identity.Users;

public sealed class UserService : IUserService
{
    private static readonly Regex MobilePattern = new(@"^[0-9+\-\s()]{7,20}$", RegexOptions.Compiled);
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository repository, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public Task<UserListResultDto> ListAsync(UserListFilterRequest request, CancellationToken cancellationToken = default)
    {
        request.PageNumber = Math.Max(1, request.PageNumber);
        request.PageSize = Math.Clamp(request.PageSize, 5, 100);
        request.SortColumn = NormalizeSortColumn(request.SortColumn);
        request.SortDirection = string.Equals(request.SortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";
        request.Status = NormalizeOptional(request.Status);
        request.Search = NormalizeOptional(request.Search);

        return _repository.ListAsync(request, cancellationToken);
    }

    public Task<UserDetailDto?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty || id == Guid.Empty)
        {
            return Task.FromResult<UserDetailDto?>(null);
        }

        return _repository.GetByIdAsync(organizationId, id, cancellationToken);
    }

    public async Task<UserCommandResult> CreateAsync(UserCreateRequest request, CancellationToken cancellationToken = default)
    {
        var validation = ValidateCommon(request.OrganizationId, request.FirstName, request.LastName, request.Email, request.MobileNumber, request.Role, request.Status);
        if (validation is not null)
        {
            return UserCommandResult.Failure(validation);
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            return UserCommandResult.Failure("Password must be at least 8 characters.");
        }

        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
        {
            return UserCommandResult.Failure("Password and confirmation password do not match.");
        }

        var command = new UserCreateCommand
        {
            OrganizationId = request.OrganizationId,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            MobileNumber = NormalizeOptional(request.MobileNumber),
            Role = request.Role.Trim(),
            Status = NormalizeStatus(request.Status),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            CreatedBy = request.CreatedBy
        };

        var userId = await _repository.CreateAsync(command, cancellationToken);
        return UserCommandResult.Success("User created successfully.", userId);
    }

    public async Task<UserCommandResult> UpdateAsync(UserUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Id == Guid.Empty)
        {
            return UserCommandResult.Failure("User id is required.");
        }

        var validation = ValidateCommon(request.OrganizationId, request.FirstName, request.LastName, request.Email, request.MobileNumber, request.Role, request.Status);
        if (validation is not null)
        {
            return UserCommandResult.Failure(validation);
        }

        var command = new UserUpdateCommand
        {
            Id = request.Id,
            OrganizationId = request.OrganizationId,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            MobileNumber = NormalizeOptional(request.MobileNumber),
            Role = request.Role.Trim(),
            Status = NormalizeStatus(request.Status),
            UpdatedBy = request.UpdatedBy
        };

        await _repository.UpdateAsync(command, cancellationToken);
        return UserCommandResult.Success("User updated successfully.", request.Id);
    }

    public async Task<UserCommandResult> SoftDeleteAsync(UserSoftDeleteRequest request, CancellationToken cancellationToken = default)
    {
        if (request.OrganizationId == Guid.Empty || request.Id == Guid.Empty)
        {
            return UserCommandResult.Failure("Organization and user id are required.");
        }

        await _repository.SoftDeleteAsync(request, cancellationToken);
        return UserCommandResult.Success("User deleted successfully.", request.Id);
    }

    private static string? ValidateCommon(
        Guid organizationId,
        string firstName,
        string lastName,
        string email,
        string? mobileNumber,
        string role,
        string status)
    {
        if (organizationId == Guid.Empty)
        {
            return "Organization context is required.";
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            return "First name is required.";
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return "Last name is required.";
        }

        if (string.IsNullOrWhiteSpace(email) || !MailAddress.TryCreate(email, out _))
        {
            return "A valid email address is required.";
        }

        if (!string.IsNullOrWhiteSpace(mobileNumber) && !MobilePattern.IsMatch(mobileNumber))
        {
            return "Mobile number is not valid.";
        }

        if (string.IsNullOrWhiteSpace(role))
        {
            return "Role is required.";
        }

        if (!IsSupportedStatus(status))
        {
            return "Status must be Active, Inactive, or Pending.";
        }

        return null;
    }

    private static string NormalizeSortColumn(string? sortColumn)
    {
        return sortColumn?.Trim() switch
        {
            "FullName" => "FullName",
            "Email" => "Email",
            "Role" => "Role",
            "Status" => "Status",
            "CreatedDate" => "CreatedDate",
            "LastLogin" => "LastLogin",
            _ => "CreatedDate"
        };
    }

    private static bool IsSupportedStatus(string? status)
    {
        return string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Inactive", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeStatus(string status)
    {
        return string.Equals(status, "Inactive", StringComparison.OrdinalIgnoreCase)
            ? "Inactive"
            : string.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase)
                ? "Pending"
                : "Active";
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

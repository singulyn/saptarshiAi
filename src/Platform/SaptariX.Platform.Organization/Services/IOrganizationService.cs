using OrganizationEntity = SaptariX.Platform.Organization.Entities.Organization;

namespace SaptariX.Platform.Organization.Services;

public interface IOrganizationService
{
    Task<IReadOnlyList<OrganizationEntity>> GetOrganizationsAsync(CancellationToken cancellationToken = default);
    Task<OrganizationEntity?> GetOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
}

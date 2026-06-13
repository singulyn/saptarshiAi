using OrganizationEntity = SaptariX.Platform.Organization.Entities.Organization;

namespace SaptariX.Platform.Organization.Services;

public sealed class OrganizationService : IOrganizationService
{
    private static readonly IReadOnlyList<OrganizationEntity> SeedOrganizations =
    [
        new("SaptariX Internal", "saptarix-internal"),
        new("Example Organization", "example-organization")
    ];

    public Task<IReadOnlyList<OrganizationEntity>> GetOrganizationsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SeedOrganizations);
    }

    public Task<OrganizationEntity?> GetOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SeedOrganizations.FirstOrDefault(x => x.Id == organizationId));
    }
}

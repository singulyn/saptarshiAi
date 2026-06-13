using SaptariX.Plugin.Abstractions.Organization;

namespace SaptariX.Platform.Organization.Services;

public sealed class CurrentOrganizationService : ICurrentOrganizationService
{
    private readonly IOrganizationResolver _resolver;
    private Guid? _organizationId;

    public CurrentOrganizationService(IOrganizationResolver resolver)
    {
        _resolver = resolver;
    }

    public Guid? OrganizationId => _organizationId ??= _resolver.ResolveOrganizationIdAsync().GetAwaiter().GetResult();

    public string? OrganizationName => OrganizationId.HasValue ? "Current Organization" : null;
}

using SaptariX.Domain.Primitives;

namespace SaptariX.Platform.Organization.Entities;

public sealed class OrganizationModule : OrganizationScopedEntity
{
    public OrganizationModule(Guid organizationId, string moduleName, bool enabled)
    {
        AssignOrganization(organizationId);
        ModuleName = moduleName;
        Enabled = enabled;
    }

    public string ModuleName { get; private set; }
    public bool Enabled { get; private set; }
}

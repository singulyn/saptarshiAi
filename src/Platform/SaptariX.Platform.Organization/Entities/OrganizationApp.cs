using SaptariX.Domain.Primitives;

namespace SaptariX.Platform.Organization.Entities;

public sealed class OrganizationApp : OrganizationScopedEntity
{
    public OrganizationApp(Guid organizationId, string appKey, string displayName)
    {
        AssignOrganization(organizationId);
        AppKey = appKey;
        DisplayName = displayName;
        Enabled = true;
    }

    public string AppKey { get; private set; }
    public string DisplayName { get; private set; }
    public bool Enabled { get; private set; }
}

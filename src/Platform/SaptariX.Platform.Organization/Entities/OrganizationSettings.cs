using SaptariX.Domain.Primitives;

namespace SaptariX.Platform.Organization.Entities;

public sealed class OrganizationSettings : OrganizationScopedEntity
{
    public OrganizationSettings(Guid organizationId, string key, string value)
    {
        AssignOrganization(organizationId);
        Key = key;
        Value = value;
    }

    public string Key { get; private set; }
    public string Value { get; private set; }
}

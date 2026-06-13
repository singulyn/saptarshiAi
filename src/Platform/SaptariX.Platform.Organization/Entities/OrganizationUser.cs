using SaptariX.Domain.Primitives;

namespace SaptariX.Platform.Organization.Entities;

public sealed class OrganizationUser : OrganizationScopedEntity
{
    public OrganizationUser(Guid organizationId, Guid userId, string roleName)
    {
        AssignOrganization(organizationId);
        UserId = userId;
        RoleName = roleName;
    }

    public Guid UserId { get; private set; }
    public string RoleName { get; private set; }
}

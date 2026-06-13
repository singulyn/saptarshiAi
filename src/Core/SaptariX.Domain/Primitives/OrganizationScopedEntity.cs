namespace SaptariX.Domain.Primitives;

public abstract class OrganizationScopedEntity : AuditableEntity
{
    public Guid OrganizationId { get; protected set; }

    protected void AssignOrganization(Guid organizationId)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("OrganizationId is required.", nameof(organizationId));
        }

        OrganizationId = organizationId;
    }
}

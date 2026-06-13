using SaptariX.Domain.Primitives;

namespace SaptariX.Platform.Organization.Entities;

public sealed class Organization : AuditableEntity
{
    public Organization(string name, string slug)
    {
        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        Status = "Active";
    }

    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string Status { get; private set; }

    public void Rename(string name)
    {
        Name = name.Trim();
        MarkUpdated(null);
    }
}

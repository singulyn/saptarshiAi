namespace SaptariX.Domain.Primitives;

public abstract class AuditableEntity : BaseEntity
{
    public DateTimeOffset CreatedAtUtc { get; protected set; } = DateTimeOffset.UtcNow;
    public Guid? CreatedByUserId { get; protected set; }
    public DateTimeOffset? UpdatedAtUtc { get; protected set; }
    public Guid? UpdatedByUserId { get; protected set; }

    public void SetCreatedBy(Guid? userId)
    {
        CreatedByUserId = userId;
    }

    public void MarkUpdated(Guid? userId, DateTimeOffset? atUtc = null)
    {
        UpdatedByUserId = userId;
        UpdatedAtUtc = atUtc ?? DateTimeOffset.UtcNow;
    }
}

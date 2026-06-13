namespace SaptariX.Domain.Primitives;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public bool IsDeleted { get; private set; }

    public void MarkDeleted()
    {
        IsDeleted = true;
    }
}

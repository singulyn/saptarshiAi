namespace SaptariX.Contracts.Events;

public abstract record IntegrationEvent(Guid EventId, DateTimeOffset OccurredAtUtc, Guid? OrganizationId);

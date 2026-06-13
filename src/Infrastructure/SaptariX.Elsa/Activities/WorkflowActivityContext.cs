namespace SaptariX.Elsa.Activities;

public sealed record WorkflowActivityContext(Guid OrganizationId, string WorkflowName, IReadOnlyDictionary<string, object?> Variables);

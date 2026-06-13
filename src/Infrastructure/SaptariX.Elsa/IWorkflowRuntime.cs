namespace SaptariX.Elsa;

public interface IWorkflowRuntime
{
    Task StartWorkflowAsync(string workflowName, Guid organizationId, object? input = null, CancellationToken cancellationToken = default);
}

namespace SaptariX.Elsa.Activities;

public interface IWorkflowActivity
{
    string Name { get; }
    Task ExecuteAsync(WorkflowActivityContext context, CancellationToken cancellationToken = default);
}

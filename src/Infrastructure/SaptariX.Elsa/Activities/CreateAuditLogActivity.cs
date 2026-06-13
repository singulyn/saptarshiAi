namespace SaptariX.Elsa.Activities;

public sealed class CreateAuditLogActivity : IWorkflowActivity
{
    public string Name => "CreateAuditLogActivity";

    public Task ExecuteAsync(WorkflowActivityContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

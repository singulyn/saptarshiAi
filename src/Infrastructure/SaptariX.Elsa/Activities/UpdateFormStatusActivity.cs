namespace SaptariX.Elsa.Activities;

public sealed class UpdateFormStatusActivity : IWorkflowActivity
{
    public string Name => "UpdateFormStatusActivity";

    public Task ExecuteAsync(WorkflowActivityContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

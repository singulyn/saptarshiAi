namespace SaptariX.Elsa.Activities;

public sealed class SendNotificationActivity : IWorkflowActivity
{
    public string Name => "SendNotificationActivity";

    public Task ExecuteAsync(WorkflowActivityContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

namespace SaptariX.Elsa.Activities;

public sealed class FormSubmittedActivity : IWorkflowActivity
{
    public string Name => "FormSubmittedActivity";

    public Task ExecuteAsync(WorkflowActivityContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

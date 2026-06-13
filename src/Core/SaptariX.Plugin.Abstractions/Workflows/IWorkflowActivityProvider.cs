namespace SaptariX.Plugin.Abstractions.Workflows;

public interface IWorkflowActivityProvider
{
    void RegisterWorkflowActivities(IWorkflowActivityRegistry registry);
}

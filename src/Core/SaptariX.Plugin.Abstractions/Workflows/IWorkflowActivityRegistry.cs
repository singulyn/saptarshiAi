namespace SaptariX.Plugin.Abstractions.Workflows;

public interface IWorkflowActivityRegistry
{
    void Add(WorkflowActivityDefinition activity);
    IReadOnlyList<WorkflowActivityDefinition> GetActivities();
}

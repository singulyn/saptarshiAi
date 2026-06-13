using SaptariX.Plugin.Abstractions.Workflows;

namespace SaptariX.Platform.ModuleRegistry;

public sealed class InMemoryWorkflowActivityRegistry : IWorkflowActivityRegistry
{
    private readonly List<WorkflowActivityDefinition> _activities = [];

    public void Add(WorkflowActivityDefinition activity)
    {
        _activities.RemoveAll(x => x.Name.Equals(activity.Name, StringComparison.OrdinalIgnoreCase));
        _activities.Add(activity);
    }

    public IReadOnlyList<WorkflowActivityDefinition> GetActivities()
    {
        return _activities.OrderBy(x => x.ModuleName).ThenBy(x => x.Name).ToList();
    }
}

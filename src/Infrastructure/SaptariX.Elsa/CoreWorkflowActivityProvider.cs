using SaptariX.Elsa.Activities;
using SaptariX.Plugin.Abstractions.Workflows;

namespace SaptariX.Elsa;

public sealed class CoreWorkflowActivityProvider : IWorkflowActivityProvider
{
    public void RegisterWorkflowActivities(IWorkflowActivityRegistry registry)
    {
        registry.Add(new WorkflowActivityDefinition("SendNotificationActivity", "Send Notification", "Core", typeof(SendNotificationActivity)));
        registry.Add(new WorkflowActivityDefinition("UpdateFormStatusActivity", "Update Form Status", "Core", typeof(UpdateFormStatusActivity)));
        registry.Add(new WorkflowActivityDefinition("CreateAuditLogActivity", "Create Audit Log", "Core", typeof(CreateAuditLogActivity)));
    }
}

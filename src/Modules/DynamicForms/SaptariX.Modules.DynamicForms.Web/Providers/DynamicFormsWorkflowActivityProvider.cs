using SaptariX.Elsa.Activities;
using SaptariX.Plugin.Abstractions.Workflows;

namespace SaptariX.Modules.DynamicForms.Web.Providers;

public sealed class DynamicFormsWorkflowActivityProvider : IWorkflowActivityProvider
{
    public void RegisterWorkflowActivities(IWorkflowActivityRegistry registry)
    {
        registry.Add(new WorkflowActivityDefinition("FormSubmittedActivity", "Form Submitted", "DynamicForms", typeof(FormSubmittedActivity)));
    }
}

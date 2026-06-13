using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SaptariX.Elsa;

public sealed class WorkflowRuntime : IWorkflowRuntime
{
    private readonly ElsaWorkflowOptions _options;
    private readonly ILogger<WorkflowRuntime> _logger;

    public WorkflowRuntime(IOptions<ElsaWorkflowOptions> options, ILogger<WorkflowRuntime> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task StartWorkflowAsync(
        string workflowName,
        Guid organizationId,
        object? input = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Workflow {WorkflowName} requested for organization {OrganizationId}. SQL persistence enabled: {UseSqlServerPersistence}",
            workflowName,
            organizationId,
            _options.UseSqlServerPersistence);

        return Task.CompletedTask;
    }
}

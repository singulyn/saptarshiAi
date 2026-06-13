using SaptariX.Modules.DynamicForms.Application;
using SaptariX.Modules.DynamicForms.Contracts;
using SaptariX.Persistence.Abstractions;

namespace SaptariX.Modules.DynamicForms.Infrastructure;

public sealed class DynamicFormRepository : IDynamicFormRepository
{
    private readonly IStoredProcedureExecutor _storedProcedureExecutor;

    public DynamicFormRepository(IStoredProcedureExecutor storedProcedureExecutor)
    {
        _storedProcedureExecutor = storedProcedureExecutor;
    }

    public async Task<IReadOnlyList<DynamicFormSummaryDto>> GetFormsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty)
        {
            return CreateSeedForms(organizationId);
        }

        IReadOnlyList<DynamicFormSummaryDto> rows;
        try
        {
            rows = await _storedProcedureExecutor.QueryAsync<DynamicFormSummaryDto>(
                "forms.GetDynamicForms",
                new { OrganizationId = organizationId },
                cancellationToken: cancellationToken);
        }
        catch
        {
            rows = [];
        }

        return rows.Count > 0 ? rows : CreateSeedForms(organizationId);
    }

    private static IReadOnlyList<DynamicFormSummaryDto> CreateSeedForms(Guid organizationId)
    {
        return
        [
            new(Guid.NewGuid(), organizationId, "Employee Onboarding", "Published", 42, DateTimeOffset.UtcNow.AddDays(-1)),
            new(Guid.NewGuid(), organizationId, "Vendor Registration", "Draft", 8, DateTimeOffset.UtcNow.AddDays(-3))
        ];
    }
}

using SaptariX.Modules.DynamicForms.Contracts;

namespace SaptariX.Modules.DynamicForms.Application;

public interface IDynamicFormRepository
{
    Task<IReadOnlyList<DynamicFormSummaryDto>> GetFormsAsync(Guid organizationId, CancellationToken cancellationToken = default);
}

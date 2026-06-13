using SaptariX.Modules.DynamicForms.Contracts;

namespace SaptariX.Modules.DynamicForms.Application;

public sealed class DynamicFormService : IDynamicFormService
{
    private readonly IDynamicFormRepository _repository;

    public DynamicFormService(IDynamicFormRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<DynamicFormSummaryDto>> GetFormsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetFormsAsync(organizationId, cancellationToken);
    }
}

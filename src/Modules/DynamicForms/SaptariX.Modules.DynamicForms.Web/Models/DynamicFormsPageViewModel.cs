using SaptariX.Modules.DynamicForms.Contracts;

namespace SaptariX.Modules.DynamicForms.Web.Models;

public sealed record DynamicFormsPageViewModel(IReadOnlyList<DynamicFormSummaryDto> Forms);

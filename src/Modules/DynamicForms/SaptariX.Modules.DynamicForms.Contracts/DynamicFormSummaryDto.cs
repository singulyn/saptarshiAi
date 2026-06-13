namespace SaptariX.Modules.DynamicForms.Contracts;

public sealed record DynamicFormSummaryDto(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string Status,
    int SubmissionCount,
    DateTimeOffset UpdatedAtUtc);

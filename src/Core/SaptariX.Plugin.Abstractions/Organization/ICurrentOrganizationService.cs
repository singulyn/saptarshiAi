namespace SaptariX.Plugin.Abstractions.Organization;

public interface ICurrentOrganizationService
{
    Guid? OrganizationId { get; }
    string? OrganizationName { get; }
}

namespace SaptariX.Plugin.Abstractions.Organization;

public interface IOrganizationResolver
{
    Task<Guid?> ResolveOrganizationIdAsync(CancellationToken cancellationToken = default);
}

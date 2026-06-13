using Microsoft.AspNetCore.Http;
using SaptariX.Plugin.Abstractions.Organization;
using SaptariX.Shared.Constants;

namespace SaptariX.Platform.Organization.Services;

public sealed class HeaderOrganizationResolver : IOrganizationResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HeaderOrganizationResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Guid?> ResolveOrganizationIdAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return Task.FromResult<Guid?>(null);
        }

        if (httpContext.Request.Headers.TryGetValue(PlatformConstants.DefaultOrganizationHeader, out var value) &&
            Guid.TryParse(value.FirstOrDefault(), out var headerOrganizationId))
        {
            return Task.FromResult<Guid?>(headerOrganizationId);
        }

        if (httpContext.Request.Cookies.TryGetValue("SaptariX.OrganizationId", out var cookieValue) &&
            Guid.TryParse(cookieValue, out var cookieOrganizationId))
        {
            return Task.FromResult<Guid?>(cookieOrganizationId);
        }

        return Task.FromResult<Guid?>(null);
    }
}

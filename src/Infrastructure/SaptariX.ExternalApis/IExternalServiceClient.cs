namespace SaptariX.ExternalApis;

public interface IExternalServiceClient
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}

namespace SaptariX.Auth.Api.Contracts;

public sealed record LoginResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAtUtc);

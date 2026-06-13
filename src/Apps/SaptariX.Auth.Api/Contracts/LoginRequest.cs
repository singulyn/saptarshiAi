namespace SaptariX.Auth.Api.Contracts;

public sealed record LoginRequest(string Email, string Password, Guid? OrganizationId);

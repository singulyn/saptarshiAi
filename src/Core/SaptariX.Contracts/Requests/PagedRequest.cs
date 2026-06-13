namespace SaptariX.Contracts.Requests;

public sealed record PagedRequest(int PageNumber = 1, int PageSize = 25);

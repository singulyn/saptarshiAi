CREATE OR ALTER PROCEDURE forms.GetDynamicForms
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        f.Id,
        f.OrganizationId,
        f.Name,
        f.Status,
        CAST(0 AS INT) AS SubmissionCount,
        COALESCE(f.UpdatedAtUtc, f.CreatedAtUtc) AS UpdatedAtUtc
    FROM forms.DynamicForms AS f
    WHERE f.OrganizationId = @OrganizationId
    ORDER BY f.Name;
END
GO

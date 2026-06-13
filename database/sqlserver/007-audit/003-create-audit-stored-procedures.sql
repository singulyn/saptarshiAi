USE SaptariX_Platform;
GO

CREATE OR ALTER PROCEDURE audit.CreateAuditLog
    @OrganizationId UNIQUEIDENTIFIER = NULL,
    @UserId UNIQUEIDENTIFIER = NULL,
    @Action NVARCHAR(200),
    @EntityName NVARCHAR(200),
    @EntityId NVARCHAR(100) = NULL,
    @PayloadJson NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO audit.AuditLogs (Id, OrganizationId, UserId, Action, EntityName, EntityId, PayloadJson)
    VALUES (NEWID(), @OrganizationId, @UserId, @Action, @EntityName, @EntityId, @PayloadJson);
END
GO

USE SaptariX_Platform;
GO

IF OBJECT_ID('audit.AuditLogs', 'U') IS NULL
BEGIN
    CREATE TABLE audit.AuditLogs
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_audit_AuditLogs PRIMARY KEY,
        OrganizationId UNIQUEIDENTIFIER NULL,
        UserId UNIQUEIDENTIFIER NULL,
        Action NVARCHAR(200) NOT NULL,
        EntityName NVARCHAR(200) NOT NULL,
        EntityId NVARCHAR(100) NULL,
        PayloadJson NVARCHAR(MAX) NULL,
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_audit_AuditLogs_CreatedAtUtc DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_audit_AuditLogs_OrganizationId_CreatedAtUtc
        ON audit.AuditLogs (OrganizationId, CreatedAtUtc DESC);
END
GO

USE SaptariX_Platform;
GO

IF OBJECT_ID('org.Organizations', 'U') IS NULL
BEGIN
    CREATE TABLE org.Organizations
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_org_Organizations PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Slug NVARCHAR(120) NOT NULL,
        Status NVARCHAR(40) NOT NULL CONSTRAINT DF_org_Organizations_Status DEFAULT ('Active'),
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_org_Organizations_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc DATETIMEOFFSET NULL,
        CONSTRAINT UQ_org_Organizations_Slug UNIQUE (Slug)
    );
END
GO

CREATE OR ALTER PROCEDURE org.GetOrganizations
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Slug, Status, CreatedAtUtc, UpdatedAtUtc
    FROM org.Organizations
    ORDER BY Name;
END
GO

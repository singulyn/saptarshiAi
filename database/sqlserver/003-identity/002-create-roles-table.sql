USE SaptariX_Platform;
GO

IF OBJECT_ID('id.Roles', 'U') IS NULL
BEGIN
    CREATE TABLE id.Roles
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_id_Roles PRIMARY KEY,
        OrganizationId UNIQUEIDENTIFIER NULL,
        Name NVARCHAR(100) NOT NULL,
        DisplayName NVARCHAR(200) NOT NULL,
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_id_Roles_CreatedAtUtc DEFAULT SYSUTCDATETIME()
    );

    CREATE UNIQUE INDEX UX_id_Roles_OrganizationId_Name
        ON id.Roles (OrganizationId, Name)
        WHERE OrganizationId IS NOT NULL;
END
GO

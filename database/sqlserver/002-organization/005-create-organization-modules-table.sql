USE SaptariX_Platform;
GO

IF OBJECT_ID('org.OrganizationModules', 'U') IS NULL
BEGIN
    CREATE TABLE org.OrganizationModules
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_org_OrganizationModules PRIMARY KEY,
        OrganizationId UNIQUEIDENTIFIER NOT NULL,
        ModuleName NVARCHAR(100) NOT NULL,
        IsEnabled BIT NOT NULL CONSTRAINT DF_org_OrganizationModules_IsEnabled DEFAULT (1),
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_org_OrganizationModules_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_org_OrganizationModules_Organizations FOREIGN KEY (OrganizationId) REFERENCES org.Organizations(Id),
        CONSTRAINT UQ_org_OrganizationModules UNIQUE (OrganizationId, ModuleName)
    );
END
GO

USE SaptariX_Platform;
GO

IF OBJECT_ID('org.OrganizationSettings', 'U') IS NULL
BEGIN
    CREATE TABLE org.OrganizationSettings
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_org_OrganizationSettings PRIMARY KEY,
        OrganizationId UNIQUEIDENTIFIER NOT NULL,
        [Key] NVARCHAR(150) NOT NULL,
        [Value] NVARCHAR(MAX) NOT NULL,
        UpdatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_org_OrganizationSettings_UpdatedAtUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_org_OrganizationSettings_Organizations FOREIGN KEY (OrganizationId) REFERENCES org.Organizations(Id),
        CONSTRAINT UQ_org_OrganizationSettings UNIQUE (OrganizationId, [Key])
    );
END
GO

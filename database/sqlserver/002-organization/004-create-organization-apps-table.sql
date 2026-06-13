USE SaptariX_Platform;
GO

IF OBJECT_ID('org.OrganizationApps', 'U') IS NULL
BEGIN
    CREATE TABLE org.OrganizationApps
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_org_OrganizationApps PRIMARY KEY,
        OrganizationId UNIQUEIDENTIFIER NOT NULL,
        AppKey NVARCHAR(100) NOT NULL,
        DisplayName NVARCHAR(200) NOT NULL,
        IsEnabled BIT NOT NULL CONSTRAINT DF_org_OrganizationApps_IsEnabled DEFAULT (1),
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_org_OrganizationApps_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_org_OrganizationApps_Organizations FOREIGN KEY (OrganizationId) REFERENCES org.Organizations(Id),
        CONSTRAINT UQ_org_OrganizationApps UNIQUE (OrganizationId, AppKey)
    );
END
GO

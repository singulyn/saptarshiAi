USE SaptariX_Platform;
GO

IF OBJECT_ID('org.OrganizationUsers', 'U') IS NULL
BEGIN
    CREATE TABLE org.OrganizationUsers
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_org_OrganizationUsers PRIMARY KEY,
        OrganizationId UNIQUEIDENTIFIER NOT NULL,
        UserId UNIQUEIDENTIFIER NOT NULL,
        RoleName NVARCHAR(100) NOT NULL,
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_org_OrganizationUsers_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_org_OrganizationUsers_Organizations FOREIGN KEY (OrganizationId) REFERENCES org.Organizations(Id),
        CONSTRAINT UQ_org_OrganizationUsers UNIQUE (OrganizationId, UserId)
    );
END
GO

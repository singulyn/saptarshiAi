USE SaptariX_Platform;
GO

IF OBJECT_ID('id.UserRoles', 'U') IS NULL
BEGIN
    CREATE TABLE id.UserRoles
    (
        UserId UNIQUEIDENTIFIER NOT NULL,
        RoleId UNIQUEIDENTIFIER NOT NULL,
        OrganizationId UNIQUEIDENTIFIER NOT NULL,
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_id_UserRoles_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_id_UserRoles PRIMARY KEY (UserId, RoleId, OrganizationId),
        CONSTRAINT FK_id_UserRoles_Users FOREIGN KEY (UserId) REFERENCES id.Users(Id),
        CONSTRAINT FK_id_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES id.Roles(Id),
        CONSTRAINT FK_id_UserRoles_Organizations FOREIGN KEY (OrganizationId) REFERENCES org.Organizations(Id)
    );
END
GO

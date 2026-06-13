USE SaptariX_Platform;
GO

IF OBJECT_ID('id.RolePermissions', 'U') IS NULL
BEGIN
    CREATE TABLE id.RolePermissions
    (
        RoleId UNIQUEIDENTIFIER NOT NULL,
        PermissionId UNIQUEIDENTIFIER NOT NULL,
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_id_RolePermissions_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_id_RolePermissions PRIMARY KEY (RoleId, PermissionId),
        CONSTRAINT FK_id_RolePermissions_Roles FOREIGN KEY (RoleId) REFERENCES id.Roles(Id),
        CONSTRAINT FK_id_RolePermissions_Permissions FOREIGN KEY (PermissionId) REFERENCES id.Permissions(Id)
    );
END
GO

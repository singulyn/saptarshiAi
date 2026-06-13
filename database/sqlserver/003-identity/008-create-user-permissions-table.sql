USE SaptariX_Platform;
GO

IF OBJECT_ID('id.UserPermissions', 'U') IS NULL
BEGIN
    CREATE TABLE id.UserPermissions
    (
        UserId UNIQUEIDENTIFIER NOT NULL,
        OrganizationId UNIQUEIDENTIFIER NOT NULL,
        PermissionName NVARCHAR(200) NOT NULL,
        IsGranted BIT NOT NULL,
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_id_UserPermissions_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc DATETIMEOFFSET NULL,
        UpdatedBy UNIQUEIDENTIFIER NULL,
        CONSTRAINT PK_id_UserPermissions PRIMARY KEY (UserId, OrganizationId, PermissionName),
        CONSTRAINT FK_id_UserPermissions_Users FOREIGN KEY (UserId) REFERENCES id.Users(Id)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_id_UserPermissions_OrganizationId_UserId' AND object_id = OBJECT_ID('id.UserPermissions'))
BEGIN
    CREATE INDEX IX_id_UserPermissions_OrganizationId_UserId
        ON id.UserPermissions (OrganizationId, UserId);
END
GO

MERGE id.Permissions AS target
USING
(
    VALUES
        ('Users.View', 'View users', 'Users'),
        ('Users.Create', 'Create users', 'Users'),
        ('Users.Update', 'Update users', 'Users'),
        ('Users.Delete', 'Delete users', 'Users'),
        ('Users.ManagePermissions', 'Manage user permissions', 'Users'),
        ('Roles.View', 'View roles', 'Roles'),
        ('Roles.Assign', 'Assign roles', 'Roles'),
        ('Reports.View', 'View reports', 'Reports'),
        ('Settings.View', 'View settings', 'Settings')
) AS source (Name, DisplayName, [Group])
ON target.Name = source.Name
WHEN NOT MATCHED THEN
    INSERT (Id, Name, DisplayName, [Group])
    VALUES (NEWID(), source.Name, source.DisplayName, source.[Group]);
GO

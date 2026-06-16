IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'access')
BEGIN
    EXEC(N'CREATE SCHEMA [access]');
END
GO

IF OBJECT_ID(N'[access].[Permissions]', N'U') IS NULL
BEGIN
    CREATE TABLE [access].[Permissions]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_access_Permissions] PRIMARY KEY,
        [Name] NVARCHAR(160) NOT NULL,
        [DisplayName] NVARCHAR(200) NOT NULL,
        [Group] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [IsSystem] BIT NOT NULL CONSTRAINT [DF_access_Permissions_IsSystem] DEFAULT (0),
        [CreatedDateUtc] DATETIMEOFFSET(7) NOT NULL CONSTRAINT [DF_access_Permissions_CreatedDateUtc] DEFAULT (SYSUTCDATETIME()),
        [CreatedBy] UNIQUEIDENTIFIER NULL,
        [UpdatedDateUtc] DATETIMEOFFSET(7) NULL,
        [UpdatedBy] UNIQUEIDENTIFIER NULL,
        [DeletedAtUtc] DATETIMEOFFSET(7) NULL,
        [DeletedBy] UNIQUEIDENTIFIER NULL,
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_access_Permissions_IsDeleted] DEFAULT (0)
    );

    CREATE UNIQUE INDEX [UX_access_Permissions_Name]
        ON [access].[Permissions]([Name])
        WHERE [IsDeleted] = 0;
END
GO

IF OBJECT_ID(N'[access].[Roles]', N'U') IS NULL
BEGIN
    CREATE TABLE [access].[Roles]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_access_Roles] PRIMARY KEY,
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
        [Name] NVARCHAR(120) NOT NULL,
        [DisplayName] NVARCHAR(160) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [Status] NVARCHAR(40) NOT NULL CONSTRAINT [DF_access_Roles_Status] DEFAULT (N'Active'),
        [IsSystem] BIT NOT NULL CONSTRAINT [DF_access_Roles_IsSystem] DEFAULT (0),
        [CreatedDateUtc] DATETIMEOFFSET(7) NOT NULL CONSTRAINT [DF_access_Roles_CreatedDateUtc] DEFAULT (SYSUTCDATETIME()),
        [CreatedBy] UNIQUEIDENTIFIER NULL,
        [UpdatedDateUtc] DATETIMEOFFSET(7) NULL,
        [UpdatedBy] UNIQUEIDENTIFIER NULL,
        [DeletedAtUtc] DATETIMEOFFSET(7) NULL,
        [DeletedBy] UNIQUEIDENTIFIER NULL,
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_access_Roles_IsDeleted] DEFAULT (0)
    );

    CREATE UNIQUE INDEX [UX_access_Roles_Organization_Name]
        ON [access].[Roles]([OrganizationId], [Name])
        WHERE [IsDeleted] = 0;
END
GO

IF OBJECT_ID(N'[access].[UserRoles]', N'U') IS NULL
BEGIN
    CREATE TABLE [access].[UserRoles]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_access_UserRoles] PRIMARY KEY,
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [RoleId] UNIQUEIDENTIFIER NOT NULL,
        [CreatedDateUtc] DATETIMEOFFSET(7) NOT NULL CONSTRAINT [DF_access_UserRoles_CreatedDateUtc] DEFAULT (SYSUTCDATETIME()),
        [CreatedBy] UNIQUEIDENTIFIER NULL,
        CONSTRAINT [FK_access_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [access].[Roles]([Id])
    );

    CREATE UNIQUE INDEX [UX_access_UserRoles_User_Role]
        ON [access].[UserRoles]([OrganizationId], [UserId], [RoleId]);
END
GO

IF OBJECT_ID(N'[access].[RolePermissions]', N'U') IS NULL
BEGIN
    CREATE TABLE [access].[RolePermissions]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_access_RolePermissions] PRIMARY KEY,
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
        [RoleId] UNIQUEIDENTIFIER NOT NULL,
        [PermissionId] UNIQUEIDENTIFIER NOT NULL,
        [CreatedDateUtc] DATETIMEOFFSET(7) NOT NULL CONSTRAINT [DF_access_RolePermissions_CreatedDateUtc] DEFAULT (SYSUTCDATETIME()),
        [CreatedBy] UNIQUEIDENTIFIER NULL,
        CONSTRAINT [FK_access_RolePermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [access].[Roles]([Id]),
        CONSTRAINT [FK_access_RolePermissions_Permissions] FOREIGN KEY ([PermissionId]) REFERENCES [access].[Permissions]([Id])
    );

    CREATE UNIQUE INDEX [UX_access_RolePermissions_Role_Permission]
        ON [access].[RolePermissions]([OrganizationId], [RoleId], [PermissionId]);
END
GO

IF OBJECT_ID(N'[access].[UserPermissions]', N'U') IS NULL
BEGIN
    CREATE TABLE [access].[UserPermissions]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_access_UserPermissions] PRIMARY KEY,
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [PermissionId] UNIQUEIDENTIFIER NOT NULL,
        [IsGranted] BIT NOT NULL,
        [ChangedDateUtc] DATETIMEOFFSET(7) NOT NULL CONSTRAINT [DF_access_UserPermissions_ChangedDateUtc] DEFAULT (SYSUTCDATETIME()),
        [ChangedBy] UNIQUEIDENTIFIER NULL,
        CONSTRAINT [FK_access_UserPermissions_Permissions] FOREIGN KEY ([PermissionId]) REFERENCES [access].[Permissions]([Id])
    );

    CREATE UNIQUE INDEX [UX_access_UserPermissions_User_Permission]
        ON [access].[UserPermissions]([OrganizationId], [UserId], [PermissionId]);
END
GO

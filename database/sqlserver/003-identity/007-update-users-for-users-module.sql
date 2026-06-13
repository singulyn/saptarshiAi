USE SaptariX_Platform;
GO

IF SCHEMA_ID('id') IS NULL
    EXEC('CREATE SCHEMA id');
GO

IF SCHEMA_ID('identity') IS NULL
    EXEC('CREATE SCHEMA [identity]');
GO

IF OBJECT_ID('id.Users', 'U') IS NULL
BEGIN
    CREATE TABLE id.Users
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_id_Users PRIMARY KEY,
        OrganizationId UNIQUEIDENTIFIER NULL,
        Email NVARCHAR(320) NOT NULL,
        FirstName NVARCHAR(100) NULL,
        LastName NVARCHAR(100) NULL,
        DisplayName NVARCHAR(200) NOT NULL,
        MobileNumber NVARCHAR(30) NULL,
        RoleName NVARCHAR(100) NULL,
        Status NVARCHAR(40) NOT NULL CONSTRAINT DF_id_Users_Status DEFAULT ('Active'),
        PasswordHash NVARCHAR(500) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_id_Users_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_id_Users_IsDeleted DEFAULT (0),
        DeletedAtUtc DATETIMEOFFSET NULL,
        DeletedBy UNIQUEIDENTIFIER NULL,
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_id_Users_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc DATETIMEOFFSET NULL,
        LastLoginAtUtc DATETIMEOFFSET NULL,
        CONSTRAINT UQ_id_Users_Email UNIQUE (Email)
    );
END
GO

IF COL_LENGTH('id.Users', 'OrganizationId') IS NULL
    ALTER TABLE id.Users ADD OrganizationId UNIQUEIDENTIFIER NULL;
GO

IF COL_LENGTH('id.Users', 'FirstName') IS NULL
    ALTER TABLE id.Users ADD FirstName NVARCHAR(100) NULL;
GO

IF COL_LENGTH('id.Users', 'LastName') IS NULL
    ALTER TABLE id.Users ADD LastName NVARCHAR(100) NULL;
GO

IF COL_LENGTH('id.Users', 'MobileNumber') IS NULL
    ALTER TABLE id.Users ADD MobileNumber NVARCHAR(30) NULL;
GO

IF COL_LENGTH('id.Users', 'RoleName') IS NULL
    ALTER TABLE id.Users ADD RoleName NVARCHAR(100) NULL;
GO

IF COL_LENGTH('id.Users', 'Status') IS NULL
    ALTER TABLE id.Users ADD Status NVARCHAR(40) NOT NULL CONSTRAINT DF_id_Users_Status DEFAULT ('Active');
GO

IF COL_LENGTH('id.Users', 'IsDeleted') IS NULL
    ALTER TABLE id.Users ADD IsDeleted BIT NOT NULL CONSTRAINT DF_id_Users_IsDeleted DEFAULT (0);
GO

IF COL_LENGTH('id.Users', 'DeletedAtUtc') IS NULL
    ALTER TABLE id.Users ADD DeletedAtUtc DATETIMEOFFSET NULL;
GO

IF COL_LENGTH('id.Users', 'DeletedBy') IS NULL
    ALTER TABLE id.Users ADD DeletedBy UNIQUEIDENTIFIER NULL;
GO

IF COL_LENGTH('id.Users', 'UpdatedAtUtc') IS NULL
    ALTER TABLE id.Users ADD UpdatedAtUtc DATETIMEOFFSET NULL;
GO

IF COL_LENGTH('id.Users', 'LastLoginAtUtc') IS NULL
    ALTER TABLE id.Users ADD LastLoginAtUtc DATETIMEOFFSET NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_id_Users_OrganizationId_IsDeleted' AND object_id = OBJECT_ID('id.Users'))
BEGIN
    CREATE INDEX IX_id_Users_OrganizationId_IsDeleted
        ON id.Users (OrganizationId, IsDeleted, CreatedAtUtc DESC);
END
GO

USE SaptariX_Platform;
GO

IF OBJECT_ID('core.Applications', 'U') IS NULL
BEGIN
    CREATE TABLE core.Applications
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_core_Applications PRIMARY KEY,
        AppKey NVARCHAR(100) NOT NULL,
        DisplayName NVARCHAR(200) NOT NULL,
        IsEnabled BIT NOT NULL CONSTRAINT DF_core_Applications_IsEnabled DEFAULT (1),
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_core_Applications_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_core_Applications_AppKey UNIQUE (AppKey)
    );
END
GO

IF OBJECT_ID('core.Modules', 'U') IS NULL
BEGIN
    CREATE TABLE core.Modules
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_core_Modules PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        DisplayName NVARCHAR(200) NOT NULL,
        Version NVARCHAR(50) NOT NULL,
        IsEnabled BIT NOT NULL CONSTRAINT DF_core_Modules_IsEnabled DEFAULT (1),
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_core_Modules_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_core_Modules_Name UNIQUE (Name)
    );
END
GO

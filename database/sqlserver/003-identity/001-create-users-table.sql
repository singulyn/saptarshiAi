USE SaptariX_Platform;
GO

IF SCHEMA_ID('id') IS NULL
    EXEC('CREATE SCHEMA id');
GO

IF OBJECT_ID('id.Users', 'U') IS NULL
BEGIN
    CREATE TABLE id.Users
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_id_Users PRIMARY KEY,
        Email NVARCHAR(320) NOT NULL,
        DisplayName NVARCHAR(200) NOT NULL,
        PasswordHash NVARCHAR(500) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_id_Users_IsActive DEFAULT (1),
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_id_Users_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_id_Users_Email UNIQUE (Email)
    );
END
GO

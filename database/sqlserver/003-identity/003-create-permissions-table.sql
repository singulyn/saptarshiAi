USE SaptariX_Platform;
GO

IF OBJECT_ID('id.Permissions', 'U') IS NULL
BEGIN
    CREATE TABLE id.Permissions
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_id_Permissions PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        DisplayName NVARCHAR(200) NOT NULL,
        [Group] NVARCHAR(100) NOT NULL,
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_id_Permissions_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_id_Permissions_Name UNIQUE (Name)
    );
END
GO

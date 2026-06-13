USE SaptariX_Platform;
GO

IF SCHEMA_ID('audit') IS NULL
    EXEC('CREATE SCHEMA audit');
GO

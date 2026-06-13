USE SaptariX_Platform;
GO

IF SCHEMA_ID('org') IS NULL
    EXEC('CREATE SCHEMA org');
GO

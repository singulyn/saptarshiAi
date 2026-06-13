USE SaptariX_Platform;
GO

IF SCHEMA_ID('elsa') IS NULL
    EXEC('CREATE SCHEMA elsa');
GO

-- Elsa runtime tables are created by the Elsa SQL Server persistence provider
-- during the workflow infrastructure phase. This schema keeps database ownership
-- explicit from day one.

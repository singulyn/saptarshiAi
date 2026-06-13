IF SCHEMA_ID('forms') IS NULL
    EXEC('CREATE SCHEMA forms');
GO

IF OBJECT_ID('forms.DynamicForms', 'U') IS NULL
BEGIN
    CREATE TABLE forms.DynamicForms
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_forms_DynamicForms PRIMARY KEY,
        OrganizationId UNIQUEIDENTIFIER NOT NULL,
        Name NVARCHAR(200) NOT NULL,
        Status NVARCHAR(50) NOT NULL CONSTRAINT DF_forms_DynamicForms_Status DEFAULT ('Draft'),
        SchemaJson NVARCHAR(MAX) NOT NULL,
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_forms_DynamicForms_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc DATETIMEOFFSET NULL
    );

    CREATE INDEX IX_forms_DynamicForms_OrganizationId
        ON forms.DynamicForms (OrganizationId);
END
GO

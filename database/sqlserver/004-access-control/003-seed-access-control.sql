DECLARE @OrganizationId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000101';
DECLARE @SuperAdminUserId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000201';

DECLARE @Permissions TABLE
(
    [Name] NVARCHAR(160) NOT NULL,
    [DisplayName] NVARCHAR(200) NOT NULL,
    [Group] NVARCHAR(100) NOT NULL
);

INSERT INTO @Permissions ([Name], [DisplayName], [Group])
VALUES
(N'platform.dashboard.view', N'View dashboard', N'Platform'),
(N'Organizations.View', N'View organizations', N'Platform'),
(N'Organizations.Create', N'Create organizations', N'Platform'),
(N'Organizations.Update', N'Update organizations', N'Platform'),
(N'Organizations.Delete', N'Delete organizations', N'Platform'),
(N'Organizations.ManageApps', N'Manage organization apps and products', N'Platform'),
(N'Organizations.ManageModules', N'Manage organization modules', N'Platform'),
(N'Organizations.ManageDomains', N'Manage organization domains', N'Platform'),
(N'Organizations.ManageSettings', N'Manage organization settings', N'Platform'),
(N'platform.modules.view', N'View modules', N'Platform'),
(N'Users.View', N'View users', N'Identity'),
(N'Users.Create', N'Create users', N'Identity'),
(N'Users.Update', N'Update users', N'Identity'),
(N'Users.Delete', N'Delete users', N'Identity'),
(N'Users.ManageRoles', N'Manage user roles', N'Identity'),
(N'Users.ResetPassword', N'Reset user passwords', N'Identity'),
(N'Users.ManagePermissions', N'Manage user permissions', N'Identity'),
(N'Roles.View', N'View roles', N'Identity'),
(N'Roles.Create', N'Create roles', N'Identity'),
(N'Roles.Update', N'Update roles', N'Identity'),
(N'Roles.Delete', N'Delete roles', N'Identity'),
(N'Roles.Assign', N'Assign roles', N'Identity'),
(N'Roles.ManagePermissions', N'Manage role permissions', N'Identity'),
(N'Permissions.View', N'View permissions', N'Identity'),
(N'Permissions.Create', N'Create permissions', N'Identity'),
(N'Permissions.Update', N'Update permissions', N'Identity'),
(N'Permissions.Delete', N'Delete permissions', N'Identity'),
(N'platform.workflow.view', N'View workflow', N'Workflow'),
(N'Reports.View', N'View reports', N'Reports'),
(N'platform.auditlogs.view', N'View audit logs', N'Audit'),
(N'Settings.View', N'View settings', N'Configuration'),
(N'Developer.UIComponents.View', N'View UI component catalogue', N'Developer Tools');

MERGE [access].[Permissions] AS target
USING @Permissions AS source
ON target.[Name] = source.[Name]
WHEN MATCHED THEN
    UPDATE SET
        [DisplayName] = source.[DisplayName],
        [Group] = source.[Group],
        [Description] = source.[DisplayName],
        [IsSystem] = 1,
        [IsDeleted] = 0
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Id], [Name], [DisplayName], [Group], [Description], [IsSystem])
    VALUES (NEWID(), source.[Name], source.[DisplayName], source.[Group], source.[DisplayName], 1);

DECLARE @Roles TABLE
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(120) NOT NULL,
    [DisplayName] NVARCHAR(160) NOT NULL,
    [Description] NVARCHAR(500) NOT NULL
);

INSERT INTO @Roles ([Id], [Name], [DisplayName], [Description])
VALUES
('20000000-0000-0000-0000-000000000001', N'SuperAdmin', N'Super Admin', N'Full platform administration.'),
('20000000-0000-0000-0000-000000000002', N'Developer', N'Developer', N'Developer and UI kit access.'),
('20000000-0000-0000-0000-000000000003', N'Admin', N'Admin', N'Organization administration.'),
('20000000-0000-0000-0000-000000000004', N'Manager', N'Manager', N'Operational management access.'),
('20000000-0000-0000-0000-000000000005', N'User', N'User', N'Standard application user.');

MERGE [access].[Roles] AS target
USING @Roles AS source
ON target.[OrganizationId] = @OrganizationId AND target.[Name] = source.[Name]
WHEN MATCHED THEN
    UPDATE SET
        [DisplayName] = source.[DisplayName],
        [Description] = source.[Description],
        [Status] = N'Active',
        [IsSystem] = 1,
        [IsDeleted] = 0
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Id], [OrganizationId], [Name], [DisplayName], [Description], [Status], [IsSystem])
    VALUES (source.[Id], @OrganizationId, source.[Name], source.[DisplayName], source.[Description], N'Active', 1);

INSERT INTO [access].[RolePermissions] ([Id], [OrganizationId], [RoleId], [PermissionId])
SELECT NEWID(), @OrganizationId, r.[Id], p.[Id]
FROM [access].[Roles] r
CROSS JOIN [access].[Permissions] p
WHERE r.[OrganizationId] = @OrganizationId
  AND r.[Name] IN (N'SuperAdmin', N'Developer')
  AND p.[IsDeleted] = 0
  AND NOT EXISTS (
      SELECT 1
      FROM [access].[RolePermissions] rp
      WHERE rp.[OrganizationId] = @OrganizationId
        AND rp.[RoleId] = r.[Id]
        AND rp.[PermissionId] = p.[Id]
  );

INSERT INTO [access].[RolePermissions] ([Id], [OrganizationId], [RoleId], [PermissionId])
SELECT NEWID(), @OrganizationId, r.[Id], p.[Id]
FROM [access].[Roles] r
INNER JOIN [access].[Permissions] p
    ON p.[Group] IN (N'Platform', N'Identity', N'Configuration')
WHERE r.[OrganizationId] = @OrganizationId
  AND r.[Name] = N'Admin'
  AND p.[IsDeleted] = 0
  AND NOT EXISTS (
      SELECT 1
      FROM [access].[RolePermissions] rp
      WHERE rp.[OrganizationId] = @OrganizationId
        AND rp.[RoleId] = r.[Id]
        AND rp.[PermissionId] = p.[Id]
  );

INSERT INTO [access].[UserRoles] ([Id], [OrganizationId], [UserId], [RoleId])
SELECT NEWID(), @OrganizationId, @SuperAdminUserId, r.[Id]
FROM [access].[Roles] r
WHERE r.[OrganizationId] = @OrganizationId
  AND r.[Name] = N'SuperAdmin'
  AND NOT EXISTS (
      SELECT 1
      FROM [access].[UserRoles] ur
      WHERE ur.[OrganizationId] = @OrganizationId
        AND ur.[UserId] = @SuperAdminUserId
        AND ur.[RoleId] = r.[Id]
  );

CREATE OR ALTER PROCEDURE [access].[sp_Role_List]
    @OrganizationId UNIQUEIDENTIFIER,
    @Search NVARCHAR(200) = NULL,
    @Status NVARCHAR(40) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(40) = N'Name',
    @SortDirection NVARCHAR(4) = N'asc'
AS
BEGIN
    SET NOCOUNT ON;

    WITH Filtered AS
    (
        SELECT
            r.[Id],
            r.[OrganizationId],
            r.[Name],
            r.[DisplayName],
            r.[Description],
            r.[Status],
            r.[IsSystem],
            r.[CreatedDateUtc],
            COUNT(DISTINCT ur.[UserId]) AS [UserCount],
            COUNT(DISTINCT rp.[PermissionId]) AS [PermissionCount]
        FROM [access].[Roles] r
        LEFT JOIN [access].[UserRoles] ur ON ur.[OrganizationId] = r.[OrganizationId] AND ur.[RoleId] = r.[Id]
        LEFT JOIN [access].[RolePermissions] rp ON rp.[OrganizationId] = r.[OrganizationId] AND rp.[RoleId] = r.[Id]
        WHERE r.[OrganizationId] = @OrganizationId
          AND r.[IsDeleted] = 0
          AND (@Status IS NULL OR r.[Status] = @Status)
          AND (
                @Search IS NULL
                OR r.[Name] LIKE N'%' + @Search + N'%'
                OR r.[DisplayName] LIKE N'%' + @Search + N'%'
                OR r.[Description] LIKE N'%' + @Search + N'%'
              )
        GROUP BY r.[Id], r.[OrganizationId], r.[Name], r.[DisplayName], r.[Description], r.[Status], r.[IsSystem], r.[CreatedDateUtc]
    )
    SELECT
        *,
        COUNT(1) OVER() AS [TotalCount]
    FROM Filtered
    ORDER BY
        CASE WHEN @SortColumn = N'Name' AND @SortDirection = N'asc' THEN [Name] END ASC,
        CASE WHEN @SortColumn = N'Name' AND @SortDirection = N'desc' THEN [Name] END DESC,
        CASE WHEN @SortColumn = N'DisplayName' AND @SortDirection = N'asc' THEN [DisplayName] END ASC,
        CASE WHEN @SortColumn = N'DisplayName' AND @SortDirection = N'desc' THEN [DisplayName] END DESC,
        CASE WHEN @SortColumn = N'Status' AND @SortDirection = N'asc' THEN [Status] END ASC,
        CASE WHEN @SortColumn = N'Status' AND @SortDirection = N'desc' THEN [Status] END DESC,
        CASE WHEN @SortColumn = N'PermissionCount' AND @SortDirection = N'asc' THEN [PermissionCount] END ASC,
        CASE WHEN @SortColumn = N'PermissionCount' AND @SortDirection = N'desc' THEN [PermissionCount] END DESC,
        [Name] ASC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

CREATE OR ALTER PROCEDURE [access].[sp_Role_GetById]
    @OrganizationId UNIQUEIDENTIFIER,
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        [Id],
        [OrganizationId],
        [Name],
        [DisplayName],
        ISNULL([Description], N'') AS [Description],
        [Status],
        [IsSystem]
    FROM [access].[Roles]
    WHERE [OrganizationId] = @OrganizationId
      AND [Id] = @Id
      AND [IsDeleted] = 0;
END
GO

CREATE OR ALTER PROCEDURE [access].[sp_Role_Create]
    @OrganizationId UNIQUEIDENTIFIER,
    @Name NVARCHAR(120),
    @DisplayName NVARCHAR(160),
    @Description NVARCHAR(500) = NULL,
    @Status NVARCHAR(40) = N'Active',
    @CreatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();

    INSERT INTO [access].[Roles] ([Id], [OrganizationId], [Name], [DisplayName], [Description], [Status], [IsSystem], [CreatedBy])
    VALUES (@Id, @OrganizationId, @Name, @DisplayName, @Description, @Status, 0, @CreatedBy);

    SELECT @Id AS [Id];
END
GO

CREATE OR ALTER PROCEDURE [access].[sp_Role_Update]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @DisplayName NVARCHAR(160),
    @Description NVARCHAR(500) = NULL,
    @Status NVARCHAR(40) = N'Active',
    @UpdatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [access].[Roles]
    SET [DisplayName] = @DisplayName,
        [Description] = @Description,
        [Status] = @Status,
        [UpdatedDateUtc] = SYSUTCDATETIME(),
        [UpdatedBy] = @UpdatedBy
    WHERE [Id] = @Id
      AND [OrganizationId] = @OrganizationId
      AND [IsDeleted] = 0
      AND [IsSystem] = 0;
END
GO

CREATE OR ALTER PROCEDURE [access].[sp_Role_SoftDelete]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @DeletedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [access].[Roles]
    SET [IsDeleted] = 1,
        [DeletedAtUtc] = SYSUTCDATETIME(),
        [DeletedBy] = @DeletedBy
    WHERE [Id] = @Id
      AND [OrganizationId] = @OrganizationId
      AND [IsSystem] = 0;
END
GO

CREATE OR ALTER PROCEDURE [access].[sp_Permission_List]
    @Search NVARCHAR(200) = NULL,
    @Group NVARCHAR(100) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(40) = N'Name',
    @SortDirection NVARCHAR(4) = N'asc'
AS
BEGIN
    SET NOCOUNT ON;

    WITH Filtered AS
    (
        SELECT [Id], [Name], [DisplayName], [Group], [Description], [IsSystem], [CreatedDateUtc]
        FROM [access].[Permissions]
        WHERE [IsDeleted] = 0
          AND (@Group IS NULL OR [Group] = @Group)
          AND (
                @Search IS NULL
                OR [Name] LIKE N'%' + @Search + N'%'
                OR [DisplayName] LIKE N'%' + @Search + N'%'
                OR [Group] LIKE N'%' + @Search + N'%'
              )
    )
    SELECT
        *,
        COUNT(1) OVER() AS [TotalCount]
    FROM Filtered
    ORDER BY
        CASE WHEN @SortColumn = N'Name' AND @SortDirection = N'asc' THEN [Name] END ASC,
        CASE WHEN @SortColumn = N'Name' AND @SortDirection = N'desc' THEN [Name] END DESC,
        CASE WHEN @SortColumn = N'DisplayName' AND @SortDirection = N'asc' THEN [DisplayName] END ASC,
        CASE WHEN @SortColumn = N'DisplayName' AND @SortDirection = N'desc' THEN [DisplayName] END DESC,
        CASE WHEN @SortColumn = N'Group' AND @SortDirection = N'asc' THEN [Group] END ASC,
        CASE WHEN @SortColumn = N'Group' AND @SortDirection = N'desc' THEN [Group] END DESC,
        [Name] ASC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

CREATE OR ALTER PROCEDURE [access].[sp_Permission_GetById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        [Id],
        [Name],
        [DisplayName],
        [Group],
        ISNULL([Description], N'') AS [Description],
        [IsSystem]
    FROM [access].[Permissions]
    WHERE [Id] = @Id
      AND [IsDeleted] = 0;
END
GO

CREATE OR ALTER PROCEDURE [access].[sp_Permission_Create]
    @Name NVARCHAR(160),
    @DisplayName NVARCHAR(200),
    @Group NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @IsSystem BIT = 0,
    @CreatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();

    INSERT INTO [access].[Permissions] ([Id], [Name], [DisplayName], [Group], [Description], [IsSystem], [CreatedBy])
    VALUES (@Id, @Name, @DisplayName, @Group, @Description, @IsSystem, @CreatedBy);

    SELECT @Id AS [Id];
END
GO

CREATE OR ALTER PROCEDURE [access].[sp_Permission_Update]
    @Id UNIQUEIDENTIFIER,
    @DisplayName NVARCHAR(200),
    @Group NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @UpdatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [access].[Permissions]
    SET [DisplayName] = @DisplayName,
        [Group] = @Group,
        [Description] = @Description,
        [UpdatedDateUtc] = SYSUTCDATETIME(),
        [UpdatedBy] = @UpdatedBy
    WHERE [Id] = @Id
      AND [IsDeleted] = 0
      AND [IsSystem] = 0;
END
GO

CREATE OR ALTER PROCEDURE [access].[sp_Permission_Delete]
    @Id UNIQUEIDENTIFIER,
    @DeletedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [access].[Permissions]
    SET [IsDeleted] = 1,
        [DeletedAtUtc] = SYSUTCDATETIME(),
        [DeletedBy] = @DeletedBy
    WHERE [Id] = @Id
      AND [IsSystem] = 0;
END
GO

CREATE OR ALTER PROCEDURE [access].[sp_RolePermission_GetByRoleId]
    @OrganizationId UNIQUEIDENTIFIER,
    @RoleId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.[Name] AS [PermissionName],
        p.[DisplayName],
        p.[Group] AS [ModuleName],
        CAST(CASE WHEN rp.[Id] IS NULL THEN 0 ELSE 1 END AS BIT) AS [IsGranted]
    FROM [access].[Permissions] p
    LEFT JOIN [access].[RolePermissions] rp
        ON rp.[PermissionId] = p.[Id]
       AND rp.[OrganizationId] = @OrganizationId
       AND rp.[RoleId] = @RoleId
    WHERE p.[IsDeleted] = 0
    ORDER BY p.[Group], p.[Name];
END
GO

CREATE OR ALTER PROCEDURE [access].[sp_RolePermission_Save]
    @OrganizationId UNIQUEIDENTIFIER,
    @RoleId UNIQUEIDENTIFIER,
    @PermissionName NVARCHAR(160),
    @IsGranted BIT,
    @ChangedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @PermissionId UNIQUEIDENTIFIER;
    SELECT @PermissionId = [Id] FROM [access].[Permissions] WHERE [Name] = @PermissionName AND [IsDeleted] = 0;

    IF @PermissionId IS NULL
        RETURN;

    IF @IsGranted = 1
    BEGIN
        IF NOT EXISTS (
            SELECT 1 FROM [access].[RolePermissions]
            WHERE [OrganizationId] = @OrganizationId AND [RoleId] = @RoleId AND [PermissionId] = @PermissionId
        )
        BEGIN
            INSERT INTO [access].[RolePermissions] ([Id], [OrganizationId], [RoleId], [PermissionId], [CreatedBy])
            VALUES (NEWID(), @OrganizationId, @RoleId, @PermissionId, @ChangedBy);
        END
    END
    ELSE
    BEGIN
        DELETE FROM [access].[RolePermissions]
        WHERE [OrganizationId] = @OrganizationId
          AND [RoleId] = @RoleId
          AND [PermissionId] = @PermissionId;
    END
END
GO

CREATE OR ALTER PROCEDURE [access].[sp_UserRole_GetByUserId]
    @OrganizationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.[Id] AS [RoleId],
        r.[Name],
        r.[DisplayName],
        r.[Description],
        r.[IsSystem],
        CAST(CASE WHEN ur.[Id] IS NULL THEN 0 ELSE 1 END AS BIT) AS [IsAssigned]
    FROM [access].[Roles] r
    LEFT JOIN [access].[UserRoles] ur
        ON ur.[OrganizationId] = r.[OrganizationId]
       AND ur.[RoleId] = r.[Id]
       AND ur.[UserId] = @UserId
    WHERE r.[OrganizationId] = @OrganizationId
      AND r.[IsDeleted] = 0
      AND r.[Status] = N'Active'
    ORDER BY r.[IsSystem] DESC, r.[DisplayName] ASC;
END
GO

CREATE OR ALTER PROCEDURE [access].[sp_UserRole_Save]
    @OrganizationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @RoleId UNIQUEIDENTIFIER,
    @IsAssigned BIT,
    @ChangedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @IsAssigned = 1
    BEGIN
        IF NOT EXISTS (
            SELECT 1 FROM [access].[UserRoles]
            WHERE [OrganizationId] = @OrganizationId AND [UserId] = @UserId AND [RoleId] = @RoleId
        )
        BEGIN
            INSERT INTO [access].[UserRoles] ([Id], [OrganizationId], [UserId], [RoleId], [CreatedBy])
            VALUES (NEWID(), @OrganizationId, @UserId, @RoleId, @ChangedBy);
        END
    END
    ELSE
    BEGIN
        DELETE FROM [access].[UserRoles]
        WHERE [OrganizationId] = @OrganizationId AND [UserId] = @UserId AND [RoleId] = @RoleId;
    END
END
GO

CREATE OR ALTER PROCEDURE [access].[sp_User_EffectivePermissions]
    @OrganizationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    WITH RoleGrant AS
    (
        SELECT DISTINCT p.[Name] AS [PermissionName]
        FROM [access].[UserRoles] ur
        INNER JOIN [access].[RolePermissions] rp
            ON rp.[OrganizationId] = ur.[OrganizationId]
           AND rp.[RoleId] = ur.[RoleId]
        INNER JOIN [access].[Permissions] p
            ON p.[Id] = rp.[PermissionId]
        WHERE ur.[OrganizationId] = @OrganizationId
          AND ur.[UserId] = @UserId
          AND p.[IsDeleted] = 0
    ),
    DirectDeny AS
    (
        SELECT p.[Name] AS [PermissionName]
        FROM [access].[UserPermissions] up
        INNER JOIN [access].[Permissions] p ON p.[Id] = up.[PermissionId]
        WHERE up.[OrganizationId] = @OrganizationId
          AND up.[UserId] = @UserId
          AND up.[IsGranted] = 0
          AND p.[IsDeleted] = 0
    ),
    DirectAllow AS
    (
        SELECT p.[Name] AS [PermissionName]
        FROM [access].[UserPermissions] up
        INNER JOIN [access].[Permissions] p ON p.[Id] = up.[PermissionId]
        WHERE up.[OrganizationId] = @OrganizationId
          AND up.[UserId] = @UserId
          AND up.[IsGranted] = 1
          AND p.[IsDeleted] = 0
    )
    SELECT [PermissionName]
    FROM RoleGrant
    WHERE [PermissionName] NOT IN (SELECT [PermissionName] FROM DirectDeny)
    UNION
    SELECT [PermissionName]
    FROM DirectAllow;
END
GO

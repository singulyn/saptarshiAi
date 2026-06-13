USE SaptariX_Platform;
GO

CREATE OR ALTER PROCEDURE [identity].[sp_User_List]
    @OrganizationId UNIQUEIDENTIFIER,
    @Search NVARCHAR(200) = NULL,
    @Status NVARCHAR(40) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(40) = N'CreatedDate',
    @SortDirection NVARCHAR(4) = N'desc'
AS
BEGIN
    SET NOCOUNT ON;

    SET @PageNumber = CASE WHEN @PageNumber < 1 THEN 1 ELSE @PageNumber END;
    SET @PageSize = CASE WHEN @PageSize < 5 THEN 5 WHEN @PageSize > 100 THEN 100 ELSE @PageSize END;

    ;WITH UsersForOrganization AS
    (
        SELECT
            u.Id,
            COALESCE(u.OrganizationId, ou.OrganizationId) AS OrganizationId,
            COALESCE(u.FirstName, u.DisplayName, N'') AS FirstName,
            COALESCE(u.LastName, N'') AS LastName,
            COALESCE(NULLIF(LTRIM(RTRIM(CONCAT(u.FirstName, N' ', u.LastName))), N''), u.DisplayName, u.Email) AS FullName,
            u.Email,
            u.MobileNumber,
            COALESCE(u.RoleName, ou.RoleName, N'Member') AS [Role],
            COALESCE(u.Status, CASE WHEN u.IsActive = 1 THEN N'Active' ELSE N'Inactive' END) AS [Status],
            u.CreatedAtUtc AS CreatedDateUtc,
            u.LastLoginAtUtc
        FROM id.Users AS u
        LEFT JOIN org.OrganizationUsers AS ou
            ON ou.UserId = u.Id
           AND ou.OrganizationId = @OrganizationId
        WHERE ISNULL(u.IsDeleted, 0) = 0
          AND (u.OrganizationId = @OrganizationId OR ou.OrganizationId = @OrganizationId)
    ),
    FilteredUsers AS
    (
        SELECT *
        FROM UsersForOrganization
        WHERE (@Search IS NULL
               OR FullName LIKE N'%' + @Search + N'%'
               OR Email LIKE N'%' + @Search + N'%'
               OR MobileNumber LIKE N'%' + @Search + N'%')
          AND (@Status IS NULL OR [Status] = @Status)
    )
    SELECT
        Id,
        OrganizationId,
        FirstName,
        LastName,
        FullName,
        Email,
        MobileNumber,
        [Role],
        [Status],
        CreatedDateUtc,
        LastLoginAtUtc,
        COUNT(1) OVER() AS TotalCount
    FROM FilteredUsers
    ORDER BY
        CASE WHEN @SortColumn = N'FullName' AND @SortDirection = N'asc' THEN FullName END ASC,
        CASE WHEN @SortColumn = N'FullName' AND @SortDirection <> N'asc' THEN FullName END DESC,
        CASE WHEN @SortColumn = N'Email' AND @SortDirection = N'asc' THEN Email END ASC,
        CASE WHEN @SortColumn = N'Email' AND @SortDirection <> N'asc' THEN Email END DESC,
        CASE WHEN @SortColumn = N'Role' AND @SortDirection = N'asc' THEN [Role] END ASC,
        CASE WHEN @SortColumn = N'Role' AND @SortDirection <> N'asc' THEN [Role] END DESC,
        CASE WHEN @SortColumn = N'Status' AND @SortDirection = N'asc' THEN [Status] END ASC,
        CASE WHEN @SortColumn = N'Status' AND @SortDirection <> N'asc' THEN [Status] END DESC,
        CASE WHEN @SortColumn = N'LastLogin' AND @SortDirection = N'asc' THEN LastLoginAtUtc END ASC,
        CASE WHEN @SortColumn = N'LastLogin' AND @SortDirection <> N'asc' THEN LastLoginAtUtc END DESC,
        CASE WHEN @SortColumn = N'CreatedDate' AND @SortDirection = N'asc' THEN CreatedDateUtc END ASC,
        CASE WHEN @SortColumn = N'CreatedDate' AND @SortDirection <> N'asc' THEN CreatedDateUtc END DESC,
        Id ASC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

CREATE OR ALTER PROCEDURE [identity].[sp_User_GetById]
    @OrganizationId UNIQUEIDENTIFIER,
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        u.Id,
        COALESCE(u.OrganizationId, ou.OrganizationId) AS OrganizationId,
        COALESCE(u.FirstName, u.DisplayName, N'') AS FirstName,
        COALESCE(u.LastName, N'') AS LastName,
        u.Email,
        u.MobileNumber,
        COALESCE(u.RoleName, ou.RoleName, N'Member') AS [Role],
        COALESCE(u.Status, CASE WHEN u.IsActive = 1 THEN N'Active' ELSE N'Inactive' END) AS [Status],
        u.CreatedAtUtc AS CreatedDateUtc,
        u.LastLoginAtUtc
    FROM id.Users AS u
    LEFT JOIN org.OrganizationUsers AS ou
        ON ou.UserId = u.Id
       AND ou.OrganizationId = @OrganizationId
    WHERE u.Id = @Id
      AND ISNULL(u.IsDeleted, 0) = 0
      AND (u.OrganizationId = @OrganizationId OR ou.OrganizationId = @OrganizationId);
END
GO

CREATE OR ALTER PROCEDURE [identity].[sp_User_Create]
    @OrganizationId UNIQUEIDENTIFIER,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(320),
    @MobileNumber NVARCHAR(30) = NULL,
    @Role NVARCHAR(100),
    @Status NVARCHAR(40),
    @PasswordHash NVARCHAR(500),
    @CreatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();
    DECLARE @DisplayName NVARCHAR(200) = LTRIM(RTRIM(CONCAT(@FirstName, N' ', @LastName)));

    INSERT INTO id.Users
    (
        Id,
        OrganizationId,
        Email,
        FirstName,
        LastName,
        DisplayName,
        MobileNumber,
        RoleName,
        Status,
        PasswordHash,
        IsActive,
        IsDeleted,
        CreatedAtUtc
    )
    VALUES
    (
        @Id,
        @OrganizationId,
        @Email,
        @FirstName,
        @LastName,
        @DisplayName,
        @MobileNumber,
        @Role,
        @Status,
        @PasswordHash,
        CASE WHEN @Status = N'Active' THEN 1 ELSE 0 END,
        0,
        SYSUTCDATETIME()
    );

    IF OBJECT_ID('org.OrganizationUsers', 'U') IS NOT NULL
       AND EXISTS (SELECT 1 FROM org.Organizations WHERE Id = @OrganizationId)
    BEGIN
        MERGE org.OrganizationUsers AS target
        USING (SELECT @OrganizationId AS OrganizationId, @Id AS UserId, @Role AS RoleName) AS source
        ON target.OrganizationId = source.OrganizationId AND target.UserId = source.UserId
        WHEN MATCHED THEN
            UPDATE SET RoleName = source.RoleName
        WHEN NOT MATCHED THEN
            INSERT (Id, OrganizationId, UserId, RoleName)
            VALUES (NEWID(), source.OrganizationId, source.UserId, source.RoleName);
    END

    SELECT @Id AS Id;
END
GO

CREATE OR ALTER PROCEDURE [identity].[sp_User_Update]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(320),
    @MobileNumber NVARCHAR(30) = NULL,
    @Role NVARCHAR(100),
    @Status NVARCHAR(40),
    @UpdatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE id.Users
    SET
        FirstName = @FirstName,
        LastName = @LastName,
        DisplayName = LTRIM(RTRIM(CONCAT(@FirstName, N' ', @LastName))),
        Email = @Email,
        MobileNumber = @MobileNumber,
        RoleName = @Role,
        Status = @Status,
        IsActive = CASE WHEN @Status = N'Active' THEN 1 ELSE 0 END,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE Id = @Id
      AND ISNULL(IsDeleted, 0) = 0
      AND OrganizationId = @OrganizationId;

    IF OBJECT_ID('org.OrganizationUsers', 'U') IS NOT NULL
       AND EXISTS (SELECT 1 FROM org.Organizations WHERE Id = @OrganizationId)
    BEGIN
        MERGE org.OrganizationUsers AS target
        USING (SELECT @OrganizationId AS OrganizationId, @Id AS UserId, @Role AS RoleName) AS source
        ON target.OrganizationId = source.OrganizationId AND target.UserId = source.UserId
        WHEN MATCHED THEN
            UPDATE SET RoleName = source.RoleName
        WHEN NOT MATCHED THEN
            INSERT (Id, OrganizationId, UserId, RoleName)
            VALUES (NEWID(), source.OrganizationId, source.UserId, source.RoleName);
    END
END
GO

CREATE OR ALTER PROCEDURE [identity].[sp_User_SoftDelete]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @DeletedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE id.Users
    SET
        IsDeleted = 1,
        DeletedAtUtc = SYSUTCDATETIME(),
        DeletedBy = @DeletedBy,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE Id = @Id
      AND OrganizationId = @OrganizationId
      AND ISNULL(IsDeleted, 0) = 0;
END
GO

CREATE OR ALTER PROCEDURE [identity].[sp_UserPermission_GetByUserId]
    @OrganizationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH PermissionCatalog AS
    (
        SELECT Name AS PermissionName, DisplayName, [Group] AS ModuleName
        FROM id.Permissions
        UNION
        SELECT PermissionName, PermissionName AS DisplayName,
            CASE WHEN CHARINDEX(N'.', PermissionName) > 1 THEN LEFT(PermissionName, CHARINDEX(N'.', PermissionName) - 1) ELSE N'General' END AS ModuleName
        FROM id.UserPermissions
        WHERE OrganizationId = @OrganizationId
          AND UserId = @UserId
    ),
    RoleGrants AS
    (
        SELECT DISTINCT p.Name AS PermissionName
        FROM id.UserRoles AS ur
        INNER JOIN id.RolePermissions AS rp ON rp.RoleId = ur.RoleId
        INNER JOIN id.Permissions AS p ON p.Id = rp.PermissionId
        WHERE ur.UserId = @UserId
          AND ur.OrganizationId = @OrganizationId
    ),
    DirectGrants AS
    (
        SELECT PermissionName, IsGranted
        FROM id.UserPermissions
        WHERE UserId = @UserId
          AND OrganizationId = @OrganizationId
    )
    SELECT
        pc.PermissionName,
        pc.DisplayName,
        pc.ModuleName,
        CONVERT(BIT, CASE WHEN rg.PermissionName IS NULL THEN 0 ELSE 1 END) AS IsRoleGranted,
        dg.IsGranted AS DirectGrant
    FROM PermissionCatalog AS pc
    LEFT JOIN RoleGrants AS rg ON rg.PermissionName = pc.PermissionName
    LEFT JOIN DirectGrants AS dg ON dg.PermissionName = pc.PermissionName
    ORDER BY pc.ModuleName, pc.PermissionName;
END
GO

CREATE OR ALTER PROCEDURE [identity].[sp_UserPermission_Save]
    @OrganizationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @PermissionName NVARCHAR(200),
    @IsGranted BIT,
    @ChangedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    MERGE id.UserPermissions WITH (HOLDLOCK) AS target
    USING
    (
        SELECT
            @OrganizationId AS OrganizationId,
            @UserId AS UserId,
            @PermissionName AS PermissionName,
            @IsGranted AS IsGranted
    ) AS source
    ON target.OrganizationId = source.OrganizationId
       AND target.UserId = source.UserId
       AND target.PermissionName = source.PermissionName
    WHEN MATCHED THEN
        UPDATE SET
            IsGranted = source.IsGranted,
            UpdatedAtUtc = SYSUTCDATETIME(),
            UpdatedBy = @ChangedBy
    WHEN NOT MATCHED THEN
        INSERT (UserId, OrganizationId, PermissionName, IsGranted, CreatedAtUtc, UpdatedAtUtc, UpdatedBy)
        VALUES (source.UserId, source.OrganizationId, source.PermissionName, source.IsGranted, SYSUTCDATETIME(), SYSUTCDATETIME(), @ChangedBy);
END
GO

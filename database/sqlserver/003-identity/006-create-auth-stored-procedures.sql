USE SaptariX_Platform;
GO

CREATE OR ALTER PROCEDURE id.GetUserByEmail
    @Email NVARCHAR(320)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Email, DisplayName, PasswordHash, IsActive
    FROM id.Users
    WHERE Email = @Email;
END
GO

CREATE OR ALTER PROCEDURE id.GetUserPermissions
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT p.Name, p.DisplayName, p.[Group]
    FROM id.UserRoles AS ur
    INNER JOIN id.RolePermissions AS rp ON rp.RoleId = ur.RoleId
    INNER JOIN id.Permissions AS p ON p.Id = rp.PermissionId
    WHERE ur.UserId = @UserId
      AND ur.OrganizationId = @OrganizationId;
END
GO

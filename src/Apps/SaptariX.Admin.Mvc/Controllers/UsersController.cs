using Microsoft.AspNetCore.Mvc;
using SaptariX.Admin.Mvc.ViewModels.Users;
using SaptariX.Platform.AccessControl;
using SaptariX.Platform.AccessControl.Permissions;
using SaptariX.Platform.AccessControl.UserRoles;
using SaptariX.Platform.Identity.Users;
using SaptariX.Plugin.Abstractions.Identity;
using SaptariX.Plugin.Abstractions.Organization;

namespace SaptariX.Admin.Mvc.Controllers;

public sealed class UsersController : Controller
{
    private static readonly Guid DemoOrganizationId = Guid.Parse("00000000-0000-0000-0000-000000000101");
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000201");

    private readonly IUserService _userService;
    private readonly IUserPermissionService _userPermissionService;
    private readonly IUserRoleService _userRoleService;
    private readonly ICurrentOrganizationService _currentOrganizationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionAuthorizationService _permissionAuthorizationService;

    public UsersController(
        IUserService userService,
        IUserPermissionService userPermissionService,
        IUserRoleService userRoleService,
        ICurrentOrganizationService currentOrganizationService,
        ICurrentUserService currentUserService,
        IPermissionAuthorizationService permissionAuthorizationService)
    {
        _userService = userService;
        _userPermissionService = userPermissionService;
        _userRoleService = userRoleService;
        _currentOrganizationService = currentOrganizationService;
        _currentUserService = currentUserService;
        _permissionAuthorizationService = permissionAuthorizationService;
    }

    [HttpGet("/Users")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.UsersView, cancellationToken))
        {
            return Forbid();
        }

        return View(UserListViewModel.Empty(GetOrganizationId()));
    }

    [HttpGet("/Users/List")]
    public async Task<IActionResult> List([FromQuery] UserListFilterRequest request, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.UsersView, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        request.OrganizationId = GetOrganizationId();
        var result = await _userService.ListAsync(request, cancellationToken);
        return PartialView("_UserList", UserListViewModel.FromDto(result, request));
    }

    [HttpGet("/Users/GetById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.UsersUpdate, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var user = await _userService.GetByIdAsync(GetOrganizationId(), id, cancellationToken);
        if (user is null)
        {
            return NotFound(new { succeeded = false, message = "User was not found." });
        }

        return Json(UserFormViewModel.FromDto(user));
    }

    [HttpPost("/Users/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] UserCreateRequest request, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.UsersCreate, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        request.OrganizationId = GetOrganizationId();
        request.CreatedBy = GetUserId();
        var result = await _userService.CreateAsync(request, cancellationToken);
        return ToJsonResult(result);
    }

    [HttpPost("/Users/Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update([FromForm] UserUpdateRequest request, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.UsersUpdate, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        request.OrganizationId = GetOrganizationId();
        request.UpdatedBy = GetUserId();
        var result = await _userService.UpdateAsync(request, cancellationToken);
        return ToJsonResult(result);
    }

    [HttpPost("/Users/SoftDelete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(Guid id, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.UsersDelete, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var result = await _userService.SoftDeleteAsync(
            new UserSoftDeleteRequest
            {
                Id = id,
                OrganizationId = GetOrganizationId(),
                DeletedBy = GetUserId()
            },
            cancellationToken);

        return ToJsonResult(result);
    }

    [HttpGet("/Users/GetPermissions/{userId:guid}")]
    public async Task<IActionResult> GetPermissions(Guid userId, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.UsersManagePermissions, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var permissions = await _userPermissionService.GetPermissionsAsync(
            new UserPermissionRequest
            {
                UserId = userId,
                OrganizationId = GetOrganizationId()
            },
            cancellationToken);

        return PartialView("_UserPermissionsDrawer", UserPermissionViewModel.FromDto(permissions));
    }

    [HttpPost("/Users/SavePermissions")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePermissions([FromBody] UserPermissionSaveRequest request, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.UsersManagePermissions, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        request.OrganizationId = GetOrganizationId();
        request.ChangedBy = GetUserId();
        var result = await _userPermissionService.SavePermissionsAsync(request, cancellationToken);
        return ToJsonResult(result);
    }

    [HttpGet("/Users/GetRoles/{userId:guid}")]
    public async Task<IActionResult> GetRoles(Guid userId, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.UsersManageRoles, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var roles = await _userRoleService.GetRolesAsync(
            new UserRoleRequest
            {
                UserId = userId,
                OrganizationId = GetOrganizationId()
            },
            cancellationToken);

        return PartialView("_UserRolesDrawer", UserRoleViewModel.FromDto(roles));
    }

    [HttpPost("/Users/SaveRoles")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveRoles([FromBody] UserRoleSaveRequest request, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.UsersManageRoles, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        request.OrganizationId = GetOrganizationId();
        request.ChangedBy = GetUserId();
        var result = await _userRoleService.SaveRolesAsync(request, cancellationToken);
        Response.StatusCode = result.Succeeded ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;
        return Json(new { result.Succeeded, result.Message, result.UserId });
    }

    private JsonResult ToJsonResult(UserCommandResult result)
    {
        Response.StatusCode = result.Succeeded ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;
        return Json(new { result.Succeeded, result.Message, result.UserId });
    }

    private Task<bool> HasPermissionAsync(string permission, CancellationToken cancellationToken)
    {
        return _permissionAuthorizationService.HasPermissionAsync(GetUserId(), GetOrganizationId(), permission, cancellationToken);
    }

    private Guid GetOrganizationId()
    {
        return _currentOrganizationService.OrganizationId.GetValueOrDefault(DemoOrganizationId);
    }

    private Guid GetUserId()
    {
        return _currentUserService.UserId.GetValueOrDefault(DemoUserId);
    }
}

using Microsoft.AspNetCore.Mvc;
using SaptariX.Admin.Mvc.ViewModels.Roles;
using SaptariX.Platform.AccessControl;
using SaptariX.Platform.AccessControl.Permissions;
using SaptariX.Platform.AccessControl.Roles;
using SaptariX.Plugin.Abstractions.Identity;
using SaptariX.Plugin.Abstractions.Organization;

namespace SaptariX.Admin.Mvc.Controllers;

public sealed class RolesController : Controller
{
    private static readonly Guid DemoOrganizationId = Guid.Parse("00000000-0000-0000-0000-000000000101");
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000201");

    private readonly IRoleService _roleService;
    private readonly ICurrentOrganizationService _currentOrganizationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionAuthorizationService _permissionAuthorizationService;

    public RolesController(
        IRoleService roleService,
        ICurrentOrganizationService currentOrganizationService,
        ICurrentUserService currentUserService,
        IPermissionAuthorizationService permissionAuthorizationService)
    {
        _roleService = roleService;
        _currentOrganizationService = currentOrganizationService;
        _currentUserService = currentUserService;
        _permissionAuthorizationService = permissionAuthorizationService;
    }

    [HttpGet("/Roles")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.RolesView, cancellationToken))
        {
            return Forbid();
        }

        return View(RoleListViewModel.Empty(GetOrganizationId()));
    }

    [HttpGet("/Roles/List")]
    public async Task<IActionResult> List([FromQuery] RoleListFilterRequest request, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.RolesView, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        request.OrganizationId = GetOrganizationId();
        var result = await _roleService.ListAsync(request, cancellationToken);
        return PartialView("_RoleList", RoleListViewModel.FromDto(result, request));
    }

    [HttpGet("/Roles/GetById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.RolesUpdate, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var role = await _roleService.GetByIdAsync(GetOrganizationId(), id, cancellationToken);
        if (role is null)
        {
            return NotFound(new { succeeded = false, message = "Role was not found." });
        }

        return Json(RoleFormViewModel.FromDto(role));
    }

    [HttpPost("/Roles/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] RoleCreateRequest request, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.RolesCreate, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        request.OrganizationId = GetOrganizationId();
        request.CreatedBy = GetUserId();
        var result = await _roleService.CreateAsync(request, cancellationToken);
        return ToJsonResult(result);
    }

    [HttpPost("/Roles/Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update([FromForm] RoleUpdateRequest request, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.RolesUpdate, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        request.OrganizationId = GetOrganizationId();
        request.UpdatedBy = GetUserId();
        var result = await _roleService.UpdateAsync(request, cancellationToken);
        return ToJsonResult(result);
    }

    [HttpPost("/Roles/SoftDelete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(Guid id, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.RolesDelete, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var result = await _roleService.SoftDeleteAsync(
            new RoleSoftDeleteRequest
            {
                Id = id,
                OrganizationId = GetOrganizationId(),
                DeletedBy = GetUserId()
            },
            cancellationToken);

        return ToJsonResult(result);
    }

    [HttpGet("/Roles/GetPermissions/{roleId:guid}")]
    public async Task<IActionResult> GetPermissions(Guid roleId, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.RolesManagePermissions, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var permissions = await _roleService.GetPermissionsAsync(
            new RolePermissionRequest
            {
                RoleId = roleId,
                OrganizationId = GetOrganizationId()
            },
            cancellationToken);

        return PartialView("_RolePermissionsDrawer", RolePermissionViewModel.FromDto(permissions));
    }

    [HttpPost("/Roles/SavePermissions")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePermissions([FromBody] RolePermissionSaveRequest request, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.RolesManagePermissions, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        request.OrganizationId = GetOrganizationId();
        request.ChangedBy = GetUserId();
        var result = await _roleService.SavePermissionsAsync(request, cancellationToken);
        return ToJsonResult(result);
    }

    private JsonResult ToJsonResult(RoleCommandResult result)
    {
        Response.StatusCode = result.Succeeded ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;
        return Json(new { result.Succeeded, result.Message, result.RoleId });
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

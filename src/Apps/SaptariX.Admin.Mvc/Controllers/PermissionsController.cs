using Microsoft.AspNetCore.Mvc;
using SaptariX.Admin.Mvc.ViewModels.Permissions;
using SaptariX.Platform.AccessControl;
using SaptariX.Platform.AccessControl.Permissions;
using SaptariX.Plugin.Abstractions.Identity;
using SaptariX.Plugin.Abstractions.Organization;

namespace SaptariX.Admin.Mvc.Controllers;

public sealed class PermissionsController : Controller
{
    private static readonly Guid DemoOrganizationId = Guid.Parse("00000000-0000-0000-0000-000000000101");
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000201");

    private readonly IPermissionService _permissionService;
    private readonly ICurrentOrganizationService _currentOrganizationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionAuthorizationService _permissionAuthorizationService;

    public PermissionsController(
        IPermissionService permissionService,
        ICurrentOrganizationService currentOrganizationService,
        ICurrentUserService currentUserService,
        IPermissionAuthorizationService permissionAuthorizationService)
    {
        _permissionService = permissionService;
        _currentOrganizationService = currentOrganizationService;
        _currentUserService = currentUserService;
        _permissionAuthorizationService = permissionAuthorizationService;
    }

    [HttpGet("/Permissions")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.PermissionsView, cancellationToken))
        {
            return Forbid();
        }

        return View(PermissionListViewModel.Empty());
    }

    [HttpGet("/Permissions/List")]
    public async Task<IActionResult> List([FromQuery] PermissionListFilterRequest request, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.PermissionsView, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var result = await _permissionService.ListAsync(request, cancellationToken);
        return PartialView("_PermissionList", PermissionListViewModel.FromDto(result, request));
    }

    [HttpGet("/Permissions/GetById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.PermissionsUpdate, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var permission = await _permissionService.GetByIdAsync(id, cancellationToken);
        if (permission is null)
        {
            return NotFound(new { succeeded = false, message = "Permission was not found." });
        }

        return Json(PermissionFormViewModel.FromDto(permission));
    }

    [HttpPost("/Permissions/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] PermissionCreateRequest request, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.PermissionsCreate, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        request.CreatedBy = GetUserId();
        var result = await _permissionService.CreateAsync(request, cancellationToken);
        return ToJsonResult(result);
    }

    [HttpPost("/Permissions/Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update([FromForm] PermissionUpdateRequest request, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.PermissionsUpdate, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        request.UpdatedBy = GetUserId();
        var result = await _permissionService.UpdateAsync(request, cancellationToken);
        return ToJsonResult(result);
    }

    [HttpPost("/Permissions/Delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(PlatformPermissions.PermissionsDelete, cancellationToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var result = await _permissionService.DeleteAsync(
            new PermissionDeleteRequest { Id = id, DeletedBy = GetUserId() },
            cancellationToken);
        return ToJsonResult(result);
    }

    private JsonResult ToJsonResult(PermissionCommandResult result)
    {
        Response.StatusCode = result.Succeeded ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;
        return Json(new { result.Succeeded, result.Message, result.PermissionId });
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

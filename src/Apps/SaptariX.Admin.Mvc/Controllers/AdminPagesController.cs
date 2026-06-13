using Microsoft.AspNetCore.Mvc;
using SaptariX.Admin.Mvc.Models;
using SaptariX.Admin.Mvc.Services;

namespace SaptariX.Admin.Mvc.Controllers;

public sealed class DashboardController : Controller
{
    private readonly IDashboardPageService _service;

    public DashboardController(IDashboardPageService service)
    {
        _service = service;
    }

    [HttpGet("/Dashboard")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await _service.GetAsync(cancellationToken));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }
}

public sealed class OrganizationsController : StaticPageController<IOrganizationsPageService>
{
    public OrganizationsController(IOrganizationsPageService service) : base(service) { }
}

public sealed class ModulesController : StaticPageController<IModulesPageService>
{
    public ModulesController(IModulesPageService service) : base(service) { }
}

public sealed class RolesController : StaticPageController<IRolesPageService>
{
    public RolesController(IRolesPageService service) : base(service) { }
}

public sealed class PermissionsController : StaticPageController<IPermissionsPageService>
{
    public PermissionsController(IPermissionsPageService service) : base(service) { }
}

public sealed class WorkflowController : StaticPageController<IWorkflowPageService>
{
    public WorkflowController(IWorkflowPageService service) : base(service) { }
}

public sealed class ReportsController : StaticPageController<IReportsPageService>
{
    public ReportsController(IReportsPageService service) : base(service) { }
}

public sealed class AuditLogsController : StaticPageController<IAuditLogsPageService>
{
    public AuditLogsController(IAuditLogsPageService service) : base(service) { }
}

public sealed class SettingsController : StaticPageController<ISettingsPageService>
{
    public SettingsController(ISettingsPageService service) : base(service) { }
}

public abstract class StaticPageController<TService> : Controller
    where TService : IAdminPageService
{
    private readonly TService _service;

    protected StaticPageController(TService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await _service.GetAsync(cancellationToken));
    }
}

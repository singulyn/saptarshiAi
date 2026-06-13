using Microsoft.AspNetCore.Mvc;
using SaptariX.Modules.DynamicForms.Application;
using SaptariX.Modules.DynamicForms.Web.Models;
using SaptariX.Plugin.Abstractions.Organization;

namespace SaptariX.Modules.DynamicForms.Web.Controllers;

public sealed class DynamicFormsController : Controller
{
    private readonly IDynamicFormService _dynamicFormService;
    private readonly ICurrentOrganizationService _currentOrganizationService;

    public DynamicFormsController(
        IDynamicFormService dynamicFormService,
        ICurrentOrganizationService currentOrganizationService)
    {
        _dynamicFormService = dynamicFormService;
        _currentOrganizationService = currentOrganizationService;
    }

    [HttpGet("/DynamicForms")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var organizationId = _currentOrganizationService.OrganizationId ?? Guid.Empty;
        var forms = await _dynamicFormService.GetFormsAsync(organizationId, cancellationToken);
        var model = new DynamicFormsPageViewModel(forms);
        return View(model);
    }
}

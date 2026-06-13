using Microsoft.AspNetCore.Mvc;
using SaptariX.Modules.UIKit.Application;
using SaptariX.Modules.UIKit.Web.Permissions;

namespace SaptariX.Modules.UIKit.Web.Controllers;

public sealed class UIComponentsController : Controller
{
    private readonly IUiComponentCatalogService _catalogService;

    public UIComponentsController(IUiComponentCatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet("/UIComponents")]
    public IActionResult Index() => ComponentView("Overview");

    [HttpGet("/UIComponents/FormControls")]
    public IActionResult FormControls() => ComponentView(nameof(FormControls));

    [HttpGet("/UIComponents/Tables")]
    public IActionResult Tables() => ComponentView(nameof(Tables));

    [HttpGet("/UIComponents/TablePatterns")]
    public IActionResult TablePatterns() => ComponentView(nameof(TablePatterns));

    [HttpGet("/UIComponents/InlineCreateTables")]
    public IActionResult InlineCreateTables() => ComponentView(nameof(InlineCreateTables));

    [HttpGet("/UIComponents/InputTables")]
    public IActionResult InputTables() => ComponentView(nameof(InputTables));

    [HttpGet("/UIComponents/FilterToolbars")]
    public IActionResult FilterToolbars() => ComponentView(nameof(FilterToolbars));

    [HttpGet("/UIComponents/Modals")]
    public IActionResult Modals() => ComponentView(nameof(Modals));

    [HttpGet("/UIComponents/Drawers")]
    public IActionResult Drawers() => ComponentView(nameof(Drawers));

    [HttpGet("/UIComponents/Cards")]
    public IActionResult Cards() => ComponentView(nameof(Cards));

    [HttpGet("/UIComponents/Buttons")]
    public IActionResult Buttons() => ComponentView(nameof(Buttons));

    [HttpGet("/UIComponents/IconLibrary")]
    public IActionResult IconLibrary() => ComponentView(nameof(IconLibrary));

    [HttpGet("/UIComponents/TabsAccordions")]
    public IActionResult TabsAccordions() => ComponentView(nameof(TabsAccordions));

    [HttpGet("/UIComponents/AlertsToasts")]
    public IActionResult AlertsToasts() => ComponentView(nameof(AlertsToasts));

    [HttpGet("/UIComponents/Loaders")]
    public IActionResult Loaders() => ComponentView(nameof(Loaders));

    [HttpGet("/UIComponents/EmptyStates")]
    public IActionResult EmptyStates() => ComponentView(nameof(EmptyStates));

    [HttpGet("/UIComponents/LayoutExamples")]
    public IActionResult LayoutExamples() => ComponentView(nameof(LayoutExamples));

    private IActionResult ComponentView(string pageKey)
    {
        var page = _catalogService.GetPage(pageKey);
        ViewData["ActiveMenu"] = page.ActiveRoute;
        ViewData["Permission"] = UIKitPermissions.View;
        return View(page);
    }
}

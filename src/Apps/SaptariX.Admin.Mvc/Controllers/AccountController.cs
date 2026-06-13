using Microsoft.AspNetCore.Mvc;
using SaptariX.Admin.Mvc.Models;
using SaptariX.Admin.Mvc.Services;

namespace SaptariX.Admin.Mvc.Controllers;

public sealed class AccountController : Controller
{
    private readonly IAdminAuthenticationService _authenticationService;

    public AccountController(IAdminAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpGet("/Account/Login")]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("/Account/Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError(string.Empty, "Email and password are required.");
            return View(model);
        }

        await _authenticationService.SignInAsync(HttpContext, model);
        return LocalRedirect(string.IsNullOrWhiteSpace(model.ReturnUrl) ? "/Dashboard" : model.ReturnUrl);
    }

    [HttpGet("/Account/Logout")]
    public async Task<IActionResult> Logout()
    {
        await _authenticationService.SignOutAsync(HttpContext);
        return RedirectToAction(nameof(Login));
    }
}

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using SaptariX.Admin.Mvc.Navigation;
using SaptariX.Admin.Mvc.Services;
using SaptariX.Elsa;
using SaptariX.Modules.DynamicForms.Web;
using SaptariX.Modules.UIKit.Web;
using SaptariX.Observability;
using SaptariX.Persistence.SqlServer;
using SaptariX.Platform.AccessControl;
using SaptariX.Platform.Identity;
using SaptariX.Platform.ModuleRegistry;
using SaptariX.Platform.Organization;
using SaptariX.Plugin.Abstractions.Modules;
using SaptariX.Plugin.Abstractions.Navigation;
using SaptariX.Plugin.Abstractions.Permissions;
using SaptariX.Plugin.Abstractions.Workflows;
using SaptariX.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.UseSaptariXSerilog();

var dynamicFormsModule = new DynamicFormsModule();
var uiKitModule = new UIKitModule();

builder.Services
    .AddControllersWithViews()
    .AddApplicationPart(typeof(DynamicFormsModule).Assembly)
    .AddApplicationPart(typeof(UIKitModule).Assembly);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtectionKeys")))
    .SetApplicationName("SaptariX.Admin.Mvc");

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "SaptariX.Admin";
    });

builder.Services.AddAuthorization();
builder.Services.AddSaptariXSqlServerPersistence(builder.Configuration);
builder.Services.AddSaptariXElsa(builder.Configuration);
builder.Services.AddSaptariXRedisReadyCache(builder.Configuration);
builder.Services.AddOrganizationPlatform();
builder.Services.AddIdentityPlatform();
builder.Services.AddAccessControlPlatform();
builder.Services.AddPlatformModuleRegistry();
builder.Services.AddAdminMvcServices();
builder.Services.AddSingleton<IMenuProvider, PlatformMenuProvider>();
builder.Services.AddSingleton<IPlatformModule>(dynamicFormsModule);
builder.Services.AddSingleton<IPlatformModule>(uiKitModule);
dynamicFormsModule.AddServices(builder.Services, builder.Configuration);
uiKitModule.AddServices(builder.Services, builder.Configuration);

var app = builder.Build();

var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Dashboard/Error");
    app.UseHsts();
}

RegisterPlatformContributions(app.Services);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/Dashboard"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "SaptariX.Admin.Mvc",
    timestampUtc = DateTimeOffset.UtcNow
}));

app.Run();

static void RegisterPlatformContributions(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();

    var menuRegistry = scope.ServiceProvider.GetRequiredService<IMenuRegistry>();
    foreach (var provider in scope.ServiceProvider.GetServices<IMenuProvider>())
    {
        provider.RegisterMenus(menuRegistry);
    }

    var permissionRegistry = scope.ServiceProvider.GetRequiredService<IPermissionRegistry>();
    foreach (var provider in scope.ServiceProvider.GetServices<IPermissionProvider>())
    {
        provider.RegisterPermissions(permissionRegistry);
    }

    var workflowRegistry = scope.ServiceProvider.GetRequiredService<IWorkflowActivityRegistry>();
    foreach (var provider in scope.ServiceProvider.GetServices<IWorkflowActivityProvider>())
    {
        provider.RegisterWorkflowActivities(workflowRegistry);
    }

    foreach (var module in scope.ServiceProvider.GetServices<IPlatformModule>())
    {
        module.RegisterMenus(menuRegistry);
        module.RegisterPermissions(permissionRegistry);
        module.RegisterWorkflowActivities(workflowRegistry);
    }
}

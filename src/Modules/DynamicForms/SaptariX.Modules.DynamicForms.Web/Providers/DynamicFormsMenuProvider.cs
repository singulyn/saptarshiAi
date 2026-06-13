using SaptariX.Modules.DynamicForms.Web.Permissions;
using SaptariX.Plugin.Abstractions.Navigation;

namespace SaptariX.Modules.DynamicForms.Web.Providers;

public sealed class DynamicFormsMenuProvider : IMenuProvider
{
    public void RegisterMenus(IMenuRegistry registry)
    {
        registry.Add(new MenuItemDefinition("Dynamic Forms", "/DynamicForms", DynamicFormsPermissions.View, "fa-solid fa-rectangle-list", 60));
    }
}

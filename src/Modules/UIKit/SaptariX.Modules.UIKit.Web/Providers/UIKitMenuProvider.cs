using SaptariX.Modules.UIKit.Web.Permissions;
using SaptariX.Plugin.Abstractions.Navigation;

namespace SaptariX.Modules.UIKit.Web.Providers;

public sealed class UIKitMenuProvider : IMenuProvider
{
    public void RegisterMenus(IMenuRegistry registry)
    {
        registry.Add(new MenuItemDefinition("Developer Tools", "#", UIKitPermissions.View, "fa-solid fa-code", 900));
        registry.Add(new MenuItemDefinition("Overview", "/UIComponents", UIKitPermissions.View, "fa-solid fa-layer-group", 911, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Form Controls", "/UIComponents/FormControls", UIKitPermissions.View, "fa-solid fa-rectangle-list", 912, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Tables", "/UIComponents/Tables", UIKitPermissions.View, "fa-solid fa-table", 913, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Table Patterns", "/UIComponents/TablePatterns", UIKitPermissions.View, "fa-solid fa-table-list", 914, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Inline Create Tables", "/UIComponents/InlineCreateTables", UIKitPermissions.View, "fa-solid fa-square-plus", 915, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Input Tables", "/UIComponents/InputTables", UIKitPermissions.View, "fa-solid fa-keyboard", 916, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Filter Toolbars", "/UIComponents/FilterToolbars", UIKitPermissions.View, "fa-solid fa-filter", 917, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Modals & Popups", "/UIComponents/Modals", UIKitPermissions.View, "fa-solid fa-window-maximize", 918, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Right Drawers", "/UIComponents/Drawers", UIKitPermissions.View, "fa-solid fa-window-restore", 919, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Cards", "/UIComponents/Cards", UIKitPermissions.View, "fa-regular fa-id-card", 920, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Buttons & Badges", "/UIComponents/Buttons", UIKitPermissions.View, "fa-solid fa-toggle-on", 921, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Icon Library", "/UIComponents/IconLibrary", UIKitPermissions.View, "fa-solid fa-icons", 922, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Tabs & Accordions", "/UIComponents/TabsAccordions", UIKitPermissions.View, "fa-solid fa-folder-open", 923, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Alerts & Toasts", "/UIComponents/AlertsToasts", UIKitPermissions.View, "fa-solid fa-bell", 924, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Loaders & Spinners", "/UIComponents/Loaders", UIKitPermissions.View, "fa-solid fa-spinner", 925, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Empty States", "/UIComponents/EmptyStates", UIKitPermissions.View, "fa-solid fa-inbox", 926, "Developer Tools"));
        registry.Add(new MenuItemDefinition("Layout Examples", "/UIComponents/LayoutExamples", UIKitPermissions.View, "fa-solid fa-table-columns", 927, "Developer Tools"));
    }
}

using SaptariX.Modules.UIKit.Contracts;

namespace SaptariX.Modules.UIKit.Application;

public sealed class UiComponentCatalogService : IUiComponentCatalogService
{
    private static readonly IReadOnlyList<UiComponentCardDto> ComponentCards =
    [
        new("Form Controls", "Inputs, selects, validation, and compact form layouts.", "/UIComponents/FormControls", "fa-solid fa-rectangle-list"),
        new("Tables", "Responsive AdminLTE tables for list and action-heavy screens.", "/UIComponents/Tables", "fa-solid fa-table"),
        new("Table Patterns", "Enterprise table cards with top search, filters, loading, empty, and pagination states.", "/UIComponents/TablePatterns", "fa-solid fa-table-list"),
        new("Inline Create Tables", "Small master data create/update directly above compact entity tables.", "/UIComponents/InlineCreateTables", "fa-solid fa-square-plus"),
        new("Input Tables", "Editable table rows for builders, pricing, and config screens.", "/UIComponents/InputTables", "fa-solid fa-keyboard"),
        new("Filter Toolbars", "Reusable top filter bars with search, status, type, date, scope, and actions.", "/UIComponents/FilterToolbars", "fa-solid fa-filter"),
        new("Modals & Popups", "Confirmation, form, detail, success, and error modal patterns.", "/UIComponents/Modals", "fa-solid fa-window-maximize"),
        new("Right Drawers", "Offcanvas patterns for filters, forms, details, and permissions.", "/UIComponents/Drawers", "fa-solid fa-window-restore"),
        new("Cards", "Admin cards, stats, progress, collapsible, and empty card patterns.", "/UIComponents/Cards", "fa-regular fa-id-card"),
        new("Buttons & Badges", "Command buttons, icon actions, loading states, and badges.", "/UIComponents/Buttons", "fa-solid fa-toggle-on"),
        new("Icon Library", "Font Awesome Free reference for platform navigation, actions, and status icons.", "/UIComponents/IconLibrary", "fa-solid fa-icons"),
        new("Tabs & Accordions", "Tabbed workflows, accordions, and update-mode patterns.", "/UIComponents/TabsAccordions", "fa-solid fa-folder-open"),
        new("Alerts & Toasts", "Inline alerts and reusable toast helper methods.", "/UIComponents/AlertsToasts", "fa-solid fa-bell"),
        new("Loaders & Spinners", "Spinners, overlays, loading rows, and skeleton states.", "/UIComponents/Loaders", "fa-solid fa-spinner"),
        new("Empty States", "No-records, disabled modules, no permissions, and error states.", "/UIComponents/EmptyStates", "fa-solid fa-inbox"),
        new("Layout Examples", "Page composition patterns for common enterprise workflows.", "/UIComponents/LayoutExamples", "fa-solid fa-table-columns")
    ];

    public UiComponentPageDto GetPage(string pageKey)
    {
        return pageKey switch
        {
            "FormControls" => Build("Form Controls", "Controls", "Reusable AdminLTE form inputs and compact layout patterns.", "/UIComponents/FormControls", FormExamples()),
            "Tables" => Build("Tables", "Data Grids", "Responsive table patterns for Users, Roles, Organizations, Forms, and Reports.", "/UIComponents/Tables", TableExamples()),
            "TablePatterns" => Build("Table Patterns", "Enterprise Tables", "Reusable table layouts with top search, filters, loading, empty, actions, and pagination.", "/UIComponents/TablePatterns", TablePatternExamples()),
            "InlineCreateTables" => Build("Inline Create Tables", "Small Masters", "Create and update simple entities directly above the table when the form has two to five fields.", "/UIComponents/InlineCreateTables", InlineCreateExamples()),
            "InputTables" => Build("Input Tables", "Editable Tables", "Input-heavy row editing patterns for builders and setup screens.", "/UIComponents/InputTables", InputTableExamples()),
            "FilterToolbars" => Build("Filter Toolbars", "Filters", "Reusable search and filter toolbar patterns that stack cleanly across screen sizes.", "/UIComponents/FilterToolbars", FilterToolbarExamples()),
            "Modals" => Build("Modals & Popups", "Dialogs", "Bootstrap modal patterns with consistent confirmation wording.", "/UIComponents/Modals", ModalExamples()),
            "Drawers" => Build("Right Drawers", "Offcanvas", "Right-side drawers for forms, details, filters, and permissions.", "/UIComponents/Drawers", DrawerExamples()),
            "Cards" => Build("Cards", "Containers", "Card patterns for compact enterprise admin pages.", "/UIComponents/Cards", CardExamples()),
            "Buttons" => Build("Buttons & Badges", "Commands", "Consistent buttons, badges, and table action icons.", "/UIComponents/Buttons", ButtonExamples()),
            "IconLibrary" => Build("Icon Library", "Font Awesome", "Font Awesome Free usage reference for SaptariX platform icons.", "/UIComponents/IconLibrary", IconLibraryExamples()),
            "TabsAccordions" => Build("Tabs & Accordions", "Navigation", "Tabbed workflows and collapsible permission groups.", "/UIComponents/TabsAccordions", TabExamples()),
            "AlertsToasts" => Build("Alerts & Toasts", "Feedback", "Inline feedback and toast helper patterns.", "/UIComponents/AlertsToasts", AlertExamples()),
            "Loaders" => Build("Loaders & Spinners", "Loading States", "Seven loading patterns plus permission drawer loading examples.", "/UIComponents/Loaders", LoaderExamples()),
            "EmptyStates" => Build("Empty States", "No Data", "Reusable empty and error states with optional actions.", "/UIComponents/EmptyStates", EmptyStateExamples()),
            "LayoutExamples" => Build("Layout Examples", "Page Patterns", "Reusable page layouts for future modules.", "/UIComponents/LayoutExamples", LayoutExamples()),
            _ => new UiComponentPageDto(
                "UI Components",
                "Developer Tools",
                "Internal reusable AdminLTE v4 component catalogue for SuperAdmin and Developer users.",
                "/UIComponents",
                ComponentCards,
                OverviewExamples())
        };
    }

    private static UiComponentPageDto Build(
        string title,
        string kicker,
        string summary,
        string activeRoute,
        IReadOnlyList<UiComponentExampleDto> examples)
    {
        return new UiComponentPageDto(title, kicker, summary, activeRoute, [], examples);
    }

    private static IReadOnlyList<UiComponentExampleDto> OverviewExamples()
    {
        return
        [
            new("Component usage", "Start from the closest UI Kit page, copy the snippet, then wire it to module services.", "<partial name=\"UIComponents/_UiCard\" />", "Keep future module pages compact and consistent with AdminLTE."),
            new("Standard action flow", "List pages should use tabs, card tables, drawer details, and modal confirmation.", "<button class=\"btn btn-primary btn-sm\">Create</button>", "Avoid browser confirm dialogs and page-level edit screens unless the workflow demands them.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> FormExamples()
    {
        return
        [
            new("Basic inputs", "Use for text, email, password, number, mobile, search, disabled, and readonly fields.", "<input class=\"form-control form-control-sm\" name=\"Email\" type=\"email\" required />", "Use `form-control-sm` for dense enterprise screens."),
            new("Advanced inputs", "Use select, textarea, date, file, toggle, checkbox, radio, help text, and validation states.", "<select class=\"form-select form-select-sm\"><option>Active</option></select>", "Put validation text directly under the field."),
            new("Form layouts", "Use one-column, two-column, compact card, and sticky footer form patterns.", "<div class=\"row g-3\"><div class=\"col-md-6\">...</div></div>", "Use sticky footers for drawers and long forms.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> TableExamples()
    {
        return
        [
            new("Basic table", "Use for simple lookup lists with five to six columns.", "<table class=\"table table-hover align-middle\">...</table>", "Always wrap tables in `.table-responsive`."),
            new("Advanced table", "Use for Users, Roles, Organizations, Forms, and Reports.", "<partial name=\"UIComponents/_UiAdvancedTable\" />", "Include loading, empty, pagination, and compact actions."),
            new("Enterprise top toolbar", "Use when search and filters must be visible above the grid.", "<partial name=\"UIComponents/_UiEnterpriseTable\" />", "Keep filters compact and stack them on mobile."),
            new("Action table", "Use when every row has view, edit, permissions/settings, and soft-delete actions.", "<td class=\"text-end\"><partial name=\"UIComponents/_UiActionButtons\" /></td>", "Keep actions compact, right-aligned, and icon-only with accessible labels."),
            new("Empty and loading states", "Use while searching, filtering, or waiting for records.", "<partial name=\"UIComponents/_UiEmptyState\" />", "Pair loading and empty states with the same table frame.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> TablePatternExamples()
    {
        return
        [
            new("Enterprise table", "Use for primary list pages with search, filters, loading, empty, pagination, and actions.", "<partial name=\"UIComponents/_UiEnterpriseTable\" />", "This is the default list pattern for future entity modules."),
            new("Top toolbar", "Keep filters above the grid when users compare and refine results repeatedly.", "<partial name=\"UIComponents/_UiFilterToolbar\" />", "Move advanced filters to a right drawer only when the toolbar becomes crowded."),
            new("Action cell", "Keep action buttons narrow and right-aligned.", "<td class=\"sx-action-cell\"><button class=\"btn btn-light btn-sm fa-icon-btn\">...</button></td>", "Use view, edit, settings/permissions, and soft delete actions consistently.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> InlineCreateExamples()
    {
        return
        [
            new("Inline create", "Use for small master data with two to five simple fields.", "<partial name=\"UIComponents/_UiInlineCreateTable\" />", "Do not use this pattern for permission matrices, long forms, or workflow builders."),
            new("Edit mode", "The same top section becomes update mode when a row edit action is clicked.", "formTitle.textContent = 'Update Role Category';", "Show Cancel Edit and reset back to create mode after save."),
            new("Soft delete", "Delete actions must use a Bootstrap confirmation modal.", "<button data-bs-toggle=\"modal\" data-bs-target=\"#uiInlineSoftDeleteModal\">...</button>", "Do not use browser confirm dialogs.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> FilterToolbarExamples()
    {
        return
        [
            new("Reusable toolbar", "Use search, status, type, date range, scope, reset, and apply actions.", "<partial name=\"UIComponents/_UiFilterToolbar\" />", "Toolbar controls should stack on mobile."),
            new("Table filter bar", "Keep filters in the card body above the table.", "<div class=\"card-body\"><partial name=\"UIComponents/_UiFilterToolbar\" /></div>", "Avoid repeating custom filter CSS in page views."),
            new("Optional create action", "Add create only when the table does not already have a primary action in the header.", "<button class=\"btn btn-success btn-sm\">Create</button>", "Prefer one obvious primary action per card.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> InputTableExamples()
    {
        return
        [
            new("Basic input table", "Use for builder rows with name, type, required, sort order, and actions.", "<tr><td><input class=\"form-control form-control-sm\" /></td></tr>", "Keep inputs compact and row actions fixed width."),
            new("Pricing input table", "Use for plan and pricing setup.", "<input class=\"form-control form-control-sm\" type=\"number\" min=\"0\" />", "Validate row-level errors near the row."),
            new("Dynamic form builder table", "Use for DynamicForms and AppBuilder field setup.", "<select class=\"form-select form-select-sm\"><option>Text</option></select>", "This is the preferred pattern for metadata builders.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> ModalExamples()
    {
        return
        [
            new("Delete confirmation", "Use for soft-delete operations only.", "<div class=\"modal\"><div class=\"modal-body\">Are you sure you want to delete this record?</div></div>", "Do not use `confirm()`."),
            new("Form modal", "Use for short forms that do not need a full page or drawer.", "<div class=\"modal-dialog modal-lg\">...</div>", "Use drawers for longer workflows."),
            new("Feedback modal", "Use success and error modals only when a toast is not enough.", "<button class=\"btn btn-success btn-sm\">Done</button>", "Prefer toasts for routine saves.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> DrawerExamples()
    {
        return
        [
            new("Basic drawer", "Use for detail panels and secondary workflows.", "<div class=\"offcanvas offcanvas-end ui-drawer\">...</div>", "Desktop width should stay around 420px to 520px."),
            new("Permission drawer", "Use module-wise accordion permission groups with search and select all.", "<div class=\"accordion\"><label class=\"form-check\">...</label></div>", "Reuse this pattern for Users and Roles permission management."),
            new("Filter drawer", "Use for advanced filters when the table header would become crowded.", "<button data-bs-toggle=\"offcanvas\">Filters</button>", "Keep footer actions sticky.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> CardExamples()
    {
        return
        [
            new("Basic card", "Use for standalone content sections.", "<div class=\"card\"><div class=\"card-body\">...</div></div>", "Keep radius aligned with project standard."),
            new("Stats card", "Use for dashboards and summaries.", "<div class=\"metric-tile\"><span>Users</span><strong>128</strong></div>", "Use concise labels and clear hierarchy."),
            new("Table card", "Use for list screens.", "<div class=\"card\"><div class=\"table-responsive\">...</div></div>", "Avoid nested cards.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> ButtonExamples()
    {
        return
        [
            new("Buttons", "Use Bootstrap semantic button colors consistently.", "<button class=\"btn btn-primary btn-sm\">Save</button>", "Use small buttons in dense admin screens."),
            new("Badges", "Use badges for status, role, and labels.", "<span class=\"badge text-bg-success\">Active</span>", "Avoid inventing new colors for known states."),
            new("Action icons", "Use compact icon buttons for table actions.", "<button class=\"btn btn-light btn-sm ui-icon-button\">...</button>", "Keep action columns clean and right-aligned."),
            new("Advanced Buttons 1", "Gradient command buttons adapted from the Button 42 pattern.", "<button class=\"ui-adv-btn ui-adv-btn-gradient ui-adv-btn-sunset\">Primary</button>", "Use for high-emphasis primary actions."),
            new("Advanced Buttons 2", "Solid glow buttons adapted from the Button 34 pattern.", "<button class=\"ui-adv-btn ui-adv-btn-glow ui-adv-btn-violet\">Primary</button>", "Use for compact premium actions."),
            new("Advanced Buttons 3", "Soft inset buttons adapted from the Button 33 pattern.", "<button class=\"ui-adv-btn ui-adv-btn-inset ui-adv-btn-mint\">Primary</button>", "Use for positive secondary actions."),
            new("Advanced Buttons 4", "Dimensional radial buttons adapted from the Button 29 pattern.", "<button class=\"ui-adv-btn ui-adv-btn-radial ui-adv-btn-radial-blue\">Primary</button>", "Use sparingly for prominent workflows."),
            new("Advanced Buttons 5", "Elevated flat buttons adapted from the Button 5 pattern.", "<button class=\"ui-adv-btn ui-adv-btn-elevated ui-adv-btn-orange\">Primary</button>", "Use for direct command actions."),
            new("Advanced Buttons 6", "Warm paper buttons adapted from the Button 83 pattern.", "<button class=\"ui-adv-btn ui-adv-btn-paper ui-adv-btn-cream\">Primary</button>", "Use for softer utility actions.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> IconLibraryExamples()
    {
        return
        [
            new("Sidebar icons", "Use Font Awesome classes directly in menu definitions.", "new MenuItemDefinition(\"Organizations\", \"/Organizations\", permission, \"fa-solid fa-building\", 20);", "Load only solid and regular Font Awesome Free styles globally."),
            new("Table actions", "Use icon-only buttons with accessible labels.", "<button class=\"btn btn-light btn-sm fa-icon-btn\" aria-label=\"Edit\"><i class=\"fa-solid fa-pen-to-square\"></i></button>", "Keep action buttons compact and right-aligned."),
            new("Status feedback", "Pair semantic colors with status icons.", "<span class=\"badge text-bg-success\"><i class=\"fa-solid fa-circle-check\"></i> Active</span>", "Use solid icons for status indicators.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> TabExamples()
    {
        return
        [
            new("Create/List tabs", "Use for list plus create workflows.", "<ul class=\"nav nav-tabs\"><button class=\"nav-link active\">List</button></ul>", "Default to the list tab."),
            new("Update mode tab", "Reuse the create form tab for edits.", "formTab.textContent = 'Update User';", "This is the Users module pattern."),
            new("Permission accordion", "Use collapsible groups for module permissions.", "<div class=\"accordion-item\">...</div>", "Group by module name.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> AlertExamples()
    {
        return
        [
            new("Alerts", "Use inline alerts for page-level feedback.", "<div class=\"alert alert-success\">Saved successfully.</div>", "Make alerts dismissible when they are not blocking."),
            new("Toasts", "Use helper methods for AJAX saves.", "window.SaptariXToast.success('Saved successfully.');", "Toast host is available through `_UiToastHost`."),
            new("AJAX errors", "Use error toasts for failed async actions.", "window.SaptariXToast.error('Unable to save.');", "Keep error copy clear and actionable.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> LoaderExamples()
    {
        return
        [
            new("Spinners", "Use border, grow, small, medium, and large spinners.", "<div class=\"spinner-border spinner-border-sm\"></div>", "Use inline loading text for short waits."),
            new("Table loading", "Use loading rows while list data is fetching.", "<tr><td colspan=\"5\">Loading records...</td></tr>", "Keep table dimensions stable."),
            new("Skeleton loader", "Use skeleton blocks for card and detail loading.", "<div class=\"ui-skeleton\"></div>", "Skeletons should be lightweight.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> EmptyStateExamples()
    {
        return
        [
            new("No records", "Use when a list has no data yet.", "<partial name=\"UIComponents/_UiEmptyState\" />", "Offer one clear action when appropriate."),
            new("Search returned no result", "Use when filtering hides all data.", "<div class=\"ui-empty-state\">No matches</div>", "Tell the user to adjust filters."),
            new("Error loading data", "Use when a fetch fails.", "<div class=\"alert alert-danger\">Unable to load data.</div>", "Do not leave the table blank.")
        ];
    }

    private static IReadOnlyList<UiComponentExampleDto> LayoutExamples()
    {
        return
        [
            new("Two-column layout", "Use for master-detail and settings screens.", "<div class=\"row g-3\"><aside class=\"col-lg-4\">...</aside><main class=\"col-lg-8\">...</main></div>", "Keep columns balanced at desktop sizes."),
            new("Table plus drawer", "Use for list screens with details or permissions.", "<button data-bs-toggle=\"offcanvas\">Open drawer</button>", "Preferred over separate edit pages."),
            new("Sticky footer form", "Use for long forms and drawers.", "<div class=\"ui-sticky-footer\">...</div>", "Keep primary actions visible.")
        ];
    }
}

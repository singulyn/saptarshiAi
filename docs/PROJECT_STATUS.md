# Project Status

Current date: 2026-06-17

## Completed

- Created `SaptariX.Platform.sln` with app, core, infrastructure, platform, module, worker, and test projects.
- Targeted projects to `net8.0`.
- Integrated AdminLTE v4 compiled assets into Admin MVC.
- Built Admin MVC dashboard layout with header, sidebar, footer, breadcrumb, user menu, organization switcher, notifications, and dark-mode-ready AdminLTE shell.
- Added Organization platform services and entities.
- Added SQL Server/Dapper persistence abstractions and implementation.
- Added initial SQL Server scripts.
- Added identity, access control, module registry, workflow, Redis-ready cache, and Serilog boundaries.
- Implemented DynamicForms sample module.
- Added future FastAPI, Node.js, and Java service placeholders.
- Added documentation and AI handover files.
- Verified `dotnet restore SaptariX.Platform.sln` succeeds.
- Verified `dotnet build SaptariX.Platform.sln --no-restore` succeeds with 0 warnings and 0 errors.
- Verified solution membership contains all app, core, infrastructure, platform, DynamicForms, worker, and test projects.
- Verified all project files target `net8.0`.
- Verified Admin MVC project references the required core, infrastructure, platform, and DynamicForms module projects.
- Verified Admin MVC responds on `/Dashboard`, `/DynamicForms`, `/Modules`, and `/Account/Login`.
- Verified AdminLTE CSS and JS assets are present under `wwwroot/vendor/adminlte` and served successfully.
- Verified SQL Server/Dapper packages and service registrations are present.
- Verified DynamicForms module registration, menu, permission, workflow activity provider, and MVC route are active.
- Implemented Users module UI and backend flow.
- Added Users sidebar menu integration with `Users.View` permission.
- Added AdminLTE tabbed Users page with User List and Create/Update User form tabs.
- Added advanced Users table with search, status filter, pagination, sort-ready columns, empty/loading states, and action buttons.
- Added shared Create/Update form behavior with create-only password fields and Cancel Edit reset behavior.
- Added Bootstrap/AdminLTE soft-delete confirmation modal with no hard delete path.
- Added right-side User Permissions drawer with permission search, collapsible groups, Select All, and save flow.
- Added Users identity services, DTOs, SQL Server/Dapper repositories, and identity stored procedure scripts.
- Verified Users AJAX create, edit load, update, permission save/load, and soft-delete flows with anti-forgery tokens.
- Implemented internal UI Kit / Design System module for Admin MVC developer users.
- Added `Developer.UIComponents.View` permission and Developer Tools sidebar hierarchy.
- Added UI Components pages for overview, form controls, tables, input tables, modals, right drawers, cards, buttons and badges, tabs and accordions, alerts and toasts, loaders and spinners, empty states, and layout examples.
- Added reusable UI component partials under `Views/Shared/UIComponents`.
- Added `ui-kit.css` and `ui-kit.js` with copy-snippet, preview/code toggle, toast helper, loading button, and input-table row add/remove behavior.
- Verified all UI Kit routes and static assets return `200`.
- Added global UI customization source at `wwwroot/scss/saptarix-ui.scss` with modular abstracts, components, and utilities partials.
- Added compiled global runtime CSS at `wwwroot/css/saptarix-ui.css` and loaded it globally in the Admin layout.
- Added reusable enterprise table, inline-create table, and filter toolbar partials.
- Added UI Kit pages and menu entries for Table Patterns, Inline Create Tables, and Filter Toolbars.
- Added entity UI pattern decision rules to handover and module-development docs.
- Verified new UI Kit table pattern routes and `saptarix-ui.css` return `200`.
- Fixed Developer Tools/UI Components sidebar treeview expand behavior.
- Fixed sidebar scroll persistence across page navigation so lower menu links do not jump back to the top.
- Updated global form control styling so focused inputs/selects/checkboxes use a clean black border with no blue glow and standardized enterprise input height/padding.
- Added a polished custom UI Kit select dropdown pattern with styled option panel, selected state, status markers, native select sync, and keyboard/click handling.
- Replaced remaining Bootstrap blue accents in primary buttons, outline-primary states, checked switches, checkboxes, radios, links, and root primary tokens with the sidebar accent `#000d0f`.
- Added a global dropdown enhancer that converts standard Bootstrap `.form-select` controls into the same premium `sx-select` option panel while preserving native select form binding.
- Added reusable premium scrollbar styling: dropdown option panels hide scrollbars, while table, code, and drawer scroll areas use thin `6px` black `#000d0f` scrollbar thumbs.
- Removed remaining Bootstrap blue glow/focus states from form controls, validation controls, buttons, close buttons, and accordions; normal focus resolves to a clean black border, while valid/invalid fields keep green/red borders without shadows.
- Refined UI Kit and Users right drawers with controlled offcanvas widths, neutral premium header/body surfaces, compact card-style content, and non-blue accordion active states.
- Improved Users table filter toolbar so search, status, Apply, and Reset stay in a single aligned desktop row.
- Improved Admin sidebar submenu UX with reference-style parent open state, child indentation, vertical rail, leaf-dot treatment, active child surface, and cleaner spacing; removed the left-arrow child token treatment.
- Updated the reusable UI Kit filter toolbar partial to the same compact enterprise table filter row pattern used by Users.
- Standardized card and table-frame surfaces by removing grey outer borders and applying the shared `--sx-card-shadow` token across Bootstrap cards, Admin panels, Users cards, UI Kit cards, and table-card wrappers.
- Refined UI Kit table demos with a full-width enterprise table, paired compact table examples, attached code previews directly under each related table, richer sample rows, reusable premium pagination, compact Advanced Table filters, 4px status badges, and 5-15px table cell padding.
- Applied the requested bottom shadow to the Admin header using `rgba(0, 0, 0, 0.4) 0px 30px 90px`.
- Added UI Kit Advanced Button Patterns with six named reusable button groups (`Advanced Buttons 1` through `Advanced Buttons 6`), each using 4px radius, inherited SaptariX font family, primary reference styles, and multiple premium palette variants.
- Fixed the Admin shell scroll model so the header and footer stay fixed while only the main content container scrolls.
- Removed the experimental generated UI Kit Icon Library implementation, including registry/sprite assets, TagHelper, generated page partials, JS, and related docs.
- Implemented the UI-only Organization module shell at `/Organizations` with Organization List and Create/Update tabs, enterprise table filters, Advanced Buttons 5 action styling, right drawers for apps/products, modules, domains, and settings, and a soft-delete confirmation modal.
- Updated Organization permission constants to use `Organizations.*` names and changed sidebar/menu icons to Font Awesome Free classes.
- Installed Font Awesome Free runtime assets under `wwwroot/vendor/fontawesome-free`, loaded solid and regular styles globally, removed the custom SaptariX icon sprite assets, and restored `UI Components -> Icon Library` as a lightweight Font Awesome reference page.
- Added a Coolify/GCP-ready Docker Compose deployment with an Admin MVC Dockerfile, private SQL Server/Redis services, persistent volumes, required SQL password configuration, reverse-proxy forwarded header support, and `/health` endpoint.
- Fixed Admin MVC publish output for container builds by preventing the DynamicForms Web module `appsettings*.json` files from colliding with the Admin app settings during publish.
- Simplified AccessControl to an in-memory Admin MVC workflow for roles, permissions, user-role mappings, and role-permission mappings without runtime RBAC enforcement.
- Replaced static Roles and Permissions pages with real Admin MVC controllers, Razor views, AJAX list/form flows, and JavaScript modules.
- Added Role Permissions right drawer and User Roles drawer.
- Changed Users, Roles, and Permissions to direct sidebar entries with no dynamic sidebar permission gating.
- Reused the Organization page shell, filter row, table wrappers, action button styling, and pagination for Users, Roles, and Permissions.

## In Progress

- None.

## Pending

- Server-side Organization module functionality: SQL scripts, stored procedures, Dapper repositories, services, AJAX endpoints, and persistence-backed validation.
- Concrete Elsa package integration.
- Production authentication and JWT issuing.
- Real Redis provider implementation.
- Full database migration runner.
- SQL Server execution test for new Users stored procedures.
- Deeper browser interaction tests for UI Kit copy/toast/drawer/modal and inline-create demos.
- CI pipeline and integration test infrastructure.

## Blockers

- None for build or Admin MVC startup.
- SQL scripts have not been applied to a running SQL Server container yet.

## Verification Notes

- Local Admin MVC URL: `http://localhost:5081`
- Current verified Admin MVC process ID: `6444`
- Verified routes: `/Dashboard`, `/DynamicForms`, `/Modules`, `/Account/Login`, `/Users`
- Verified Users endpoints: `/Users/List`, `/Users/GetById/{id}`, `/Users/Create`, `/Users/Update`, `/Users/SoftDelete/{id}`, `/Users/GetPermissions/{userId}`, `/Users/SavePermissions`
- Verified UI Kit routes: `/UIComponents`, `/UIComponents/FormControls`, `/UIComponents/Tables`, `/UIComponents/TablePatterns`, `/UIComponents/InlineCreateTables`, `/UIComponents/InputTables`, `/UIComponents/FilterToolbars`, `/UIComponents/Modals`, `/UIComponents/Drawers`, `/UIComponents/Cards`, `/UIComponents/Buttons`, `/UIComponents/IconLibrary`, `/UIComponents/TabsAccordions`, `/UIComponents/AlertsToasts`, `/UIComponents/Loaders`, `/UIComponents/EmptyStates`, `/UIComponents/LayoutExamples`
- Verified static assets: `/vendor/adminlte/css/adminlte.min.css`, `/vendor/adminlte/js/adminlte.min.js`
- Verified Font Awesome static assets: `/vendor/fontawesome-free/css/fontawesome.min.css`, `/vendor/fontawesome-free/css/solid.min.css`, `/vendor/fontawesome-free/css/regular.min.css`, `/vendor/fontawesome-free/webfonts/fa-solid-900.woff2`, `/vendor/fontawesome-free/webfonts/fa-regular-400.woff2`
- Verified Dashboard, Organizations, and UI Components Icon Library markup uses Font Awesome classes and does not emit old custom icon references.
- Verified Users static asset: `/js/modules/users.js`
- Verified Organization UI route and asset: `/Organizations` and `/js/modules/organizations.js` return `200`.
- Verified UI Kit static assets: `/css/saptarix-ui.css`, `/css/ui-kit.css`, `/js/modules/ui-kit.js`
- Verified UI Kit form controls in headless Chrome: checked switch, checkbox, radio, and primary button all resolve to `rgb(0, 13, 15)`.
- Verified key Admin MVC dropdown pages in headless Chrome: `/Users`, `/UIComponents/InputTables`, `/UIComponents/InlineCreateTables`, and `/UIComponents/TablePatterns` have `0` visible native Bootstrap select panels after enhancement.
- Verified `/Users` and `/UIComponents/InputTables` scroll styling in headless Chrome: dropdown panel scrollbar resolves to `0px/display:none`, and table scrollbars resolve to `6px` with `rgb(0, 13, 15)` thumbs.
- Verified UI Kit form states in headless Chrome: normal focused input resolves to `rgb(17, 24, 39)` border with `box-shadow: none`; invalid/valid focused inputs keep red/green borders with `box-shadow: none`.
- Verified UI Kit right drawer in headless Chrome: drawer opens at `460px`, uses neutral `rgb(251, 252, 253)` surfaces, and active accordion no longer uses Bootstrap blue.
- Verified `/Users` filter toolbar in headless Chrome: search, status, Apply, and Reset use `flex-wrap: nowrap`, strictly increasing left positions, and center-aligned controls.
- Verified `/Users` filter toolbar in headless Chrome: Apply and Reset resolve to compact `32px` button height.
- Verified reusable UI Kit filter toolbar in headless Chrome on `/UIComponents/TablePatterns`: toolbar uses `flex-wrap: nowrap`, 4 controls, and center-aligned row layout.
- Verified Admin sidebar submenu in headless Chrome: Developer Tools is open, child token left-arrow content resolves to `none`, leaf token uses dot content, and Developer Tools icon transform resolves to `none`.
- Verified served CSS assets expose `--sx-card-shadow`; card/table wrappers in `site.css`, `saptarix-ui.css`, and `ui-kit.css` now resolve to `border: 0` with the shared card shadow declaration.
- Verified `/UIComponents/Tables`, `/UIComponents/TablePatterns`, `/css/site.css`, `/css/saptarix-ui.css`, `/css/ui-kit.css`, and `/js/modules/ui-kit.js` return `200` after the table UI pass.
- Verified `/UIComponents/Tables` markup includes reusable `sx-pagination`, attached code previews for enterprise/basic/advanced/action/empty examples, and additional sample rows.
- Verified served CSS includes the Admin header bottom shadow, premium pagination button shadow, global 4px badge radius, compact table padding, and compact UI Kit table toolbar sizing.
- Verified `/UIComponents/Buttons` and `/css/ui-kit.css` return `200`; the Buttons page includes `Advanced Button Patterns`, all six named groups, and served CSS includes the reusable `ui-adv-btn` families with 4px radius and inherited font.
- Verified after removing the generated custom Icon Library implementation that `dotnet build SaptariX.Platform.sln --no-restore` succeeds with 0 warnings and 0 errors.
- Verified after the Organization UI-only shell that `dotnet build SaptariX.Platform.sln --no-restore` succeeds with 0 warnings and 0 errors.
- Verified after the Font Awesome icon migration that `dotnet build SaptariX.Platform.sln --no-restore` succeeds with 0 warnings and 0 errors.
- Verified after the Coolify compose update that `docker-compose config --quiet` renders successfully when `SAPTARIX_SQL_PASSWORD` is set, `dotnet publish src/Apps/SaptariX.Admin.Mvc/SaptariX.Admin.Mvc.csproj -c Release -o .tmp/publish-admin-mvc-check --no-restore` succeeds, and `dotnet build SaptariX.Platform.sln --no-restore` succeeds with 0 warnings and 0 errors.
- Verified after simplifying AccessControl that `dotnet build src/Apps/SaptariX.Admin.Mvc/SaptariX.Admin.Mvc.csproj --no-restore` succeeds with 0 warnings and 0 errors.
- The local machine has ASP.NET Core 10 installed but not the ASP.NET Core 8 shared runtime, so local startup uses `DOTNET_ROLL_FORWARD=Major` while projects remain targeted to `net8.0`.
- A sandboxed package-list command attempted network restore and failed under restricted network rules; package references were verified directly from project files and the solution build succeeds.

## Next Recommended Steps

1. Apply SQL scripts to local SQL Server.
2. Implement Organization backend persistence: SQL scripts, stored procedures, repositories, services, and controller endpoints.
3. Validate the Users module against SQL Server stored procedures instead of the local fallback store.
4. Browser-verify Roles, Permissions, user-role drawer, role-permission drawer, and the direct sidebar entries.
5. Use the UI Kit inline-create table pattern for small master modules.
6. Use the UI Kit input-table pattern in DynamicForms/AppBuilder module work.
7. Replace workflow placeholder runtime with concrete Elsa integration.
8. Add integration tests once SQL Server scripts are applied.

## Roadmap

- Phase 1: Platform scaffold and Admin MVC shell.
- Phase 2: Identity, organizations, and access control persistence.
- Phase 3: DynamicForms CRUD and workflow execution.
- Phase 4: Reporting, notifications, and audit hardening.
- Phase 5: External service contracts and deployment automation.

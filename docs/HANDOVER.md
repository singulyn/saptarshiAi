# Handover

## Project Overview

SaptariX Platform is a fresh ASP.NET Core MVC SaaS platform scaffolded as a plugin-oriented modular monolith. The primary admin app is `SaptariX.Admin.Mvc`; supporting API shells, platform libraries, infrastructure libraries, modules, workers, database scripts, docs, and external-service placeholders are included.

## Architecture Summary

- Core projects define domain primitives, application results, shared contracts, and plugin abstractions.
- Infrastructure projects own SQL Server/Dapper, workflow, cache, logging, files, email, security, and external API boundaries.
- Platform projects own Organization, Identity, Access Control, Module Registry, Configuration, and Audit capabilities.
- Modules are isolated by Application, Infrastructure, Web, and Contracts projects.
- Future services under `services` communicate through `/contracts` and do not read SQL Server tables directly.

## Current Stack

.NET 8, ASP.NET Core MVC/Web API, Razor Views, AdminLTE v4, Bootstrap 5, SQL Server, Dapper, stored procedures, Serilog, Elsa-ready workflow boundaries, and Redis-ready cache abstraction.

## Folder Structure

- `src/Apps`: entrypoints.
- `src/Core`: clean architecture core and plugin contracts.
- `src/Infrastructure`: database, workflow, cache, logging, and external integrations.
- `src/Platform`: organization, identity, access control, registry, audit, and configuration.
- `src/Modules`: plugin modules.
- `database`: SQL Server and workflow persistence scripts.
- `contracts`: OpenAPI, AsyncAPI, protobuf, event, and schema contracts.
- `services`: future FastAPI, Node.js, and Java services.
- `.ai`: agent handover context and progress tracking.

## How To Run

```powershell
docker compose up -d sqlserver redis
dotnet run --project src/Apps/SaptariX.Admin.Mvc/SaptariX.Admin.Mvc.csproj
```

Run SQL scripts under `database/sqlserver` in numeric order before using stored-procedure-backed data.

This machine only had the ASP.NET Core 10 shared runtime available during verification, so the local dev server was started with `DOTNET_ROLL_FORWARD=Major` while keeping project targets at `net8.0`.

## Coolify / GCP Deployment

The root `docker-compose.yml` now defines a Coolify-ready stack with `admin-mvc`, `sqlserver`, and `redis`.

- `admin-mvc` builds from `deploy/docker/admin-mvc.Dockerfile` and listens on container port `8080`.
- SQL Server and Redis are private Compose services with no host port mappings.
- `saptarix_app_data` persists ASP.NET Core DataProtection keys.
- `saptarix_sqlserver` and `saptarix_redis` persist infrastructure state.
- `SAPTARIX_SQL_PASSWORD` is required in Coolify before deployment.
- Assign the Coolify domain to `admin-mvc` using internal port `8080`.

See `docs/deployment-coolify-gcp.md` for the deployment checklist.

## AdminLTE Integration

AdminLTE v4 was fetched from the official ColorlibHQ/AdminLTE repository into `.tmp/AdminLTE`. Only compiled runtime assets were copied to `src/Apps/SaptariX.Admin.Mvc/wwwroot/vendor/adminlte`. Demo pages, source files, and dependencies are not part of the final source.

## SQL Server And Dapper

`SaptariX.Persistence.SqlServer` uses `Microsoft.Data.SqlClient` and Dapper. Application code depends on `IDbConnectionFactory`, `IDapperRepository`, `IStoredProcedureExecutor`, and `ITransactionManager`.

## Organization Module UI Shell

`/Organizations` now renders the Organization foundation UI shell in Admin MVC. This pass is intentionally UI-only: it does not add Organization repositories, stored procedures, or server-side create/update/delete endpoints.

The page uses the List + Create/Update panel pattern. `Organization List` is the default panel. `Create Organization` hosts the reusable form. Clicking the edit action fills the same form with preview data, switches the heading/button text to `Update Organization`, and shows the back-to-list control.

The list tab uses the existing enterprise table language with top search/filter controls, loading/empty states, pagination, compact action buttons, and sort-ready headers. Primary action buttons use the UI Kit `Advanced Buttons 5` elevated family (`ui-adv-btn ui-adv-btn-elevated`).

Organization secondary configuration is represented with right drawers:

- Apps & Products Mapping
- Organization Module Access
- Organization Domains
- Organization Settings

The delete action uses a Bootstrap confirmation modal and only performs a client-side preview state change. The system Organization preview row is protected from delete in the UI. Future backend work should add stored-procedure-backed services and enforce the same rules server-side.

## Users Module

The Users module is implemented in Admin MVC with thin controller actions in `UsersController`. Business behavior is in `SaptariX.Platform.Identity.Users` services and repository interfaces. SQL Server/Dapper implementations live in `SaptariX.Persistence.SqlServer` as `UserRepository` and `UserPermissionRepository`.

The `/Users` page reuses the Organization module page shell. The header action opens the form panel, the back arrow returns to the list, and the list uses the shared Organization filter/table/button classes. Clicking the edit action fetches `/Users/GetById/{id}`, fills the same form, hides password fields, and changes the submit button to `Update User`. `Cancel Edit` clears the form and returns to the list.

The delete action opens the Bootstrap modal with the required confirmation text. Confirming calls `/Users/SoftDelete/{id}` and only marks `IsDeleted`, `DeletedAtUtc`, and `DeletedBy`; no hard-delete path exists.

The permissions action opens the right-side Bootstrap offcanvas drawer. It loads `/Users/GetPermissions/{userId}`, renders module-wise collapsible permission groups, supports drawer search and Select All per group, then posts selected direct overrides to `/Users/SavePermissions`.

Users stored procedure scripts are under `database/sqlserver/003-identity`:

- `[identity].[sp_User_List]`
- `[identity].[sp_User_GetById]`
- `[identity].[sp_User_Create]`
- `[identity].[sp_User_Update]`
- `[identity].[sp_User_SoftDelete]`
- `[identity].[sp_UserPermission_GetByUserId]`
- `[identity].[sp_UserPermission_Save]`

The repositories call these stored procedures first. When SQL Server scripts have not been applied yet, they use a local fallback store so the Admin UI remains verifiable during scaffold development.

## Access Control Module

Access Control is intentionally lightweight in this phase. It provides simple in-memory role CRUD, permission CRUD, user-role assignment, and role-permission assignment for the Admin MVC workflow. It does not enforce runtime RBAC, does not gate sidebar visibility, and does not require AccessControl SQL scripts.

Runtime service contracts live in `SaptariX.Platform.AccessControl`: `IRoleService`, `IPermissionService`, `IUserRoleService`, and `IPermissionAuthorizationService`. `PermissionAuthorizationService` currently allows access so the platform remains usable while product flow is being shaped.

Admin MVC routes cover `/Users`, `/Roles`, `/Permissions`, `/Users/GetRoles/{userId}`, `/Users/SaveRoles`, `/Users/GetPermissions/{userId}`, and `/Users/SavePermissions`. `/Users`, `/Roles`, and `/Permissions` are direct sidebar menu entries and reuse the Organization module page shell, filters, table wrappers, action buttons, and pagination styling.

## UI Kit / Design System

`src/Modules/UIKit` is the internal Admin MVC UI Kit module. It is intended for SuperAdmin and Developer users, not normal end users. The module contributes the `Developer.UIComponents.View` permission and the sidebar hierarchy `Developer Tools -> UI Components`.

UI Kit routes:

- `/UIComponents`
- `/UIComponents/FormControls`
- `/UIComponents/Tables`
- `/UIComponents/TablePatterns`
- `/UIComponents/InlineCreateTables`
- `/UIComponents/InputTables`
- `/UIComponents/FilterToolbars`
- `/UIComponents/Modals`
- `/UIComponents/Drawers`
- `/UIComponents/Cards`
- `/UIComponents/Buttons`
- `/UIComponents/IconLibrary`
- `/UIComponents/TabsAccordions`
- `/UIComponents/AlertsToasts`
- `/UIComponents/Loaders`
- `/UIComponents/EmptyStates`
- `/UIComponents/LayoutExamples`

Reusable partials live in `src/Apps/SaptariX.Admin.Mvc/Views/Shared/UIComponents`. They cover page headers, preview/code snippets, cards, form controls, tables, enterprise table patterns, filter toolbars, inline-create tables, input tables, action buttons, confirmation modals, right drawers, toast host, spinners, empty states, and skeleton loaders.

Global UI customization belongs in `wwwroot/scss/saptarix-ui.scss` and its partials under `wwwroot/scss/abstracts`, `components`, and `utilities`. The compiled runtime file is `wwwroot/css/saptarix-ui.css`, loaded globally by `_Styles.cshtml` before `ui-kit.css`. Use reusable `sx-*` classes and CSS variables for spacing, radius, colors, table density, drawer widths, card padding, and button heights instead of page-specific style blocks.

`wwwroot/js/modules/ui-kit.js` provides `window.SaptariXToast.success/error/warning/info`, copy-snippet behavior, preview/code toggles, loading button demos, input-table add/remove row behavior, and inline-create table demo behavior. `wwwroot/css/ui-kit.css` contains only UI Kit-specific spacing, drawer width, skeleton, input-table, sidebar nesting, and responsive polish.

Icon strategy:

- Use Font Awesome Free for platform icons.
- Global runtime assets live under `wwwroot/vendor/fontawesome-free` and are loaded from `Views/Shared/_Styles.cshtml`.
- Only solid and regular styles are loaded by default; do not add brands unless a page actually needs brand icons.
- Use `<i class="fa-solid ...">` or `<i class="fa-regular ...">` directly in Razor and menu definitions.
- Do not reintroduce the custom SVG sprite, generated icon registry, or custom icon library assets.
- `UI Components -> Icon Library` is a lightweight Font Awesome usage reference, not a generated SaptariX icon library.

Table patterns:

- Use `_UiEnterpriseTable` for primary list pages that need a top search/filter toolbar, responsive table wrapper, loading state, empty state, pagination footer, sort-ready headers, and compact row actions.
- Use `_UiFilterToolbar` for search, status, type, date range, organization/app scope, reset, apply, and optional create actions.
- Use `_UiInlineCreateTable` when a small master entity can be created and updated in the compact section above the table.

Future modules should start from these patterns:

- Use the advanced table pattern for Users, Roles, Organizations, Forms, and Reports.
- Use the enterprise table pattern for dense list pages with always-visible filters.
- Use the inline create table pattern for small master modules.
- Use the right drawer pattern for permission management and details panels.
- Use the input-table pattern for DynamicForms/AppBuilder metadata editing.
- Use the Organization-style List/Create/Update panel pattern for CRUD modules that do not need separate edit pages.

Entity UI pattern decision:

- Use Inline Create + Table when the entity is small, has two to five fields, and has no complex child data, permission matrix, workflow designer, or heavy validation.
- Use List + Create/Update Panels when the entity has a medium form and list/form workflows both matter, such as Users, Roles, and Organizations.
- Use Right Drawer when the user edits secondary configuration, such as user permissions, role permissions, filters, and settings.
- Use Modal when the action is short and temporary, such as confirm delete or quick status change.
- Use Dedicated Page when the entity has complex builder/configuration behavior, such as DynamicForms, Workflow Designer, and Report Builder.

## Elsa Workflow

`SaptariX.Elsa` contains workflow runtime options, SQL Server persistence configuration flags, activity registration, and sample activities. The boundary is ready for replacing the placeholder runtime with concrete Elsa package calls while preserving module contracts.

## Modules

`DynamicForms` is the sample module. It includes contracts, application service, infrastructure repository, Admin MVC controller/view, menu provider, permission provider, workflow activity provider, SQL scripts, `module.json`, and README.

## Add A New Module

1. Create the four module projects.
2. Add a `module.json` manifest.
3. Implement `IPlatformModule`.
4. Register services, menu entries, permissions, and workflow activities.
5. Add SQL scripts under the module database folder.
6. Register the module in the app composition root.

## Known Pending Tasks

- Replace placeholder Elsa runtime implementation with concrete Elsa package configuration.
- Add production-grade identity persistence and JWT token issuance.
- Add real Redis implementation behind `ICacheClient`.
- Run and verify new Users stored procedures against a local SQL Server container.
- Add browser automation coverage for UI Kit modal, drawer, toast, and input-table demos.
- Add integration tests after database scripts are applied.

## Developer Notes

Keep Organization naming everywhere. Do not introduce Tenant or Tenancy names.

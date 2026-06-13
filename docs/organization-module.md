# Organization Module

## Scope

The current Organization module work is a UI-only shell. It establishes the Admin MVC page structure, table layout, form layout, drawer layout, modal pattern, permissions names, and client-side preview behavior. It does not yet include SQL Server tables, stored procedures, Dapper repositories, server-side services, or AJAX persistence endpoints.

## UI Route

- `/Organizations`

The page uses the existing AdminLTE layout and UI Kit patterns:

- Organization List tab as the default view.
- Create/Update Organization tab using the same form for create and edit.
- Enterprise table with top search/filter toolbar.
- Right drawers for secondary configuration.
- Bootstrap confirmation modal for soft-delete preview.
- UI Kit `Advanced Buttons 5` elevated button classes for primary actions.

## List Behavior

The Organization List tab contains static preview rows. Search and filters run client-side against row data:

- Search by name, code, email, or domain.
- Status filter.
- Organization Type filter.
- Plan filter.
- Industry filter.
- Created date range placeholder.

Actions:

- View shows a preview alert.
- Edit fills the Create/Update form and switches tabs.
- Apps/Products opens the mapping drawer.
- Modules opens the module access drawer.
- Domains opens the domains drawer.
- Settings opens the settings drawer.
- Delete opens the confirmation modal unless the row is marked as the system Organization.

## Form Behavior

The form includes these sections:

- Basic Details.
- Contact Details.
- Address Details.
- Business Details.
- Branding.
- Subscription / Plan Placeholder.

Required fields are enforced with browser validation for the UI preview:

- Organization Name.
- Organization Code.
- Slug.
- Primary Email.
- Organization Type.
- Status.

Defaults:

- Currency: `INR`.
- Timezone: `Asia/Kolkata`.

## Drawers

Implemented UI drawers:

- Apps & Products Mapping.
- Organization Module Access.
- Organization Domains.
- Organization Settings.

Drawer save buttons only show UI preview feedback. No backend save is wired yet.

## Permissions

Permission constants now use Organization naming:

- `Organizations.View`
- `Organizations.Create`
- `Organizations.Update`
- `Organizations.Delete`
- `Organizations.ManageApps`
- `Organizations.ManageModules`
- `Organizations.ManageDomains`
- `Organizations.ManageSettings`

## Future Backend Work

Add SQL Server scripts under `database/sqlserver/002-organization`, Dapper repositories under `SaptariX.Persistence.SqlServer`, platform services under `SaptariX.Platform.Organization`, and thin Admin MVC endpoints behind the current UI.

Stored procedure targets:

- `org.sp_Organization_List`
- `org.sp_Organization_GetById`
- `org.sp_Organization_Create`
- `org.sp_Organization_Update`
- `org.sp_Organization_SoftDelete`
- `org.sp_Organization_ToggleStatus`
- `org.sp_Organization_GetAppsProducts`
- `org.sp_Organization_SaveAppsProducts`
- `org.sp_Organization_GetModules`
- `org.sp_Organization_SaveModules`
- `org.sp_Organization_GetDomains`
- `org.sp_Organization_SaveDomain`
- `org.sp_Organization_DeleteDomain`
- `org.sp_Organization_GetSettings`
- `org.sp_Organization_SaveSettings`

Server-side rules to enforce:

- Never hard-delete Organizations.
- Protect system Organizations from normal update/delete paths.
- Do not allow deleting the last active Organization.
- Keep `Organization` naming everywhere; do not introduce Tenant or Tenancy naming.

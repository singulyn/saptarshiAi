# Decisions

- SQL Server selected as primary DB.
- Organization naming selected instead of Tenancy.
- Modular monolith selected instead of premature microservices.
- AdminLTE v4 selected as admin UI.
- Dapper and stored procedures selected for app data.
- Elsa selected for workflow.
- Redis abstraction added with in-memory development implementation.
- Organization is the SaaS ownership boundary for apps, products, modules, domains, settings, and future user mapping.
- Current Organization work is intentionally UI-only; backend persistence should be implemented as a separate pass behind the existing UI shell.
- Organization UI actions use the UI Kit `Advanced Buttons 5` elevated button family for primary drawer, form, filter, and modal actions.

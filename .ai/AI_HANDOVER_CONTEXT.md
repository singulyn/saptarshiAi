# AI Handover Context

SaptariX Platform is a new ASP.NET Core MVC modular SaaS scaffold. Use Organization naming everywhere. Do not introduce Tenant or Tenancy naming.

Architecture decisions:

- SQL Server is primary database.
- Dapper and stored procedures are the application data pattern.
- Modular monolith is selected before microservice extraction.
- AdminLTE v4 is the admin UI shell.
- Elsa is selected for workflow through an integration boundary.
- Redis is represented by an abstraction with an in-memory development implementation.

Do:

- Keep controllers thin.
- Put business logic in services.
- Keep infrastructure out of Domain.
- Add SQL under `database/sqlserver` or module database folders.

Do not:

- Put SQL in utility classes.
- Couple modules to other module implementations.
- Add external services that read SQL Server tables directly.

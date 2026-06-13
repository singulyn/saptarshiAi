# SaptariX Platform

SaptariX Platform is a plugin-oriented modular SaaS platform built with ASP.NET Core MVC, Razor Views, AdminLTE v4, SQL Server, Dapper, stored procedures, workflow integration boundaries, and organization-based SaaS modeling.

## Architecture

The solution is a modular monolith with microservice-ready boundaries. Core contracts live under `src/Core`, infrastructure implementations under `src/Infrastructure`, platform capabilities under `src/Platform`, and feature modules under `src/Modules`.

Organization naming is mandatory. Do not introduce Tenant or Tenancy naming.

## Tech Stack

- .NET 8 target framework
- ASP.NET Core MVC and Web API
- Razor Views
- AdminLTE v4 and Bootstrap 5
- SQL Server
- Dapper with stored procedures
- Serilog
- Elsa-ready workflow infrastructure
- Redis-ready cache abstraction
- Future FastAPI, Node.js, and Java service boundaries

## Run Locally

1. Start infrastructure:

```powershell
docker compose up -d sqlserver redis
```

2. Run database scripts from `database/sqlserver` in numeric order using SSMS or `sqlcmd`.

3. Start Admin MVC:

```powershell
dotnet run --project src/Apps/SaptariX.Admin.Mvc/SaptariX.Admin.Mvc.csproj
```

If the machine has the .NET 8 base runtime but not the ASP.NET Core 8 shared runtime, set `DOTNET_ROLL_FORWARD=Major` for local testing or install the ASP.NET Core 8 Hosting Bundle/runtime.

4. Open the Admin MVC URL printed by `dotnet run`. The default route is `/Dashboard`.

## Deploy With Coolify On GCP VM

The root `docker-compose.yml` is prepared for Coolify Docker Compose deployment. It builds `SaptariX.Admin.Mvc`, keeps SQL Server and Redis private inside the Compose network, persists DataProtection keys and database data in named volumes, and exposes the Admin MVC app internally on port `8080`.

In Coolify, create a Docker Compose resource from this Git repository, set `SAPTARIX_SQL_PASSWORD`, assign the domain to the `admin-mvc` service using container port `8080`, then deploy.

Full notes are in `docs/deployment-coolify-gcp.md`.

## SQL Server

The Admin MVC app reads the `SaptariX` connection string from `appsettings.json`. SQL scripts are SSMS-compatible and organized by bounded area:

- `001-core`
- `002-organization`
- `003-identity`
- `007-audit`

Application data access goes through `SaptariX.Persistence.Abstractions` and `SaptariX.Persistence.SqlServer`; SQL queries belong in scripts, not utility classes.

## Modules

Modules follow this structure:

```text
Modules/{ModuleName}/
  SaptariX.Modules.{ModuleName}.Application/
  SaptariX.Modules.{ModuleName}.Infrastructure/
  SaptariX.Modules.{ModuleName}.Web/
  SaptariX.Modules.{ModuleName}.Contracts/
  database/
  workflows/
  module.json
  README.md
```

`DynamicForms` is the implemented sample module. It registers services, menu entries, permissions, a workflow activity placeholder, stored procedure scripts, and an Admin MVC page.

To add a module, implement `IPlatformModule`, register module services through `AddServices`, add menu and permission providers, and add database scripts under the module folder. Enable or disable modules through module manifests and the platform module registry.

## External Services

Future FastAPI, Node.js, and Java services live under `services`. They must communicate through REST, gRPC, or message contracts in `/contracts` and must not directly access SQL Server tables.

## Development Rules

- Keep controllers thin.
- Keep business logic in services.
- Use Organization naming everywhere.
- Use Dapper and stored procedures for application data.
- Keep infrastructure concerns out of Domain and Application.
- Keep module dependencies on abstractions and contracts.
- Do not commit `bin`, `obj`, `.vs`, `node_modules`, `.tmp`, logs, local settings, or secrets.

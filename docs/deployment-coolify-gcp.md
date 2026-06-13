# Deploy SaptariX Platform On GCP VM With Coolify

This repository is ready for Coolify Docker Compose deployment from the root `docker-compose.yml`.

## Coolify Resource

On the GCP VM firewall, expose only the ports Coolify/proxy needs publicly, normally `80` and `443`. Do not expose SQL Server `1433` or Redis `6379` to the internet.

Use this GitHub repository as the source:

```text
https://github.com/singulyn/saptarshiAi.git
```

Create a Docker Compose application/service stack in Coolify from that GitHub repository and keep the compose path as:

```text
docker-compose.yml
```

Coolify should build the `admin-mvc` service from:

```text
deploy/docker/admin-mvc.Dockerfile
```

The compose build context defaults to:

```text
https://github.com/singulyn/saptarshiAi.git#main
```

For local development, override `SAPTARIX_BUILD_CONTEXT=.` before running Compose.

## Required Environment Variables

Set these variables in Coolify before deploying:

```text
SAPTARIX_SQL_PASSWORD=<strong SQL Server sa password>
SAPTARIX_SQL_DATABASE=SaptariX_Platform
MSSQL_PID=Express
SAPTARIX_BUILD_CONTEXT=https://github.com/singulyn/saptarshiAi.git#main
```

Use a strong `SAPTARIX_SQL_PASSWORD`. For licensed SQL Server deployments, change `MSSQL_PID` to the licensed edition. `Express` is the default to avoid accidentally deploying an unlicensed paid edition.

## Domain Routing

Assign the public domain to the `admin-mvc` service.

The container listens on internal port `8080`, so the Coolify domain target must use port `8080`, for example:

```text
https://admin.example.com:8080
```

Coolify/Traefik terminates public HTTP/HTTPS traffic and forwards it to the container. SQL Server and Redis intentionally do not publish host ports; they are private services inside the Compose network.

## Persistent Storage

The compose file defines these volumes:

- `saptarix_app_data` for ASP.NET Core DataProtection keys.
- `saptarix_sqlserver` for SQL Server data.
- `saptarix_redis` for Redis append-only data.

Do not delete these volumes unless you are intentionally resetting the deployment.

## Database Setup

After the first SQL Server start, apply SQL scripts under `database/sqlserver` in numeric order.

The Admin MVC app can start before scripts are applied, but stored-procedure-backed features require those scripts before production use.

## Health Check

The Admin MVC service exposes:

```text
/health
```

The Docker image and compose service both use this endpoint for container health.

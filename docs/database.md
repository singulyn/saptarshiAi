# Database

SQL Server is the primary database. Scripts are SSMS-compatible and live under `database/sqlserver`.

Application data access uses Dapper and stored procedures through persistence abstractions. Core CRUD operations should be implemented as stored procedures. Organization-scoped reads and writes must filter by `OrganizationId`.

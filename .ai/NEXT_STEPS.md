# Next Steps

Immediate tasks:

1. Apply SQL Server scripts in numeric order.
2. Implement the Organization backend behind the new UI shell: SQL scripts, stored procedures, Dapper repositories, services, controller endpoints, anti-forgery AJAX saves, and persistence validation.
3. In Coolify, set `SAPTARIX_SQL_PASSWORD`, assign the `admin-mvc` service domain to internal port `8080`, and apply SQL scripts after the first SQL Server start.
4. Validate the Users module against SQL Server-backed stored procedures.
5. Use the UI Kit enterprise table pattern in the Users module when polishing the next pass.
6. Use Inline Create + Table for small master modules.
7. Use List/Create tabs for Users and other medium CRUD modules.
8. Use the UI Kit drawer pattern for permission assignment and secondary configuration.
9. Use the same global `sx-*` classes across all future modules.
10. Use Font Awesome Free icons directly in Razor/menu definitions; do not reintroduce custom sprite assets.
11. Use the UI Kit input-table pattern in DynamicForms/AppBuilder module work.
12. Implement the Roles module next, including role create/update, role permissions, and assignment to users.
13. Test `/DynamicForms` against SQL Server-backed stored procedures.
14. Replace workflow placeholder runtime with concrete Elsa package configuration.

Next coding phase prompt:

Implement the Organization backend for the existing UI shell with SQL Server stored procedures, Dapper repositories, thin MVC AJAX endpoints, soft-delete protection for system organizations, and docs/status updates.

Testing checklist:

- Build solution.
- Start SQL Server and Redis with Docker Compose.
- Run database scripts.
- Start Admin MVC.
- Verify dashboard, login, organization menu, module menu, Users page, DynamicForms page, and static AdminLTE assets.
- Verify Users create, update, soft delete, and permission drawer flows against SQL Server.

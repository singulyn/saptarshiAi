# Module Development

Modules must depend on core abstractions and contracts, not another module implementation. Each module owns its application services, infrastructure repository, web UI, SQL scripts, workflow activities, manifest, and README.

Register a module by implementing `IPlatformModule` and adding it in the app composition root.

## Entity UI Pattern Decision

Use Inline Create + Table when the entity is small, the form has two to five fields, and there is no complex child data, permission matrix, workflow designer, or heavy validation. Examples include role category, permission category, status master, product type, payment method, country/state/city small masters, tags, department, and designation.

Use List + Create/Update Tabs when the entity has a medium form, create and update need more space, and the table and form both matter. Examples include Users, Roles, and Organizations.

Use Right Drawer when the user edits secondary configuration such as user permissions, role permissions, filters, or settings.

Use Modal when the action is short and temporary, such as confirm delete or quick status change.

Use Dedicated Page when the entity has complex builder or configuration behavior, such as DynamicForms, Workflow Designer, or Report Builder.

Future Admin MVC modules should reuse the global `sx-*` classes from `wwwroot/css/saptarix-ui.css` and the UI Kit partials under `Views/Shared/UIComponents` before adding page-specific CSS.

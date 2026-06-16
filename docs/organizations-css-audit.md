# Organizations CSS Audit

Date: 2026-06-16

Scope: Static audit of the ASP.NET Core MVC frontend assets currently loaded by `Views/Shared/_Styles.cshtml`, with focus on the Organizations module CSS inside `wwwroot/css/site.css`.

## Loaded CSS Size

| File | Raw bytes | Gzip bytes |
| --- | ---: | ---: |
| `wwwroot/vendor/adminlte/css/adminlte.min.css` | 340,131 | 46,642 |
| `wwwroot/lib/bootstrap/dist/css/bootstrap.min.css` | 232,803 | 31,118 |
| `wwwroot/css/site.css` | 95,731 | 12,874 |
| `wwwroot/vendor/fontawesome-free/css/fontawesome.min.css` | 57,644 | 15,676 |
| `wwwroot/css/ui-kit.css` | 24,848 | 5,019 |
| `wwwroot/css/saptarix-ui.css` | 18,332 | 3,522 |
| `wwwroot/vendor/fontawesome-free/css/regular.min.css` | 625 | 330 |
| `wwwroot/vendor/fontawesome-free/css/solid.min.css` | 619 | 329 |
| **Total** | **770,733** | **115,510** |

## Organizations CSS Footprint

Static selector scan found 509 Organizations-related rule blocks in `site.css`.

| Metric | Value |
| --- | ---: |
| `site.css` raw size | 95,731 bytes |
| Organizations-related raw CSS | 71,636 bytes |
| Organizations-related gzip estimate | 9,125 bytes |
| Organizations share of `site.css` raw size | 74.3% |

This means most current Organizations UI work lives inside the globally loaded `site.css`. That affects every page, not only Organizations.

## Static Selector Usage

The audit extracted organization-prefixed classes from `site.css` and searched Razor and JS files under:

- `src/Apps/SaptariX.Admin.Mvc/Views`
- `src/Apps/SaptariX.Admin.Mvc/wwwroot/js`

| Metric | Count |
| --- | ---: |
| Organization-prefixed classes in CSS | 110 |
| Referenced in Razor or JS | 106 |
| Not referenced in Razor or JS | 4 |

## Definite Cleanup Candidates

These classes were found only in CSS, with no Razor or JS reference:

| Class | Rule count | Raw bytes |
| --- | ---: | ---: |
| `organization-check-grid` | 1 | 157 |
| `organization-drawer-wide` | 3 | 660 |
| `organizations-card-header` | 2 | 184 |
| `organization-setting-grid` | 3 | 510 |
| **Total** | **9** | **1,520** |

Estimated gzip saving from removing only these definite-unused rules: about 473 bytes.

These are safe cleanup candidates from static analysis, but they should still be removed in a normal code review because CSS can sometimes be referenced by future partials or generated markup.

## Performance Impact Estimate

Removing only the definite-unused rules will not materially improve performance. It saves less than 0.5 KB gzip, which is mostly code hygiene.

The real performance improvement comes from splitting Organizations CSS out of global `site.css`:

- Non-Organizations pages could avoid about 71.6 KB raw CSS.
- Estimated transfer saving is about 9.1 KB gzip.
- Current total loaded CSS is about 115.5 KB gzip, so this is roughly 8% of loaded CSS transfer on pages that do not need Organizations UI.
- Desktop impact will likely be small.
- Slower mobile networks may see a modest improvement because CSS is render-blocking. Expect small but real gains in CSS download, parse, and style calculation. The visible improvement depends on network, caching, and device speed.

On the Organizations page itself, splitting CSS does not reduce page cost unless unused rules inside the Organizations module are also removed. It mainly improves other pages by not loading this module-specific CSS globally.

## Recommended Path

1. Remove the four definite-unused selector groups.
2. Move Organizations-specific CSS from `site.css` into `wwwroot/css/organizations.css`.
3. Load `organizations.css` only on `Views/Organizations/Index.cshtml` or through a page-specific style section.
4. Keep shared primitives in `site.css`, `saptarix-ui.css`, and `ui-kit.css`.
5. Run a browser Coverage pass only after all Organizations states are opened:
   - list table
   - create wizard steps
   - validation errors
   - custom selects/dropdowns
   - apps/products drawer
   - modules drawer
   - domains drawer
   - settings drawer
   - mobile viewport

Do not auto-delete CSS based only on browser Coverage, because unopened drawers and responsive states will be reported as unused.

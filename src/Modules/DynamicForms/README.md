# DynamicForms Module

DynamicForms is the first sample plugin module for SaptariX Platform. It demonstrates the expected module split:

- Contracts for public DTOs.
- Application for use cases and service interfaces.
- Infrastructure for stored-procedure-backed persistence.
- Web for Admin MVC controller, views, menu, permissions, and workflow activity registration.

The module does not depend on another module implementation. It depends on platform abstractions and can later be extracted behind an API boundary.

# Elsa Workflow

`SaptariX.Elsa` defines the workflow integration boundary. It has options for SQL Server persistence, runtime, and management. Modules register activity definitions through `IWorkflowActivityProvider`.

Sample workflow concept:

Form Submitted -> Approval Required -> Notification Sent -> Audit Logged

Workflows can be attached to Organization, App, or Module records by storing those references in workflow metadata when concrete Elsa persistence is enabled.

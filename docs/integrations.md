# Integrations

Future external services live under `services`.

FastAPI will connect through REST or gRPC contracts in `/contracts/openapi` or `/contracts/protobuf`.

Node.js will connect through REST, WebSocket gateway contracts, or AsyncAPI event contracts.

Java adapters will connect through REST, gRPC, or message contracts for enterprise system integration.

External services must not directly access SQL Server tables. Direct database coupling would bypass authorization, organization scoping, audit, versioned contracts, and workflow policies.

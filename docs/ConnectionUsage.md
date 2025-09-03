# Database Connection Usage

The Unity client uses direct MySQL access through the `MySqlConnector` library.
Connection settings are managed by `DatabaseConfigUnity`, which exposes debug and Kim server toggles
for switching between local, Kim, and production servers.

All database queries flow through `DatabaseClientUnity`. This wrapper centralizes connection retries
and parameter handling so services like `CharacterDatabase` and `ChatService` share the same access layer.

## Adding Queries

Store each SQL statement in its own `.sql` file at the repository root. Load the file contents and pass
it to `DatabaseClientUnity` rather than embedding raw SQL strings in code.

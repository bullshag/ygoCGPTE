# Database Connection Usage

The Unity client uses direct MySQL access through the `MySqlConnector` library. Connection settings, including database hosts and API endpoints, are stored in a `ServerConfig` ScriptableObject located in the `Assets/Resources` folder. Different builds can provide environment-specific copies of this asset.

Database credentials are supplied at runtime via the `DB_USERNAME` and `DB_PASSWORD` environment variables. This keeps secrets out of source control while allowing secure configuration in deployment environments.

All database queries flow through `DatabaseClientUnity`. This wrapper centralizes connection retries and parameter handling so services like `CharacterDatabase` and `ChatService` share the same access layer.

## Adding Queries

Store each SQL statement in its own `.sql` file at the repository root. Load the file contents and pass
it to `DatabaseClientUnity` rather than embedding raw SQL strings in code.

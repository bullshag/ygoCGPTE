using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;

using WinFormsApp2;

public static class DatabaseClientUnity
{
    private const int MaxRetries = 3;

    private static async Task<MySqlConnection> OpenConnectionAsync()
    {
        int attempt = 0;
        while (true)
        {
            try
            {
                var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
                await conn.OpenAsync();
                return conn;
            }
            catch (MySqlException) when (attempt < MaxRetries)
            {
                await Task.Delay(200 * (int)Math.Pow(2, attempt));
                attempt++;
            }
        }
    }

    public static async Task<List<Dictionary<string, object?>>> QueryAsync(string sql, Dictionary<string, object?>? parameters = null)
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new MySqlCommand(sql, conn);
        AddParameters(cmd, parameters);
        var results = new List<Dictionary<string, object?>>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
            }
            results.Add(row);
        }
        return results;
    }

    public static async Task<int> ExecuteAsync(string sql, Dictionary<string, object?>? parameters = null)
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new MySqlCommand(sql, conn);
        AddParameters(cmd, parameters);
        return await cmd.ExecuteNonQueryAsync();
    }

    private static void AddParameters(MySqlCommand cmd, Dictionary<string, object?>? parameters)
    {
        if (parameters == null) return;
        foreach (var kvp in parameters)
        {
            cmd.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);

        }
    }
}

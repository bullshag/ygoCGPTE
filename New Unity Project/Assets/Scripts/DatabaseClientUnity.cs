using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using UnityClient;
using UnityEngine;

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
                var conn = new MySqlConnection(DatabaseConfigUnity.ConnectionString);
                await conn.OpenAsync();
                Debug.Log($"Database connection established on attempt {attempt + 1}");
                return conn;
            }
            catch (MySql.Data.MySqlClient.MySqlException) when (attempt < MaxRetries)
            {
                await Task.Delay(200 * (int)Math.Pow(2, attempt));
                attempt++;
            }
        }
    }

    public static async Task<List<Dictionary<string, object?>>> QueryAsync(string sql, Dictionary<string, object?>? parameters = null)
    {
        using var conn = await OpenConnectionAsync();
        Debug.Log($"Executing SQL query: {sql}");
        using var cmd = new MySqlCommand(sql, conn);
        AddParameters(cmd, parameters);
        var results = new List<Dictionary<string, object?>>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
            }
            results.Add(row);
        }
        Debug.Log($"Query returned {results.Count} rows.");
        return results;
    }

    public static async Task<int> ExecuteAsync(string sql, Dictionary<string, object?>? parameters = null)
    {
        using var conn = await OpenConnectionAsync();
        Debug.Log($"Executing SQL command: {sql}");
        using var cmd = new MySqlCommand(sql, conn);
        AddParameters(cmd, parameters);
        var rows = await cmd.ExecuteNonQueryAsync();
        Debug.Log($"Command affected {rows} rows.");
        return rows;
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

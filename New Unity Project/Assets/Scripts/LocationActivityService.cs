using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Loads location activity availability from the database.
/// </summary>
public sealed class LocationActivityService
{
    private const string QueryFileName = "unity_location_activity_settings.sql";
    private readonly string _queryPath;

    public LocationActivityService()
    {
        _queryPath = Path.Combine(Application.dataPath, "sql", QueryFileName);
    }

    /// <summary>
    /// Retrieve activity availability for the provided location.
    /// </summary>
    public async Task<LocationActivityAvailability> GetAvailabilityAsync(string locationId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(locationId))
        {
            throw new ArgumentException("Location identifier must be provided.", nameof(locationId));
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (!File.Exists(_queryPath))
        {
            throw new FileNotFoundException($"Location activity query not found at {_queryPath}.");
        }

        string sql = await ReadQueryAsync(cancellationToken).ConfigureAwait(false);

        var rows = await DatabaseClientUnity.QueryAsync(
            sql,
            new Dictionary<string, object?> { ["@location_id"] = locationId });

        cancellationToken.ThrowIfCancellationRequested();

        var availability = new LocationActivityAvailability();
        availability.ResetAll(false);

        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!row.TryGetValue("activity_key", out var keyObj) || keyObj is null)
            {
                continue;
            }

            string key = Convert.ToString(keyObj) ?? string.Empty;
            if (!LocationActivityTypeExtensions.TryParseDatabaseKey(key, out var activityType))
            {
                Debug.LogWarning($"Unknown activity key '{key}' encountered for location '{locationId}'.");
                continue;
            }

            bool isEnabled = false;
            if (row.TryGetValue("is_enabled", out var enabledObj) && enabledObj != null)
            {
                isEnabled = Convert.ToInt32(enabledObj) != 0;
            }

            availability.Set(activityType, isEnabled);
        }

        return availability;
    }

    private async Task<string> ReadQueryAsync(CancellationToken cancellationToken)
    {
        using var stream = new FileStream(_queryPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(stream);
        var builder = new System.Text.StringBuilder();
        var buffer = new char[1024];
        int read;
        while ((read = await reader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            builder.Append(buffer, 0, read);
        }

        return builder.ToString();
    }
}

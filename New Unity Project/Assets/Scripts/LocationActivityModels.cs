using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Enumerates supported location activity buttons.
/// </summary>
public enum LocationActivityType
{
    Tavern,
    Shop,
    Temple,
    Academy,
    Graveyard,
    Arena,
    SearchForEnemies
}

/// <summary>
/// Helper extensions for mapping activity types to database keys.
/// </summary>
public static class LocationActivityTypeExtensions
{
    private static readonly IReadOnlyDictionary<LocationActivityType, string> ToKeyMap =
        new Dictionary<LocationActivityType, string>
        {
            { LocationActivityType.Tavern, "tavern" },
            { LocationActivityType.Shop, "shop" },
            { LocationActivityType.Temple, "temple" },
            { LocationActivityType.Academy, "academy" },
            { LocationActivityType.Graveyard, "graveyard" },
            { LocationActivityType.Arena, "arena" },
            { LocationActivityType.SearchForEnemies, "search_for_enemies" }
        };

    private static readonly IReadOnlyDictionary<string, LocationActivityType> FromKeyMap =
        ToKeyMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Convert an activity to its database key representation.
    /// </summary>
    public static string ToDatabaseKey(this LocationActivityType type) => ToKeyMap[type];

    /// <summary>
    /// Attempt to parse a database key into an activity type.
    /// </summary>
    public static bool TryParseDatabaseKey(string key, out LocationActivityType type) =>
        FromKeyMap.TryGetValue(key, out type);
}

/// <summary>
/// Tracks which activities are enabled for the active location.
/// </summary>
public sealed class LocationActivityAvailability
{
    private readonly Dictionary<LocationActivityType, bool> _states;

    public LocationActivityAvailability()
    {
        _states = Enum.GetValues(typeof(LocationActivityType))
            .Cast<LocationActivityType>()
            .ToDictionary(t => t, _ => false);
    }

    /// <summary>
    /// Get whether an activity is enabled.
    /// </summary>
    public bool IsEnabled(LocationActivityType type) => _states.TryGetValue(type, out var value) && value;

    /// <summary>
    /// Set whether an activity is enabled.
    /// </summary>
    public void Set(LocationActivityType type, bool enabled) => _states[type] = enabled;

    /// <summary>
    /// Reset all activities to the provided state.
    /// </summary>
    public void ResetAll(bool enabled)
    {
        foreach (var key in _states.Keys.ToArray())
        {
            _states[key] = enabled;
        }
    }

    /// <summary>
    /// Enumerate activity flags.
    /// </summary>
    public IReadOnlyDictionary<LocationActivityType, bool> States => _states;
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Handles recruiting and tavern interactions via direct database calls.
/// Mirrors TavernForm operations like fetching candidates and hiring.
/// </summary>
public class TavernManager : MonoBehaviour
{
    private static readonly TimeSpan RecruitLifetime = TimeSpan.FromHours(24);
    private const int SeedRecruitBatchSize = 6;

    private static readonly (int count, float weight)[] RecruitCountWeights =
    {
        (1, 0.28f),
        (2, 0.24f),
        (3, 0.2f),
        (4, 0.14f),
        (5, 0.09f),
        (6, 0.05f)
    };

    private readonly PartyMemberGenerator _generator = new();
    private bool _hasEnsuredSchema;

    /// <summary>
    /// Fetch recruit candidates for the supplied location node, generating a new roster when the
    /// existing entries are stale or missing.
    /// </summary>
    public async Task<List<PartyMemberGenerator.GeneratedRecruit>> GetCandidatesAsync(int accountId, string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
        {
            Debug.LogWarning("TavernManager cannot fetch candidates without an active node identifier.");
            return new List<PartyMemberGenerator.GeneratedRecruit>();
        }

        await EnsureSchemaAsync();
        await PruneNodeRecruitsAsync(nodeId, forceReset: false);
        var persisted = await LoadPersistedRecruitsAsync(nodeId);

        if (ShouldRegenerateRoster(persisted))
        {
            persisted = await GenerateAndPersistRecruitsAsync(nodeId);
        }

        return persisted
            .Select(ToGeneratedRecruit)
            .ToList();
    }

    /// <summary>
    /// Hire a specific recruit using the persisted node roster.
    /// </summary>
    public async Task<bool> HireAsync(int accountId, int recruitId, string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
        {
            Debug.LogWarning("TavernManager cannot hire without an active node identifier.");
            return false;
        }

        await EnsureSchemaAsync();
        var roster = await LoadPersistedRecruitsAsync(nodeId);
        var candidate = roster.FirstOrDefault(r => r.Recruit.id == recruitId);
        if (candidate == null)
        {
            Debug.LogWarning($"Recruit {recruitId} was not found for node '{nodeId}'.");
            return false;
        }

        int cost = candidate.Cost;
        string purchaseFlowPath = Path.Combine(Application.dataPath, "sql", "unity_tavern_purchase_flow.sql");
        string[] statements = File.ReadAllText(purchaseFlowPath)
            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(statement => statement.Trim())
            .Where(statement => statement.Length > 0)
            .ToArray();

        var parameters = new Dictionary<string, object?>
        {
            ["@userId"] = accountId,
            ["@recruitId"] = recruitId,
            ["@cost"] = cost,
            ["@nodeId"] = nodeId
        };

        if (statements.Length < 3)
        {
            Debug.LogError("unity_tavern_purchase_flow.sql is missing required statements.");
            return false;
        }

        int goldRows = await DatabaseClientUnity.ExecuteAsync(statements[0], parameters);
        if (goldRows <= 0)
        {
            Debug.LogWarning($"Account {accountId} does not have enough gold to hire recruit {recruitId}.");
            return false;
        }

        int hireRows = await DatabaseClientUnity.ExecuteAsync(statements[1], parameters);
        if (hireRows <= 0)
        {
            Debug.LogWarning($"Failed to assign recruit {recruitId} to account {accountId}; refunding gold.");
            string refundSqlPath = Path.Combine(Application.dataPath, "sql", "unity_tavern_refund_gold.sql");
            await DatabaseClientUnity.ExecuteAsync(File.ReadAllText(refundSqlPath), parameters);
            return false;
        }

        int markRows = await DatabaseClientUnity.ExecuteAsync(statements[2], parameters);
        if (markRows <= 0)
        {
            Debug.LogWarning($"Recruit {recruitId} could not be marked as purchased for node '{nodeId}'.");
        }
        else
        {
            string removeSqlPath = Path.Combine(Application.dataPath, "sql", "unity_tavern_remove_recruit.sql");
            await DatabaseClientUnity.ExecuteAsync(File.ReadAllText(removeSqlPath), parameters);
        }

        await PruneNodeRecruitsAsync(nodeId, forceReset: false);
        return true;
    }

    [System.Serializable]
    public class Recruit
    {
        public int id;
        public string name = string.Empty;
        public int level;
    }

    private sealed class PersistedRecruit
    {
        public Recruit Recruit { get; init; } = new Recruit();
        public PartyMemberGenerator.RecruitStats Stats { get; init; } = PartyMemberGenerator.RecruitStats.CreateBaseline();
        public int Cost { get; init; }
        public DateTime CreatedUtc { get; init; }
    }

    private async Task<List<PersistedRecruit>> LoadPersistedRecruitsAsync(string nodeId)
    {
        string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_tavern_select_recruits_by_node.sql");
        var rows = await DatabaseClientUnity.QueryAsync(
            File.ReadAllText(sqlPath),
            new Dictionary<string, object?> { ["@nodeId"] = nodeId });

        var recruits = new List<PersistedRecruit>();
        foreach (var row in rows)
        {
            try
            {
                var recruit = new Recruit
                {
                    id = Convert.ToInt32(row["recruit_id"]),
                    name = Convert.ToString(row["name"]) ?? string.Empty,
                    level = Convert.ToInt32(row["level"])
                };

                var stats = PartyMemberGenerator.RecruitStats.CreateBaseline();
                stats.Strength = Convert.ToInt32(row["strength"]);
                stats.Dexterity = Convert.ToInt32(row["dexterity"]);
                stats.Intelligence = Convert.ToInt32(row["intelligence"]);
                stats.MaxHP = Convert.ToInt32(row["max_hp"]);
                stats.MaxMP = Convert.ToInt32(row["max_mp"]);
                stats.ActionSpeed = Convert.ToSingle(row["action_speed"]);
                stats.PhysicalDefense = Convert.ToInt32(row["physical_defense"]);
                stats.MagicDefense = Convert.ToInt32(row["magic_defense"]);
                stats.RolledPointCount = Convert.ToInt32(row["rolled_points"]);

                DateTime createdUtc = DateTime.SpecifyKind(Convert.ToDateTime(row["created_utc"]), DateTimeKind.Utc);

                recruits.Add(new PersistedRecruit
                {
                    Recruit = recruit,
                    Stats = stats,
                    Cost = Convert.ToInt32(row["cost"]),
                    CreatedUtc = createdUtc
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse tavern recruit row: {ex.Message}");
            }
        }

        Debug.Log($"Loaded {recruits.Count} persisted recruits for node '{nodeId}'.");
        return recruits;
    }

    private bool ShouldRegenerateRoster(List<PersistedRecruit> recruits)
    {
        if (recruits == null || recruits.Count == 0)
        {
            return true;
        }

        DateTime threshold = DateTime.UtcNow - RecruitLifetime;
        bool allExpired = recruits.All(r => r.CreatedUtc <= threshold);
        return allExpired;
    }

    private async Task<List<PersistedRecruit>> GenerateAndPersistRecruitsAsync(string nodeId)
    {
        var baseRecruits = await LoadOrSeedBaseRecruitsAsync();
        if (baseRecruits.Count == 0)
        {
            Debug.LogWarning("No base recruits were available for tavern generation even after seeding.");
            return new List<PersistedRecruit>();
        }

        int count = RollRecruitCount();
        var generated = _generator.BuildCandidates(baseRecruits, count);

        DateTime createdUtc = DateTime.UtcNow;
        var persisted = generated.Select(g => new PersistedRecruit
        {
            Recruit = g.Source,
            Stats = g.Stats.Clone(),
            Cost = g.Cost,
            CreatedUtc = createdUtc
        }).ToList();

        await PruneNodeRecruitsAsync(nodeId, forceReset: true);

        foreach (var recruit in persisted)
        {
            await UpsertRecruitAsync(nodeId, recruit);
        }

        Debug.Log($"Generated {persisted.Count} new recruits for node '{nodeId}'.");
        return persisted;
    }

    private async Task<List<Recruit>> LoadBaseRecruitsAsync()
    {
        string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_tavern_candidates.sql");
        var rows = await DatabaseClientUnity.QueryAsync(File.ReadAllText(sqlPath));

        var list = new List<Recruit>();
        foreach (var row in rows)
        {
            list.Add(new Recruit
            {
                id = Convert.ToInt32(row["id"]),
                name = Convert.ToString(row["name"]) ?? string.Empty,
                level = Convert.ToInt32(row["level"])
            });
        }

        return list;
    }

    private async Task<List<Recruit>> LoadOrSeedBaseRecruitsAsync()
    {
        var recruits = await LoadBaseRecruitsAsync();
        if (recruits.Count > 0)
        {
            return recruits;
        }

        Debug.LogWarning("No tavern candidates flagged in the characters table; attempting to seed fallback recruits.");

        try
        {
            await SeedDefaultRecruitsAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to seed fallback tavern recruits: {ex.Message}");
            return recruits;
        }

        recruits = await LoadBaseRecruitsAsync();
        if (recruits.Count == 0)
        {
            Debug.LogError("Fallback tavern recruit seeding did not produce any candidates. Ensure characters exist with in_tavern = 1.");
        }

        return recruits;
    }

    private async Task SeedDefaultRecruitsAsync()
    {
        string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_tavern_seed_recruits.sql");
        var parameters = new Dictionary<string, object?>
        {
            ["@limit"] = SeedRecruitBatchSize
        };

        string sql = File.ReadAllText(sqlPath);
        int affected = await DatabaseClientUnity.ExecuteAsync(sql, parameters);
        if (affected > 0)
        {
            Debug.Log($"Seeded {affected} fallback tavern recruits from existing neutral characters.");
            return;
        }

        Debug.LogWarning("Primary tavern seed script affected 0 rows. Inserting template recruits before retrying.");

        string fallbackPath = Path.Combine(Application.dataPath, "sql", "unity_tavern_insert_fallback_recruits.sql");
        int inserted = await DatabaseClientUnity.ExecuteAsync(File.ReadAllText(fallbackPath));
        if (inserted <= 0)
        {
            Debug.LogError("Fallback tavern recruit insert did not add any rows. Manual data intervention required.");
            return;
        }

        Debug.Log($"Inserted {inserted} template tavern recruits.");

        affected = await DatabaseClientUnity.ExecuteAsync(sql, parameters);
        Debug.Log($"Re-ran primary tavern seed script after fallback insert; marked {affected} recruits as available.");
    }

    private static int RollRecruitCount()
    {
        float totalWeight = RecruitCountWeights.Sum(option => option.weight);
        if (totalWeight <= 0f)
        {
            return 1;
        }

        float roll = UnityEngine.Random.value * totalWeight;
        foreach (var option in RecruitCountWeights)
        {
            if (roll <= option.weight)
            {
                return option.count;
            }

            roll -= option.weight;
        }

        return RecruitCountWeights[^1].count;
    }

    private PartyMemberGenerator.GeneratedRecruit ToGeneratedRecruit(PersistedRecruit recruit)
    {
        return new PartyMemberGenerator.GeneratedRecruit(
            recruit.Recruit,
            recruit.Stats.Clone(),
            recruit.Cost);
    }

    private async Task PruneNodeRecruitsAsync(string nodeId, bool forceReset)
    {
        string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_tavern_prune_recruits.sql");
        await DatabaseClientUnity.ExecuteAsync(
            File.ReadAllText(sqlPath),
            new Dictionary<string, object?>
            {
                ["@nodeId"] = nodeId,
                ["@forceReset"] = forceReset ? 1 : 0
            });
    }

    private async Task UpsertRecruitAsync(string nodeId, PersistedRecruit recruit)
    {
        string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_tavern_upsert_recruits.sql");
        var parameters = new Dictionary<string, object?>
        {
            ["@nodeId"] = nodeId,
            ["@recruitId"] = recruit.Recruit.id,
            ["@cost"] = recruit.Cost,
            ["@createdUtc"] = recruit.CreatedUtc,
            ["@strength"] = recruit.Stats.Strength,
            ["@dexterity"] = recruit.Stats.Dexterity,
            ["@intelligence"] = recruit.Stats.Intelligence,
            ["@maxHp"] = recruit.Stats.MaxHP,
            ["@maxMp"] = recruit.Stats.MaxMP,
            ["@actionSpeed"] = recruit.Stats.ActionSpeed,
            ["@physicalDefense"] = recruit.Stats.PhysicalDefense,
            ["@magicDefense"] = recruit.Stats.MagicDefense,
            ["@rolledPoints"] = recruit.Stats.RolledPointCount
        };

        await DatabaseClientUnity.ExecuteAsync(File.ReadAllText(sqlPath), parameters);
    }

    private async Task EnsureSchemaAsync()
    {
        if (_hasEnsuredSchema)
        {
            return;
        }

        string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_tavern_create_recruits_table.sql");

        try
        {
            await DatabaseClientUnity.ExecuteAsync(File.ReadAllText(sqlPath));
            _hasEnsuredSchema = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to ensure tavern recruit schema: {ex.Message}");
            throw;
        }
    }
}

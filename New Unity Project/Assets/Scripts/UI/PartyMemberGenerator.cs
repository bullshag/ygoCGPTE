using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Helper responsible for enriching tavern recruits with display-ready data.
/// </summary>
public class PartyMemberGenerator
{
    private const int BaseStatMinimum = 4;

    /// <summary>
    /// Build generated recruit entries from the raw tavern data.
    /// </summary>
    /// <param name="baseRecruits">Raw recruits returned by TavernManager.</param>
    /// <param name="count">Number of entries to produce.</param>
    public List<GeneratedRecruit> BuildCandidates(IEnumerable<TavernManager.Recruit> baseRecruits, int count)
    {
        var pool = baseRecruits?.Take(count) ?? Enumerable.Empty<TavernManager.Recruit>();
        var generated = new List<GeneratedRecruit>();

        foreach (var recruit in pool)
        {
            var stats = GenerateStats(recruit.level);
            int cost = Mathf.Max(25, Mathf.RoundToInt(stats.Total * 12f));
            generated.Add(new GeneratedRecruit(recruit, stats, cost));
        }

        return generated;
    }

    private RecruitStats GenerateStats(int level)
    {
        int levelBonus = Mathf.Clamp(level, 1, 20);
        return new RecruitStats
        {
            Strength = RollStat(levelBonus),
            Agility = RollStat(levelBonus),
            Intelligence = RollStat(levelBonus),
            Vitality = RollStat(levelBonus)
        };
    }

    private static int RollStat(int levelBonus)
    {
        return BaseStatMinimum + Random.Range(levelBonus, levelBonus + 6);
    }

    /// <summary>
    /// Immutable view model describing a generated recruit.
    /// </summary>
    public class GeneratedRecruit
    {
        public GeneratedRecruit(TavernManager.Recruit source, RecruitStats stats, int cost)
        {
            Source = source;
            Stats = stats;
            Cost = cost;
        }

        public TavernManager.Recruit Source { get; }

        public string Name => string.IsNullOrEmpty(Source.name)
            ? "Unknown Adventurer"
            : Source.name;

        public RecruitStats Stats { get; }

        public int Cost { get; }

        public string DisplayLabel => $"{Name} – {Cost} gold";

        public string BuildStatsDescription()
        {
            return $"STR {Stats.Strength}\nAGI {Stats.Agility}\nINT {Stats.Intelligence}\nVIT {Stats.Vitality}";
        }

        public CharacterData ToCharacterData()
        {
            return new CharacterData
            {
                Name = Name,
                MaxHP = Stats.Vitality * 10,
                HP = Stats.Vitality * 10,
                MaxMana = Mathf.Max(1, Stats.Intelligence * 5),
                Mana = Mathf.Max(1, Stats.Intelligence * 5)
            };
        }
    }

    /// <summary>
    /// Simple stat block representation.
    /// </summary>
    public struct RecruitStats
    {
        public int Strength;
        public int Agility;
        public int Intelligence;
        public int Vitality;

        public int Total => Strength + Agility + Intelligence + Vitality;
    }
}

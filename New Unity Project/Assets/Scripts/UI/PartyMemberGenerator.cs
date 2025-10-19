using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Helper responsible for enriching tavern recruits with display-ready data.
/// </summary>
public class PartyMemberGenerator
{
    private const int BaselinePrimaryStat = 5;
    private const int BaselineMaxHp = 35;
    private const int BaselineMaxMp = 35;
    private const float BaselineActionSpeed = 1f;
    private const int BaselineDefense = 0;

    private const int MinRolledPoints = 10;
    private const int MaxRolledPoints = 30;

    private const int HpPerPoint = 5;
    private const int MpPerPoint = 3;
    private const int DefensePerPoint = 3;
    private const float ActionSpeedPerPoint = 0.1f;

    /// <summary>
    /// Build generated recruit entries from the raw tavern data.
    /// </summary>
    /// <param name="baseRecruits">Raw recruits returned by TavernManager.</param>
    /// <param name="count">Number of entries to produce.</param>
    public List<GeneratedRecruit> BuildCandidates(IEnumerable<TavernManager.Recruit> baseRecruits, int count)
    {
        var recruits = baseRecruits?.ToList();
        if (recruits == null || recruits.Count == 0 || count <= 0)
        {
            return new List<GeneratedRecruit>();
        }

        int takeCount = Mathf.Clamp(count, 1, recruits.Count);
        var selected = recruits
            .OrderBy(_ => Random.value)
            .Take(takeCount);

        var generated = new List<GeneratedRecruit>();

        foreach (var recruit in selected)
        {
            var stats = GenerateStats();
            int effectivePoints = Mathf.Max(stats.GetEffectivePointTotal(), stats.RolledPointCount);
            int cost = Mathf.Max(10, effectivePoints * 10);
            generated.Add(new GeneratedRecruit(recruit, stats, cost));
        }

        return generated;
    }

    private RecruitStats GenerateStats()
    {
        var stats = RecruitStats.CreateBaseline();
        int additionalPoints = RollAdditionalPoints();

        for (int i = 0; i < additionalPoints; i++)
        {
            int statIndex = Random.Range(0, 8);
            switch (statIndex)
            {
                case 0:
                    stats.Strength += 1;
                    break;
                case 1:
                    stats.Dexterity += 1;
                    break;
                case 2:
                    stats.Intelligence += 1;
                    break;
                case 3:
                    stats.MaxHP += HpPerPoint;
                    break;
                case 4:
                    stats.MaxMP += MpPerPoint;
                    break;
                case 5:
                    stats.ActionSpeed += ActionSpeedPerPoint;
                    break;
                case 6:
                    stats.PhysicalDefense += DefensePerPoint;
                    break;
                default:
                    stats.MagicDefense += DefensePerPoint;
                    break;
            }
        }

        stats.ActionSpeed = Mathf.Round(stats.ActionSpeed * 10f) / 10f;
        stats.RolledPointCount = additionalPoints;
        return stats;
    }

    private static int RollAdditionalPoints()
    {
        float curve = Mathf.Pow(Random.value, 1.5f);
        int range = MaxRolledPoints - MinRolledPoints;
        int rolled = MinRolledPoints + Mathf.RoundToInt(curve * range);
        return Mathf.Clamp(rolled, MinRolledPoints, MaxRolledPoints);
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
            var builder = new StringBuilder();
            builder.AppendLine($"STR {Stats.Strength}");
            builder.AppendLine($"DEX {Stats.Dexterity}");
            builder.AppendLine($"INT {Stats.Intelligence}");
            builder.AppendLine($"HP {Stats.MaxHP}");
            builder.AppendLine($"MP {Stats.MaxMP}");
            builder.AppendLine($"ASPD {Stats.ActionSpeed:0.0}");
            builder.AppendLine($"P.DEF {Stats.PhysicalDefense}");
            builder.Append($"M.DEF {Stats.MagicDefense}");
            return builder.ToString();
        }

        public CharacterData ToCharacterData()
        {
            return new CharacterData
            {
                Name = Name,
                MaxHP = Stats.MaxHP,
                HP = Stats.MaxHP,
                MaxMana = Mathf.Max(1, Stats.MaxMP),
                Mana = Mathf.Max(1, Stats.MaxMP)
            };
        }
    }

    /// <summary>
    /// Simple stat block representation.
    /// </summary>
        public class RecruitStats
        {
            public int Strength { get; set; }
            public int Dexterity { get; set; }
            public int Intelligence { get; set; }
            public int MaxHP { get; set; }
        public int MaxMP { get; set; }
        public float ActionSpeed { get; set; }
        public int PhysicalDefense { get; set; }
        public int MagicDefense { get; set; }
        public int RolledPointCount { get; set; }

            public static RecruitStats CreateBaseline()
            {
                return new RecruitStats
                {
                    Strength = BaselinePrimaryStat,
                Dexterity = BaselinePrimaryStat,
                Intelligence = BaselinePrimaryStat,
                MaxHP = BaselineMaxHp,
                MaxMP = BaselineMaxMp,
                ActionSpeed = BaselineActionSpeed,
                PhysicalDefense = BaselineDefense,
                MagicDefense = BaselineDefense,
                RolledPointCount = 0
            };
            }

            public RecruitStats Clone()
            {
                return new RecruitStats
                {
                    Strength = Strength,
                    Dexterity = Dexterity,
                    Intelligence = Intelligence,
                    MaxHP = MaxHP,
                    MaxMP = MaxMP,
                    ActionSpeed = ActionSpeed,
                    PhysicalDefense = PhysicalDefense,
                    MagicDefense = MagicDefense,
                    RolledPointCount = RolledPointCount
                };
            }

        public int GetEffectivePointTotal()
        {
            int total = 0;
            total += Mathf.Max(0, Strength - BaselinePrimaryStat);
            total += Mathf.Max(0, Dexterity - BaselinePrimaryStat);
            total += Mathf.Max(0, Intelligence - BaselinePrimaryStat);
            total += Mathf.Max(0, Mathf.RoundToInt((MaxHP - BaselineMaxHp) / (float)HpPerPoint));
            total += Mathf.Max(0, Mathf.RoundToInt((MaxMP - BaselineMaxMp) / (float)MpPerPoint));
            total += Mathf.Max(0, Mathf.RoundToInt((PhysicalDefense - BaselineDefense) / (float)DefensePerPoint));
            total += Mathf.Max(0, Mathf.RoundToInt((MagicDefense - BaselineDefense) / (float)DefensePerPoint));
            total += Mathf.Max(0, Mathf.RoundToInt((ActionSpeed - BaselineActionSpeed) / ActionSpeedPerPoint));
            return total;
        }
    }
}

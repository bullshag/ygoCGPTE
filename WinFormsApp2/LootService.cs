using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public static class LootService
    {
        private static readonly Random _rng = new();

        public static Dictionary<string, int> GenerateLoot(IEnumerable<(string name, int level)> npcs, int userId, string? areaId = null)
        {
            var drops = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            foreach (var npc in npcs)
            {
                using MySqlCommand cmd = new MySqlCommand("SELECT item_name, drop_chance, min_quantity, max_quantity FROM npc_loot WHERE npc_name=@name", conn);
                cmd.Parameters.AddWithValue("@name", npc.name);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    string item = r.GetString("item_name");
                    double chance = r.GetDouble("drop_chance");
                    if (_rng.NextDouble() <= chance)
                    {
                        int min = r.GetInt32("min_quantity");
                        int max = r.GetInt32("max_quantity");
                        int qty = _rng.Next(min, max + 1);
                        if (drops.ContainsKey(item)) drops[item] += qty; else drops[item] = qty;
                    }
                }
            }
            int avgLevel = Math.Max(1, (int)Math.Round(npcs.Average(n => n.level)));
            if (drops.TryGetValue("gold", out int gold))
            {
                int boostedGold = (int)Math.Round(gold * 1.5);
                drops["gold"] = boostedGold;
                using MySqlCommand goldCmd = new MySqlCommand("UPDATE users SET gold = gold + @g WHERE id=@id", conn);
                goldCmd.Parameters.AddWithValue("@g", boostedGold);
                goldCmd.Parameters.AddWithValue("@id", userId);
                goldCmd.ExecuteNonQuery();
            }
            conn.Close();

            if (_rng.NextDouble() <= 0.03)
            {
                string? trinket = GetRandomTrinket();
                if (trinket != null)
                    drops[trinket] = drops.GetValueOrDefault(trinket) + 1;
            }

            // chance to drop additional loot from global pool
            Item? bonusLoot = LootPool.GetEnemyLoot(areaId);
            if (bonusLoot != null)
            {
                drops[bonusLoot.Name] = drops.GetValueOrDefault(bonusLoot.Name) + 1;
                InventoryService.AddItem(bonusLoot);
            }

            foreach (var kvp in drops.Where(k => k.Key != "gold"))
            {
                for (int i = 0; i < kvp.Value; i++)
                {
                    Item? item = InventoryService.CreateItem(kvp.Key);
                    if (item != null)
                    {
                        if (item is Weapon w && w.ProcChance > 0)
                        {
                            if (w.ProcAbility == null)
                            {
                                var ability = GetRandomDamageAbility();
                                if (ability != null)
                                {
                                    w.ProcAbility = ability;
                                    w.Name += $" ({ability.Name})";
                                }
                            }
                            w.NameColor = System.Drawing.Color.Purple;
                            w.Stackable = false;
                        }
                        else if (item is Weapon || item is Armor)
                        {
                            var rarity = RollRarity();
                            if (rarity != Rarity.None)
                            {
                                string baseName = item.Name;
                                item.Stackable = false;
                                item.Name = MagicItemNameGenerator.Generate(baseName, rarity);
                                ApplyBonuses(item, avgLevel, rarity);
                            }
                        }
                        InventoryService.AddItem(item);
                    }
                }
            }
            if (_rng.NextDouble() <= 0.01)
            {
                string name = SpecialWeaponGenerator.GetRandomName();
                if (InventoryService.CreateItem(name) is Weapon sw)
                {
                    var ability = GetRandomDamageAbility();
                    if (ability != null)
                    {
                        sw.ProcAbility = ability;
                        sw.Name = $"{name} ({ability.Name})";
                    }
                    drops[sw.Name] = drops.GetValueOrDefault(sw.Name) + 1;
                    InventoryService.AddItem(sw);
                }
            }
            return drops;
        }

        private static Ability? GetRandomDamageAbility()
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT id, name, description, cost, cooldown FROM abilities WHERE description LIKE '%damage%' ORDER BY RAND() LIMIT 1", conn);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                return new Ability
                {
                    Id = r.GetInt32("id"),
                    Name = r.GetString("name"),
                    Description = r.GetString("description"),
                    Cost = r.GetInt32("cost"),
                    Cooldown = r.GetInt32("cooldown")
                };
            }
            return null;
        }

        private static string? GetRandomTrinket()
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT name, drop_chance FROM trinkets", conn);
            using var r = cmd.ExecuteReader();
            var trinkets = new List<(string name, double chance)>();
            while (r.Read())
            {
                trinkets.Add((r.GetString("name"), r.GetDouble("drop_chance")));
            }
            if (trinkets.Count == 0) return null;
            double total = trinkets.Sum(t => t.chance);
            double roll = _rng.NextDouble() * total;
            double cumulative = 0;
            foreach (var t in trinkets)
            {
                cumulative += t.chance;
                if (roll <= cumulative)
                    return t.name;
            }
            return trinkets[trinkets.Count - 1].name;
        }

        private static Rarity RollRarity()
        {
            if (_rng.NextDouble() > 0.25) return Rarity.None;
            var rarity = Rarity.Green;
            if (_rng.NextDouble() <= 0.25) rarity = Rarity.Blue; else return rarity;
            if (_rng.NextDouble() <= 0.25) rarity = Rarity.Purple; else return rarity;
            if (_rng.NextDouble() <= 0.25) rarity = Rarity.Red; else return rarity;
            if (_rng.NextDouble() <= 0.10) rarity = Rarity.Rainbow;
            return rarity;
        }

        public static Rarity RollRarityForLevel(int level)
        {
            double scale = Math.Clamp(level / 50.0, 0, 1);
            double roll = _rng.NextDouble();
            double threshold = 0.2 + 0.3 * scale; // Green
            if (roll < threshold) return Rarity.Green;
            threshold += 0.05 + 0.2 * scale; // Blue
            if (roll < threshold) return Rarity.Blue;
            threshold += 0.01 + 0.1 * scale; // Purple
            if (roll < threshold) return Rarity.Purple;
            threshold += 0.002 + 0.05 * scale; // Red
            if (roll < threshold) return Rarity.Red;
            threshold += 0.0005 + 0.02 * scale; // Rainbow
            if (roll < threshold) return Rarity.Rainbow;
            return Rarity.None;
        }

        public static void ApplyBonuses(Item item, int maxPoints, Rarity rarity)
        {
            int min = rarity switch
            {
                Rarity.Green => 1,
                Rarity.Blue => 11,
                Rarity.Purple => 21,
                Rarity.Red => 31,
                Rarity.Rainbow => 41,
                _ => 1
            };
            int max = rarity switch
            {
                Rarity.Green => Math.Min(10, maxPoints),
                Rarity.Blue => Math.Min(20, maxPoints),
                Rarity.Purple => Math.Min(30, maxPoints),
                Rarity.Red => Math.Min(40, maxPoints),
                Rarity.Rainbow => maxPoints,
                _ => maxPoints
            };
            if (max < min) max = min;
            int points = _rng.Next(min, max + 1);
            item.TotalPoints = points;
            var flatOptions = new List<string> { "dex", "int", "str", "physical defense", "magical defense", "mana regen", "hp regen" };
            var percentOptions = new List<string> { "% increased max hp", "% increased max mana", "% increased dex", "% increased int", "% increased str" };
            if (item is Weapon)
            {
                percentOptions.AddRange(new[] { "% attack speed", "% crit chance", "% weapon damage", "% ability damage" });
            }
            var chosenTypes = new HashSet<string>();
            int flatPoints = Math.Min(30, points);
            for (int i = 0; i < flatPoints; i++)
            {
                string type;
                if (chosenTypes.Count < 6)
                {
                    var options = flatOptions.Where(o => !chosenTypes.Contains(o)).ToList();
                    if (options.Count == 0) options = flatOptions;
                    type = options[_rng.Next(options.Count)];
                    chosenTypes.Add(type);
                }
                else
                {
                    type = chosenTypes.ElementAt(_rng.Next(chosenTypes.Count));
                }
                item.FlatBonuses[type] = item.FlatBonuses.GetValueOrDefault(type) + 1;
            }
            int remaining = points - flatPoints;
            for (int i = 0; i < remaining; i++)
            {
                string type;
                if (chosenTypes.Count < 6)
                {
                    var options = percentOptions.Where(o => !chosenTypes.Contains(o)).ToList();
                    if (options.Count == 0) options = percentOptions;
                    type = options[_rng.Next(options.Count)];
                    chosenTypes.Add(type);
                }
                else
                {
                    var options = chosenTypes.Where(t => percentOptions.Contains(t)).ToList();
                    if (options.Count == 0) options = percentOptions;
                    type = options[_rng.Next(options.Count)];
                }
                item.PercentBonuses[type] = item.PercentBonuses.GetValueOrDefault(type) + 1;
            }

            switch (rarity)
            {
                case Rarity.Green:
                    item.NameColor = Color.Green;
                    break;
                case Rarity.Blue:
                    item.NameColor = Color.Blue;
                    break;
                case Rarity.Purple:
                    item.NameColor = Color.Purple;
                    break;
                case Rarity.Red:
                    item.NameColor = Color.Red;
                    break;
                case Rarity.Rainbow:
                    item.RainbowColors = new List<Color> { Color.Green, Color.Blue, Color.Purple, Color.Red };
                    break;
            }
        }
    }
}

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

        public static Dictionary<string, int> GenerateLoot(IEnumerable<(string name, int level)> npcs, int userId)
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
                using MySqlCommand goldCmd = new MySqlCommand("UPDATE users SET gold = gold + @g WHERE id=@id", conn);
                goldCmd.Parameters.AddWithValue("@g", gold);
                goldCmd.Parameters.AddWithValue("@id", userId);
                goldCmd.ExecuteNonQuery();
            }
            conn.Close();
            foreach (var kvp in drops.Where(k => k.Key != "gold"))
            {
                for (int i = 0; i < kvp.Value; i++)
                {
                    Item? item = InventoryService.CreateItem(kvp.Key);
                    if (item != null)
                    {
                        if (!item.Stackable)
                        {
                            ApplyBonuses(item, avgLevel);
                            InventoryService.AddItem(item);
                        }
                        else
                        {
                            InventoryService.AddItem(item);
                        }
                    }
                }
            }
            return drops;
        }

        private static void ApplyBonuses(Item item, int maxPoints)
        {
            int points = _rng.Next(1, maxPoints + 1);
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

            if (points <= 10) item.NameColor = Color.Green;
            else if (points <= 20) item.NameColor = Color.Blue;
            else if (points <= 30) item.NameColor = Color.Purple;
            else if (points <= 40) item.NameColor = Color.Red;
            else item.RainbowColors = new List<Color> { Color.Green, Color.Blue, Color.Purple, Color.Red };
        }
    }
}

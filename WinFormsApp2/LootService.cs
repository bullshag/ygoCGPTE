using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public static class LootService
    {
        private static readonly Random _rng = new();

        public static Dictionary<string, int> GenerateLoot(IEnumerable<string> npcNames, int userId)
        {
            var drops = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            foreach (var npc in npcNames)
            {
                using MySqlCommand cmd = new MySqlCommand("SELECT item_name, drop_chance, min_quantity, max_quantity FROM npc_loot WHERE npc_name=@name", conn);
                cmd.Parameters.AddWithValue("@name", npc);
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
                Item? item = InventoryService.CreateItem(kvp.Key);
                if (item != null)
                {
                    InventoryService.AddItem(item, kvp.Value);
                }
            }
            return drops;
        }
    }
}

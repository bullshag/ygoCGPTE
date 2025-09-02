using System;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public static class PowerCalculator
    {
        public static int GetNpcPower(string npcName)
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            int level;
            using (var lvlCmd = new MySqlCommand("SELECT level FROM npcs WHERE name=@n", conn))
            {
                lvlCmd.Parameters.AddWithValue("@n", npcName);
                level = Convert.ToInt32(lvlCmd.ExecuteScalar() ?? 0);
            }

            int equipCost = 0;
            using (var eqCmd = new MySqlCommand("SELECT item_name FROM npc_equipment WHERE npc_name=@n", conn))
            {
                eqCmd.Parameters.AddWithValue("@n", npcName);
                using var er = eqCmd.ExecuteReader();
                while (er.Read())
                {
                    var item = InventoryService.CreateItem(er.GetString("item_name"));
                    if (item != null)
                        equipCost += item.Price;
                }
            }

            int abilityCount;
            using (var sCmd = new MySqlCommand("SELECT COUNT(*) FROM npc_abilities WHERE npc_name=@n", conn))
            {
                sCmd.Parameters.AddWithValue("@n", npcName);
                abilityCount = Convert.ToInt32(sCmd.ExecuteScalar() ?? 0);
            }

            return (int)Math.Ceiling((level + equipCost + 3 * abilityCount) * 0.15);
        }
    }
}

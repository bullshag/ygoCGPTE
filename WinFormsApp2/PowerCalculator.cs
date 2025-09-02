using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public static class PowerCalculator
    {


        public static int CalculateNpcPower(MySqlConnection conn, string npcName, int level)
        {
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

            using (var abilCmd = new MySqlCommand("SELECT COUNT(*) FROM npc_abilities WHERE npc_name=@n", conn))
            {
                abilCmd.Parameters.AddWithValue("@n", npcName);
                abilityCount = Convert.ToInt32(abilCmd.ExecuteScalar() ?? 0);
            }

            return CalculatePower(level, equipCost, abilityCount);
        }

        public static int CalculatePower(int level, int equipmentCost, int abilityCount)
        {
            return (int)Math.Ceiling((level + equipmentCost + 3 * abilityCount) * 0.15);

        }
    }
}

using System;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public static class PowerCalculator
    {


        public static int CalculatePower(int level, int equipmentCost, int abilityCount)
        {
            return (int)Math.Ceiling((level + equipmentCost + 3 * abilityCount) * 0.15);

        }

        public static int CalculatePartyPower(int totalLevel, int totalEquipmentCost, int totalSkillCount)
        {
            return CalculatePower(totalLevel, totalEquipmentCost, totalSkillCount);
        }

        public static int GetNpcPower(string npcName)
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT power FROM npcs WHERE name=@n", conn);
            cmd.Parameters.AddWithValue("@n", npcName);
            object? result = cmd.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt32(result);
        }
    }
}

using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public static class EnemyKnowledgeService
    {
        static EnemyKnowledgeService()
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"CREATE TABLE IF NOT EXISTS enemy_kills (
account_id INT NOT NULL,
enemy_name VARCHAR(255) NOT NULL,
kill_count INT NOT NULL,
PRIMARY KEY(account_id, enemy_name))", conn);
            cmd.ExecuteNonQuery();
        }

        public static void RecordKill(int accountId, string enemyName)
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"INSERT INTO enemy_kills(account_id, enemy_name, kill_count)
VALUES(@a,@n,1)
ON DUPLICATE KEY UPDATE kill_count = kill_count + 1", conn);
            cmd.Parameters.AddWithValue("@a", accountId);
            cmd.Parameters.AddWithValue("@n", enemyName);
            cmd.ExecuteNonQuery();
        }

        public static int GetKillCount(int accountId, string enemyName)
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT kill_count FROM enemy_kills WHERE account_id=@a AND enemy_name=@n", conn);
            cmd.Parameters.AddWithValue("@a", accountId);
            cmd.Parameters.AddWithValue("@n", enemyName);
            object? result = cmd.ExecuteScalar();
            return result == null ? 0 : System.Convert.ToInt32(result);
        }

        public static List<EnemyInfo> GetEnemiesForArea(int minPower, int maxPower)
        {
            var list = new List<EnemyInfo>();
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using (var cmd = new MySqlCommand("SELECT name, role, targeting_style FROM npcs", conn))
            {
                cmd.Parameters.AddWithValue("@min", minPower);
                cmd.Parameters.AddWithValue("@max", maxPower);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string name = reader.GetString("name");
                    int power = PowerCalculator.GetNpcPower(name);
                    if (power < minPower || power > maxPower) continue;
                    var info = new EnemyInfo
                    {
                        Name = name,
                        Power = power,
                        Role = reader.GetString("role"),
                        TargetingStyle = reader.GetString("targeting_style")
                    };
                    list.Add(info);
                }
            }
            foreach (var info in list)
            {
                using var abilCmd = new MySqlCommand(@"SELECT a.name, a.description
                                                          FROM npc_abilities na
                                                          JOIN abilities a ON na.ability_id = a.id
                                                          WHERE na.npc_name=@n
                                                          ORDER BY na.slot", conn);
                abilCmd.Parameters.AddWithValue("@n", info.Name);
                using var abilReader = abilCmd.ExecuteReader();
                while (abilReader.Read())
                {
                    info.Skills.Add(new Ability
                    {
                        Name = abilReader.GetString("name"),
                        Description = abilReader.GetString("description")
                    });
                }
            }
            return list;
        }
    }

    public class EnemyInfo
    {
        public string Name { get; set; } = string.Empty;
        public int Power { get; set; }
        public string Role { get; set; } = string.Empty;
        public string TargetingStyle { get; set; } = string.Empty;
        public List<Ability> Skills { get; } = new();
        public string Description => $"Role: {Role}\nTargeting: {TargetingStyle}";
    }
}


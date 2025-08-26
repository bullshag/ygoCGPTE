using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace WinFormsApp2
{
    public static class PassiveService
    {
        public static List<Passive> GetAvailablePassives(int characterId, MySqlConnection conn)
        {
            using var cmd = new MySqlCommand(@"SELECT p.id, p.name, p.description
                                               FROM passives p
                                               WHERE p.id NOT IN (SELECT passive_id FROM character_passives WHERE character_id=@cid)", conn);
            cmd.Parameters.AddWithValue("@cid", characterId);
            using var reader = cmd.ExecuteReader();
            var list = new List<Passive>();
            while (reader.Read())
            {
                list.Add(new Passive
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Description = reader.GetString("description"),
                    Level = 0
                });
            }
            return list;
        }

        public static List<Passive> GetOwnedPassives(int characterId, MySqlConnection conn)
        {
            using var cmd = new MySqlCommand(@"SELECT p.id, p.name, p.description, cp.level
                                               FROM character_passives cp
                                               JOIN passives p ON cp.passive_id=p.id
                                               WHERE cp.character_id=@cid", conn);
            cmd.Parameters.AddWithValue("@cid", characterId);
            using var reader = cmd.ExecuteReader();
            var list = new List<Passive>();
            while (reader.Read())
            {
                list.Add(new Passive
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Description = reader.GetString("description"),
                    Level = reader.GetInt32("level")
                });
            }
            return list;
        }

        public static void PurchasePassive(int characterId, int passiveId, int cost, MySqlConnection conn)
        {
            using var spend = new MySqlCommand("UPDATE characters SET skill_points=skill_points-@c WHERE id=@cid", conn);
            spend.Parameters.AddWithValue("@c", cost);
            spend.Parameters.AddWithValue("@cid", characterId);
            spend.ExecuteNonQuery();
            using var cmd = new MySqlCommand(@"INSERT INTO character_passives(character_id, passive_id, level)
                                               VALUES(@c,@p,1)
                                               ON DUPLICATE KEY UPDATE level=level+1", conn);
            cmd.Parameters.AddWithValue("@c", characterId);
            cmd.Parameters.AddWithValue("@p", passiveId);
            cmd.ExecuteNonQuery();
        }
    }
}

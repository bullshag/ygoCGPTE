using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace WinFormsApp2
{
    public static class AbilityService
    {
        public static List<Ability> GetShopAbilities(int characterId, MySqlConnection conn)
        {
            using var cmd = new MySqlCommand(@"SELECT a.id, a.name, a.description, a.cost
                                               FROM abilities a
                                               WHERE a.id NOT IN (SELECT ability_id FROM character_abilities WHERE character_id=@cid)", conn);
            cmd.Parameters.AddWithValue("@cid", characterId);
            using var reader = cmd.ExecuteReader();
            var list = new List<Ability>();
            while (reader.Read())
            {
                list.Add(new Ability
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Description = reader.GetString("description"),
                    Cost = reader.GetInt32("cost")
                });
            }
            return list;
        }

        public static void PurchaseAbility(int characterId, int abilityId, MySqlConnection conn)
        {
            using var cmd = new MySqlCommand("INSERT INTO character_abilities(character_id, ability_id) VALUES(@c,@a)", conn);
            cmd.Parameters.AddWithValue("@c", characterId);
            cmd.Parameters.AddWithValue("@a", abilityId);
            cmd.ExecuteNonQuery();
        }

        public static List<Ability> GetCharacterAbilities(int characterId, MySqlConnection conn)
        {
            using var cmd = new MySqlCommand(@"SELECT a.id, a.name, a.description, a.cost
                                               FROM abilities a
                                               JOIN character_abilities ca ON ca.ability_id = a.id
                                               WHERE ca.character_id=@cid", conn);
            cmd.Parameters.AddWithValue("@cid", characterId);
            using var reader = cmd.ExecuteReader();
            var list = new List<Ability>();
            while (reader.Read())
            {
                list.Add(new Ability
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Description = reader.GetString("description"),
                    Cost = reader.GetInt32("cost")
                });
            }
            return list;
        }

        public static List<Ability> GetEquippedAbilities(int characterId, MySqlConnection conn)
        {
            using var cmd = new MySqlCommand(@"SELECT slot, priority, a.id, a.name, a.description, a.cost
                                               FROM character_ability_slots s
                                               LEFT JOIN abilities a ON s.ability_id = a.id
                                               WHERE s.character_id=@cid", conn);
            cmd.Parameters.AddWithValue("@cid", characterId);
            using var reader = cmd.ExecuteReader();
            var list = new List<Ability>();
            while (reader.Read())
            {
                var ability = new Ability
                {
                    Slot = reader.GetInt32("slot"),
                    Priority = reader.GetInt32("priority"),
                    Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32("id"),
                    Name = reader.IsDBNull(reader.GetOrdinal("name")) ? "-basic attack-" : reader.GetString("name"),
                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ? string.Empty : reader.GetString("description"),
                    Cost = reader.IsDBNull(reader.GetOrdinal("cost")) ? 0 : reader.GetInt32("cost")
                };
                list.Add(ability);
            }
            return list;
        }

        public static void SetAbilitySlot(int characterId, int slot, int? abilityId, int priority, MySqlConnection conn)
        {
            using var cmd = new MySqlCommand(@"INSERT INTO character_ability_slots(character_id, slot, ability_id, priority)
                                               VALUES(@c,@s,@a,@p)
                                               ON DUPLICATE KEY UPDATE ability_id=@a, priority=@p", conn);
            cmd.Parameters.AddWithValue("@c", characterId);
            cmd.Parameters.AddWithValue("@s", slot);
            if (abilityId.HasValue)
                cmd.Parameters.AddWithValue("@a", abilityId.Value);
            else
                cmd.Parameters.AddWithValue("@a", DBNull.Value);
            cmd.Parameters.AddWithValue("@p", priority);
            cmd.ExecuteNonQuery();
        }
    }
}

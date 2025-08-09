using MySql.Data.MySqlClient;
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
    }
}

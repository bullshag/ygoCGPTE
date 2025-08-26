using System;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public class RecruitCandidate
    {
        public string Name { get; set; } = string.Empty;
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int ActionSpeed { get; set; } = 1;
        public int MaxHp => 10 + 5 * Strength;
        public int Mana => 10 + 5 * Intelligence;
        public Ability? StartingAbility { get; set; }
        public Passive? StartingPassive { get; set; }

        public static RecruitCandidate Generate(Random rng, int index)
        {
            string[] names = { "Arin", "Belor", "Ciri", "Doran", "Elaine", "Faris", "Garen", "Hilda", "Iris", "Jorin" };
            var candidate = new RecruitCandidate
            {
                Name = names[rng.Next(names.Length)],
                Strength = 5,
                Dexterity = 5,
                Intelligence = 5
            };
            for (int i = 0; i < 30; i++)
            {
                int stat = rng.Next(3);
                if (stat == 0) candidate.Strength++;
                else if (stat == 1) candidate.Dexterity++;
                else candidate.Intelligence++;
            }

            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using (var abil = new MySqlCommand("SELECT id,name,description,cost,cooldown FROM abilities ORDER BY RAND() LIMIT 1", conn))
            using (var r = abil.ExecuteReader())
            {
                if (r.Read())
                {
                    candidate.StartingAbility = new Ability
                    {
                        Id = r.GetInt32("id"),
                        Name = r.GetString("name"),
                        Description = r.GetString("description"),
                        Cost = r.GetInt32("cost"),
                        Cooldown = r.GetInt32("cooldown")
                    };
                }
            }
            using (var pas = new MySqlCommand("SELECT id,name,description FROM passives ORDER BY RAND() LIMIT 1", conn))
            using (var rp = pas.ExecuteReader())
            {
                if (rp.Read())
                {
                    candidate.StartingPassive = new Passive
                    {
                        Id = rp.GetInt32("id"),
                        Name = rp.GetString("name"),
                        Description = rp.GetString("description"),
                        Level = 1
                    };
                }
            }

            return candidate;
        }
    }
}

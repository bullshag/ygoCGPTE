using System;

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
            for (int i = 0; i < 10; i++)
            {
                int stat = rng.Next(3);
                if (stat == 0) candidate.Strength++;
                else if (stat == 1) candidate.Dexterity++;
                else candidate.Intelligence++;
            }
            return candidate;
        }
    }
}

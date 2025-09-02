using System;

namespace WinFormsApp2
{
    public static class SpecialWeaponGenerator
    {
        private static readonly Random _rng = new();
        private static readonly (string Name, string Type)[] _weapons = new[]
        {
            ("Shadowstrike Dagger", "dagger"),
            ("Dragonfang Shortsword", "shortsword"),
            ("Tempest Bow", "bow"),
            ("Eternal Longsword", "longsword"),
            ("Mystic Staff", "staff"),
            ("Sorcerer's Wand", "wand"),
            ("Runebound Rod", "rod"),
            ("Titan Greataxe", "greataxe"),
            ("Reaper Scythe", "scythe"),
            ("Colossus Greatsword", "greatsword"),
            ("Soulcrusher Mace", "mace"),
            ("Earthshaker Maul", "greatmaul")
        };

        public static string GetRandomName()
        {
            return _weapons[_rng.Next(_weapons.Length)].Name;
        }

        public static bool TryGetBaseType(string name, out string type)
        {
            foreach (var w in _weapons)
            {
                if (string.Equals(w.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    type = w.Type;
                    return true;
                }
            }
            type = string.Empty;
            return false;
        }
    }
}

using System;
using System.Collections.Generic;

namespace WinFormsApp2
{
    public static class MagicItemNameGenerator
    {
        private static readonly Random _rng = new();
        private static readonly Dictionary<Rarity, string[]> Prefixes = new()
        {
            [Rarity.Green] = new[] { "Oak", "Sturdy", "Emerald", "Wild", "Verdant", "Mossy", "Forest", "Stone", "Iron", "Rugged", "Grove", "Bramble", "Herbal", "Plain", "Simple", "Humble", "Twig", "Branch", "Timber", "Grass" },
            [Rarity.Blue] = new[] { "Frost", "Azure", "Wave", "Storm", "Sky", "Sapphire", "Glacial", "Tide", "Crystal", "Mystic", "Chill", "Wind", "Ice", "Cloud", "Nimbus", "Sea", "Current", "Flow", "River", "Lake" },
            [Rarity.Purple] = new[] { "Shadow", "Night", "Void", "Eclipse", "Phantom", "Spirit", "Dusk", "Arcane", "Ethereal", "Mystic", "Spectral", "Twilight", "Gloom", "Nether", "Dream", "Wraith", "Umbral", "Rune", "Mythic", "Omen" },
            [Rarity.Red] = new[] { "Inferno", "Flame", "Dragon", "Blood", "Crimson", "Ember", "War", "Rage", "Scarlet", "Blaze", "Fury", "Molten", "Pyre", "Cinder", "Fiery", "Battle", "Ashen", "Magma", "Burning", "Sun" },
            [Rarity.Rainbow] = new[] { "Prismatic", "Celestial", "Mythical", "Legendary", "Divine", "Eternal", "Chromatic", "Radiant", "Arcadian", "Transcendent", "Harmonic", "Aurora", "Luminous", "Nebula", "Quantum", "Stellar", "Utopian", "Epoch", "Infinity", "Genesis" }
        };

        private static readonly string[] Suffixes = { "Edge", "Fang", "Song", "Bane", "Reaver", "Whisper", "Glory", "Strike", "Roar", "Gaze", "Brand", "Cry", "Oath", "Grasp", "Lament", "Shout", "Howl", "Promise", "Secret", "Gleam" };

        private static readonly HashSet<string> UsedNames = new();

        public static string Generate(string baseName, Rarity rarity)
        {
            for (int i = 0; i < 1000; i++)
            {
                var prefix = Prefixes[rarity][_rng.Next(Prefixes[rarity].Length)];
                var suffix = Suffixes[_rng.Next(Suffixes.Length)];
                string name = $"{prefix} {baseName} {suffix}";
                if (UsedNames.Add(name))
                    return name;
            }
            return $"{baseName} {_rng.Next(100000)}";
        }
    }
}

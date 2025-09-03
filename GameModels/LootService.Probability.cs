using System;

namespace WinFormsApp2
{
    public static partial class LootService
    {
        private const double BonusLootDropRate = 0.5;

        // Exposed for testing to verify bonus loot probability
        internal static bool ShouldDropBonusLoot(Random rng) => rng.NextDouble() <= BonusLootDropRate;
    }
}

using System;

namespace WinFormsApp2
{
    public static class ExperienceHelper
    {
        // Returns the total experience required to reach the given level.
        public static int GetExpForLevel(int level)
        {
            if (level <= 1) return 0;
            return 10 * level * level - 10 * level + 55;
        }

        // Returns the total experience required to reach the next level.
        public static int GetNextLevelRequirement(int currentLevel)
        {
            return GetExpForLevel(currentLevel + 1);
        }
    }
}

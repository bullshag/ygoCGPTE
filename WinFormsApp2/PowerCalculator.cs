using System;

namespace WinFormsApp2
{
    public static class PowerCalculator
    {
        public static int CalculateUnitPower(int level, int equipCost, int skillCount)
        {
            return (int)Math.Ceiling((level + equipCost + 3 * skillCount) * 0.15);
        }

        public static int CalculatePartyPower(int totalLevel, int totalEquipCost, int totalSkillCount)
        {
            return CalculateUnitPower(totalLevel, totalEquipCost, totalSkillCount);
        }
    }
}

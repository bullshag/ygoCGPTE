using System;
using System.Collections.Generic;
using System.Linq;

namespace WinFormsApp2
{
    public static class PowerCalculator
    {
        public static int Calculate(int level, IEnumerable<Item?> equipment, int abilityCount)
        {
            int equipCost = equipment.Where(i => i != null).Sum(i => i!.Price);
            return (int)Math.Ceiling((level + equipCost + 3 * abilityCount) * 0.15);
        }
    }
}

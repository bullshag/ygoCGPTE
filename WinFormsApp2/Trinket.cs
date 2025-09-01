using System.Collections.Generic;

namespace WinFormsApp2
{
    public class Trinket : Item
    {
        public Dictionary<string, double> Effects { get; } = new();
        public Trinket()
        {
            Stackable = false;
            Slot = EquipmentSlot.Trinket;
        }
    }
}

using System.Collections.Generic;
using System.Drawing;

namespace WinFormsApp2
{
    public abstract class Item
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool Stackable { get; set; }
        public EquipmentSlot? Slot { get; init; }
        public int Price { get; init; }

        public Dictionary<string, int> FlatBonuses { get; } = new();
        public Dictionary<string, int> PercentBonuses { get; } = new();
        public int TotalPoints { get; set; }
        public Color NameColor { get; set; } = Color.Black;
        public List<Color>? RainbowColors { get; set; }
    }
}

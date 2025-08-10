namespace WinFormsApp2
{
    public abstract class Item
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool Stackable { get; init; }
        public EquipmentSlot? Slot { get; init; }
    }
}

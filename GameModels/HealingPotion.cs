namespace WinFormsApp2
{
    public class HealingPotion : Item
    {
        public int HealAmount { get; init; } = 50;
        public HealingPotion()
        {
            Name = "Healing Potion";
            Description = "Restores 50 HP";
            Stackable = true;
            Slot = EquipmentSlot.LeftHand;
            Price = 30;
        }
    }
}

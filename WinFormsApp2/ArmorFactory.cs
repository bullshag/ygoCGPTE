namespace WinFormsApp2
{
    public static class ArmorFactory
    {
        public static bool TryCreate(string type, out Armor armor)
        {
            armor = type switch
            {
                "clothrobe" => new Armor { Name = "Cloth Robe", Slot = EquipmentSlot.Body, Price = 30 },
                "leatherarmor" => new Armor { Name = "Leather Armor", Slot = EquipmentSlot.Body, Price = 60 },
                "leathercap" => new Armor { Name = "Leather Cap", Slot = EquipmentSlot.Head, Price = 25 },
                "leatherboots" => new Armor { Name = "Leather Boots", Slot = EquipmentSlot.Legs, Price = 25 },
                _ => null
            };
            return armor != null;
        }

        public static Armor Create(string type)
        {
            return TryCreate(type, out var armor) ? armor : new Armor { Name = "Cloth Robe", Slot = EquipmentSlot.Body, Price = 10 };
        }
    }
}

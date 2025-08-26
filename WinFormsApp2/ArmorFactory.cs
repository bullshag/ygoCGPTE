namespace WinFormsApp2
{
    public static class ArmorFactory
    {
        public static bool TryCreate(string type, out Armor armor)
        {
            armor = type switch
            {
                "clothrobe" => new Armor { Name = "Cloth Robe", Slot = EquipmentSlot.Body, Price = 30, Description = "Simple robes favored by mages." },
                "leatherarmor" => new Armor { Name = "Leather Armor", Slot = EquipmentSlot.Body, Price = 60, Description = "Tanned hide offering balanced protection." },
                "leathercap" => new Armor { Name = "Leather Cap", Slot = EquipmentSlot.Head, Price = 25, Description = "A light leather cap." },
                "leatherboots" => new Armor { Name = "Leather Boots", Slot = EquipmentSlot.Legs, Price = 25, Description = "Sturdy leather boots." },
                "platearmor" => new Armor { Name = "Plate Armor", Slot = EquipmentSlot.Body, Price = 120, Description = "Heavy plates for frontline warriors." },
                _ => null
            };

            if (armor != null) ApplyRandomBonuses(armor);
            return armor != null;
        }

        public static Armor Create(string type)
        {
            return TryCreate(type, out var armor) ? armor : new Armor { Name = "Cloth Robe", Slot = EquipmentSlot.Body, Price = 10, Description = "Simple robes favored by mages." };
        }

        private static void ApplyRandomBonuses(Armor armor)
        {
            var rng = System.Random.Shared;
            switch (armor.Name)
            {
                case "Cloth Robe":
                    armor.FlatBonuses["Intelligence"] = rng.Next(1, 4);
                    armor.FlatBonuses["Mana"] = rng.Next(0, 4);
                    armor.FlatBonuses["HP"] = rng.Next(0, 3);
                    armor.FlatBonuses["Magic Defense"] = rng.Next(2, 6);
                    armor.FlatBonuses["Melee Defense"] = rng.Next(0, 2);
                    armor.PercentBonuses["Spell Power"] = rng.Next(1, 4);
                    break;
                case "Leather Armor":
                case "Leather Cap":
                case "Leather Boots":
                    armor.FlatBonuses["Dexterity"] = rng.Next(1, 4);
                    armor.FlatBonuses["Mana"] = rng.Next(0, 3);
                    armor.FlatBonuses["HP"] = rng.Next(0, 3);
                    armor.FlatBonuses["Magic Defense"] = rng.Next(1, 4);
                    armor.FlatBonuses["Melee Defense"] = rng.Next(1, 4);
                    break;
                case "Plate Armor":
                    armor.FlatBonuses["Strength"] = rng.Next(1, 4);
                    armor.FlatBonuses["HP"] = rng.Next(1, 5);
                    armor.FlatBonuses["Mana"] = rng.Next(0, 2);
                    armor.FlatBonuses["Melee Defense"] = rng.Next(3, 7);
                    armor.FlatBonuses["Magic Defense"] = rng.Next(0, 3);
                    break;
            }
        }
    }
}

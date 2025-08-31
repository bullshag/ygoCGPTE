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

            if (armor != null)
            {
                ApplyRandomBonuses(armor);
                return true;
            }

            if (TryCreateRegional(type, out armor))
                return true;

            return false;
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

        private static bool TryCreateRegional(string type, out Armor armor)
        {
            armor = null;
            foreach (var kv in RegionData.KeyToDisplay)
            {
                var regionKey = kv.Key;
                var regionName = kv.Value;
                if (type.StartsWith(regionKey))
                {
                    var suffix = type.Substring(regionKey.Length);
                    int i = suffix.Length - 1;
                    while (i >= 0 && char.IsDigit(suffix[i])) i--;
                    if (i < 0) return false;
                    string tierStr = suffix[(i + 1)..];
                    if (!int.TryParse(tierStr, out int tier)) return false;
                    string cls = suffix[..(i + 1)];
                    switch (cls)
                    {
                        case "cloth":
                            armor = new Armor
                            {
                                Name = $"{regionName} Cloth +{tier}",
                                Slot = EquipmentSlot.Body,
                                Price = 150 + 50 * tier,
                                Description = $"Enchanted cloth woven in {regionName}."
                            };
                            armor.FlatBonuses["Intelligence"] = 3 + tier;
                            armor.FlatBonuses["Mana"] = 5 + 2 * tier;
                            armor.FlatBonuses["Magic Defense"] = 5 + 3 * tier;
                            armor.FlatBonuses["HP"] = 2 + tier;
                            break;
                        case "leather":
                            armor = new Armor
                            {
                                Name = $"{regionName} Leather +{tier}",
                                Slot = EquipmentSlot.Body,
                                Price = 200 + 50 * tier,
                                Description = $"Reinforced leathers from {regionName}."
                            };
                            armor.FlatBonuses["Dexterity"] = 3 + tier;
                            armor.FlatBonuses["Melee Defense"] = 4 + 2 * tier;
                            armor.FlatBonuses["Magic Defense"] = 2 + tier;
                            armor.FlatBonuses["HP"] = 2 + tier;
                            break;
                        case "plate":
                            armor = new Armor
                            {
                                Name = $"{regionName} Plate +{tier}",
                                Slot = EquipmentSlot.Body,
                                Price = 250 + 75 * tier,
                                Description = $"Heavy plate forged in {regionName}."
                            };
                            armor.FlatBonuses["Strength"] = 4 + tier;
                            armor.FlatBonuses["HP"] = 5 + 2 * tier;
                            armor.FlatBonuses["Melee Defense"] = 6 + 2 * tier;
                            armor.FlatBonuses["Magic Defense"] = 2 + tier;
                            break;
                        default:
                            return false;
                    }
                    armor.Stackable = false;
                    return true;
                }
            }
            return false;
        }
    }
}

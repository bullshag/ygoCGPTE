using System;
using System.Collections.Generic;

namespace WinFormsApp2
{
    public static class ArmorFactory
    {
        private record ArmorTemplate(string Name, EquipmentSlot Slot, int Price, string Type, string Description);

        private static readonly Dictionary<string, ArmorTemplate> UniqueArmors = new(StringComparer.OrdinalIgnoreCase)
        {
            ["hidecloak"] = new("Hide Cloak", EquipmentSlot.Body, 150, "leather", "A cloak stitched from animal hides."),
            ["shadowcloak"] = new("Shadow Cloak", EquipmentSlot.Body, 120, "cloth", "A cloak that blends into shadows."),
            ["stonearmor"] = new("Stone Armor", EquipmentSlot.Body, 200, "plate", "Armor carved from stone."),
            ["searobe"] = new("Sea Robe", EquipmentSlot.Body, 120, "cloth", "Robe woven with sea motifs."),
            ["steelarmor"] = new("Steel Armor", EquipmentSlot.Body, 200, "plate", "Hardened steel plates."),
            ["silkrobe"] = new("Silk Robe", EquipmentSlot.Body, 120, "cloth", "Fine silk garment."),
            ["leatherhood"] = new("Leather Hood", EquipmentSlot.Head, 100, "leather", "A rugged leather hood."),
            ["rockplating"] = new("Rock Plating", EquipmentSlot.Body, 200, "plate", "Heavy slabs of rock."),
            ["sandcloak"] = new("Sand Cloak", EquipmentSlot.Body, 150, "leather", "Desert-worn cloak."),
            ["blessedchain"] = new("Blessed Chain", EquipmentSlot.Body, 200, "plate", "Chainmail blessed by clergy."),
            ["piratecoat"] = new("Pirate Coat", EquipmentSlot.Body, 150, "leather", "Weathered pirate attire."),
            ["monkrobe"] = new("Monk Robe", EquipmentSlot.Body, 120, "cloth", "Humble robe for monks."),
            ["darkrobe"] = new("Dark Robe", EquipmentSlot.Body, 120, "cloth", "Robe shrouded in darkness."),
            ["frostarmor"] = new("Frost Armor", EquipmentSlot.Body, 200, "plate", "Armor chilled by frost."),
            ["lightmail"] = new("Light Mail", EquipmentSlot.Body, 200, "plate", "Lightweight chainmail."),
            ["tatteredcloak"] = new("Tattered Cloak", EquipmentSlot.Body, 120, "cloth", "Worn and ragged cloak."),
            ["crimsoncoat"] = new("Crimson Coat", EquipmentSlot.Body, 150, "leather", "Bright red coat."),
            ["wardenrobe"] = new("Warden Robe", EquipmentSlot.Body, 120, "cloth", "Robes of a forest warden."),
            ["shadowarmor"] = new("Shadow Armor", EquipmentSlot.Body, 200, "plate", "Armor infused with darkness."),
            ["stormplate"] = new("Storm Plate", EquipmentSlot.Body, 200, "plate", "Plate crackling with energy."),
            ["dragonrobe"] = new("Dragon Robe", EquipmentSlot.Body, 120, "cloth", "Robe emblazoned with dragons."),
            ["ironarmor"] = new("Iron Armor", EquipmentSlot.Body, 200, "plate", "Solid iron armor."),
            ["cloak"] = new("Cloak", EquipmentSlot.Body, 120, "cloth", "A simple cloak."),
            ["goldenvestments"] = new("Golden Vestments", EquipmentSlot.Body, 120, "cloth", "Radiant ceremonial robes."),
            ["mask"] = new("Mask", EquipmentSlot.Head, 100, "leather", "A concealing mask."),
            ["etherrobe"] = new("Ether Robe", EquipmentSlot.Body, 120, "cloth", "Robe tinged with arcane ether."),
            ["emberplate"] = new("Ember Plate", EquipmentSlot.Body, 200, "plate", "Plate radiating heat."),
            ["valkyriemail"] = new("Valkyrie Mail", EquipmentSlot.Body, 200, "plate", "Mail worn by valkyries."),
            ["skyrobe"] = new("Sky Robe", EquipmentSlot.Body, 120, "cloth", "Robes dyed the color of the sky."),
            ["gravearmor"] = new("Grave Armor", EquipmentSlot.Body, 200, "plate", "Armor of a grave guardian."),
            ["camocloak"] = new("Camo Cloak", EquipmentSlot.Body, 150, "leather", "Camouflaged cloak."),
            ["silkvest"] = new("Silk Vest", EquipmentSlot.Body, 120, "cloth", "Light silk vest."),
            ["sunarmor"] = new("Sun Armor", EquipmentSlot.Body, 200, "plate", "Armor shining like the sun."),
            ["heavyshield"] = new("Heavy Shield", EquipmentSlot.LeftHand, 180, "plate", "A cumbersome protective shield."),
            ["titanshield"] = new("Titan Shield", EquipmentSlot.LeftHand, 180, "plate", "A massive shield used by titans."),
            ["scalearmor"] = new("Scale Armor", EquipmentSlot.Body, 200, "plate", "Armor made of overlapping scales."),
            ["lichrobe"] = new("Lich Robe", EquipmentSlot.Body, 120, "cloth", "Robe of a lich."),
            ["tempestscales"] = new("Tempest Scales", EquipmentSlot.Body, 200, "plate", "Scales crackling with storms."),
            ["fiendisharmor"] = new("Fiendish Armor", EquipmentSlot.Body, 200, "plate", "Armor favored by fiends."),
            ["sacredplate"] = new("Sacred Plate", EquipmentSlot.Body, 200, "plate", "Consecrated plate armor."),
            ["voidarmor"] = new("Void Armor", EquipmentSlot.Body, 200, "plate", "Armor from the void."),
            ["obsidianarmor"] = new("Obsidian Armor", EquipmentSlot.Body, 200, "plate", "Armor carved from obsidian."),
            ["mysticrobe"] = new("Mystic Robe", EquipmentSlot.Body, 120, "cloth", "Robes of a mystic."),
            ["bloodarmor"] = new("Blood Armor", EquipmentSlot.Body, 200, "plate", "Armor stained with blood."),
            ["dragonhide"] = new("Dragonhide", EquipmentSlot.Body, 200, "plate", "Armor made from dragon hide."),
        };
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

            if (UniqueArmors.TryGetValue(type, out var tmpl))
            {
                armor = new Armor { Name = tmpl.Name, Slot = tmpl.Slot, Price = tmpl.Price, Description = tmpl.Description };
                ApplyTemplateBonuses(armor, tmpl.Type);
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
            var rng = new Random();
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

        private static void ApplyTemplateBonuses(Armor armor, string template)
        {
            var rng = new Random();
            switch (template)
            {
                case "cloth":
                    armor.FlatBonuses["Intelligence"] = rng.Next(2, 5);
                    armor.FlatBonuses["Mana"] = rng.Next(1, 5);
                    armor.FlatBonuses["HP"] = rng.Next(0, 4);
                    armor.FlatBonuses["Magic Defense"] = rng.Next(3, 6);
                    armor.FlatBonuses["Melee Defense"] = rng.Next(0, 2);
                    break;
                case "leather":
                    armor.FlatBonuses["Dexterity"] = rng.Next(2, 5);
                    armor.FlatBonuses["Mana"] = rng.Next(0, 4);
                    armor.FlatBonuses["HP"] = rng.Next(1, 4);
                    armor.FlatBonuses["Magic Defense"] = rng.Next(2, 5);
                    armor.FlatBonuses["Melee Defense"] = rng.Next(2, 5);
                    break;
                case "plate":
                    armor.FlatBonuses["Strength"] = rng.Next(2, 5);
                    armor.FlatBonuses["HP"] = rng.Next(2, 6);
                    armor.FlatBonuses["Mana"] = rng.Next(0, 3);
                    armor.FlatBonuses["Melee Defense"] = rng.Next(4, 7);
                    armor.FlatBonuses["Magic Defense"] = rng.Next(1, 4);
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

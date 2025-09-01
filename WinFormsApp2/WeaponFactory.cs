namespace WinFormsApp2
{
    public static class WeaponFactory
    {
        public static bool TryCreate(string type, out Weapon weapon)
        {
            weapon = type switch
            {
                "shortsword" => new Weapon { Name = "Shortsword", Slot = EquipmentSlot.LeftHand, DexScaling = 0.75, StrScaling = 0.25, MinMultiplier = 0.8, MaxMultiplier = 1.3, Price = 50, Description = "A balanced blade for close combat." },
                "dagger" => new Weapon { Name = "Dagger", Slot = EquipmentSlot.LeftHand, DexScaling = 0.65, MinMultiplier = 0.9, MaxMultiplier = 1.1, CritDamageBonus = 0.5, Price = 20, Description = "A small blade that strikes quickly." },
                "bow" => new Weapon { Name = "Bow", Slot = EquipmentSlot.LeftHand, DexScaling = 0.75, StrScaling = 0.35, MinMultiplier = 0.8, MaxMultiplier = 1.6, TwoHanded = true, Price = 60, Description = "Ranged weapon for agile hunters." },
                "longsword" => new Weapon { Name = "Longsword", Slot = EquipmentSlot.LeftHand, StrScaling = 0.45, DexScaling = 0.30, MinMultiplier = 0.9, MaxMultiplier = 1.2, AttackSpeedMod = -0.10, Price = 80, Description = "A heavier blade favoring strength." },
                "staff" => new Weapon { Name = "Staff", Slot = EquipmentSlot.LeftHand, IntScaling = 0.90, MinMultiplier = 0.8, MaxMultiplier = 1.4, TwoHanded = true, Price = 70, Description = "Channel arcane power with this staff." },
                "wand" => new Weapon { Name = "Wand", Slot = EquipmentSlot.LeftHand, IntScaling = 0.75, DexScaling = 0.35, MinMultiplier = 0.6, MaxMultiplier = 1.0, AttackSpeedMod = 0.10, Price = 40, Description = "A light conduit for spells." },
                "rod" => new Weapon { Name = "Rod", Slot = EquipmentSlot.LeftHand, StrScaling = 0.35, IntScaling = 0.75, MinMultiplier = 0.7, MaxMultiplier = 1.5, Price = 60, Description = "A sturdy rod infused with magic." },
                "greataxe" => new Weapon { Name = "Greataxe", Slot = EquipmentSlot.LeftHand, StrScaling = 1.10, MinMultiplier = 1.1, MaxMultiplier = 2.0, CritDamageBonus = 0.25, AttackSpeedMod = -0.25, TwoHanded = true, Price = 100, Description = "A massive axe that cleaves foes." },
                "scythe" => new Weapon { Name = "Scythe", Slot = EquipmentSlot.LeftHand, DexScaling = 1.10, MinMultiplier = 0.4, MaxMultiplier = 2.2, AttackSpeedMod = 0.25, TwoHanded = true, Price = 120, Description = "A grim reaper's tool turned weapon." },
                "greatsword" => new Weapon { Name = "Greatsword", Slot = EquipmentSlot.LeftHand, DexScaling = 0.55, StrScaling = 0.55, MinMultiplier = 1.1, MaxMultiplier = 1.6, CritChanceBonus = 0.05, CritDamageBonus = 0.05, AttackSpeedMod = 0.05, TwoHanded = true, Price = 150, Description = "Two-handed blade of legend." },
                "mace" => new Weapon { Name = "Mace", Slot = EquipmentSlot.LeftHand, IntScaling = 0.15, DexScaling = 0.25, StrScaling = 0.35, MinMultiplier = 0.9, MaxMultiplier = 1.4, Price = 70, Description = "Blunt weapon that crushes armor." },
                "greatmaul" => new Weapon { Name = "Greatmaul", Slot = EquipmentSlot.LeftHand, IntScaling = 0.25, StrScaling = 0.45, DexScaling = 0.45, MinMultiplier = 0.9, MaxMultiplier = 1.6, TwoHanded = true, Price = 130, Description = "A colossal hammer requiring both hands." },
                "blade" => new Weapon { Name = "Longsword", Slot = EquipmentSlot.LeftHand, StrScaling = 0.45, DexScaling = 0.30, MinMultiplier = 0.9, MaxMultiplier = 1.2, AttackSpeedMod = -0.10, Price = 80, Description = "A heavier blade favoring strength." },
                "sword" => new Weapon { Name = "Longsword", Slot = EquipmentSlot.LeftHand, StrScaling = 0.45, DexScaling = 0.30, MinMultiplier = 0.9, MaxMultiplier = 1.2, AttackSpeedMod = -0.10, Price = 80, Description = "A heavier blade favoring strength." },
                "rapier" => new Weapon { Name = "Longsword", Slot = EquipmentSlot.LeftHand, StrScaling = 0.45, DexScaling = 0.30, MinMultiplier = 0.9, MaxMultiplier = 1.2, AttackSpeedMod = -0.10, Price = 80, Description = "A heavier blade favoring strength." },
                "cutlass" => new Weapon { Name = "Longsword", Slot = EquipmentSlot.LeftHand, StrScaling = 0.45, DexScaling = 0.30, MinMultiplier = 0.9, MaxMultiplier = 1.2, AttackSpeedMod = -0.10, Price = 80, Description = "A heavier blade favoring strength." },
                "axe" => new Weapon { Name = "Greataxe", Slot = EquipmentSlot.LeftHand, StrScaling = 1.10, MinMultiplier = 1.1, MaxMultiplier = 2.0, CritDamageBonus = 0.25, AttackSpeedMod = -0.25, TwoHanded = true, Price = 100, Description = "A massive axe that cleaves foes." },
                "hammer" => new Weapon { Name = "Greatmaul", Slot = EquipmentSlot.LeftHand, IntScaling = 0.25, StrScaling = 0.45, DexScaling = 0.45, MinMultiplier = 0.9, MaxMultiplier = 1.6, TwoHanded = true, Price = 130, Description = "A colossal hammer requiring both hands." },
                "maul" => new Weapon { Name = "Greatmaul", Slot = EquipmentSlot.LeftHand, IntScaling = 0.25, StrScaling = 0.45, DexScaling = 0.45, MinMultiplier = 0.9, MaxMultiplier = 1.6, TwoHanded = true, Price = 130, Description = "A colossal hammer requiring both hands." },
                "club" => new Weapon { Name = "Mace", Slot = EquipmentSlot.LeftHand, IntScaling = 0.15, DexScaling = 0.25, StrScaling = 0.35, MinMultiplier = 0.9, MaxMultiplier = 1.4, Price = 70, Description = "Blunt weapon that crushes armor." },
                "scepter" => new Weapon { Name = "Rod", Slot = EquipmentSlot.LeftHand, StrScaling = 0.35, IntScaling = 0.75, MinMultiplier = 0.7, MaxMultiplier = 1.5, Price = 60, Description = "A sturdy rod infused with magic." },
                "tome" => new Weapon { Name = "Rod", Slot = EquipmentSlot.LeftHand, StrScaling = 0.35, IntScaling = 0.75, MinMultiplier = 0.7, MaxMultiplier = 1.5, Price = 60, Description = "A sturdy rod infused with magic." },
                "codex" => new Weapon { Name = "Rod", Slot = EquipmentSlot.LeftHand, StrScaling = 0.35, IntScaling = 0.75, MinMultiplier = 0.7, MaxMultiplier = 1.5, Price = 60, Description = "A sturdy rod infused with magic." },
                "lance" => new Weapon { Name = "Greatsword", Slot = EquipmentSlot.LeftHand, DexScaling = 0.55, StrScaling = 0.55, MinMultiplier = 1.1, MaxMultiplier = 1.6, CritChanceBonus = 0.05, CritDamageBonus = 0.05, AttackSpeedMod = 0.05, TwoHanded = true, Price = 150, Description = "Two-handed blade of legend." },
                "spear" => new Weapon { Name = "Greatsword", Slot = EquipmentSlot.LeftHand, DexScaling = 0.55, StrScaling = 0.55, MinMultiplier = 1.1, MaxMultiplier = 1.6, CritChanceBonus = 0.05, CritDamageBonus = 0.05, AttackSpeedMod = 0.05, TwoHanded = true, Price = 150, Description = "Two-handed blade of legend." },
                "claw" => new Weapon { Name = "Dagger", Slot = EquipmentSlot.LeftHand, DexScaling = 0.65, MinMultiplier = 0.9, MaxMultiplier = 1.1, CritDamageBonus = 0.5, Price = 20, Description = "A small blade that strikes quickly." },
                "talon" => new Weapon { Name = "Dagger", Slot = EquipmentSlot.LeftHand, DexScaling = 0.65, MinMultiplier = 0.9, MaxMultiplier = 1.1, CritDamageBonus = 0.5, Price = 20, Description = "A small blade that strikes quickly." },
                _ => null,
            };

            if (weapon != null)
                return true;

            return TryCreateRegional(type, out weapon);
        }

        public static Weapon Create(string type)
        {
            return TryCreate(type, out var weapon)
                ? weapon
                : new Weapon { Name = "Fists", Slot = EquipmentSlot.LeftHand, StrScaling = 1.0, MinMultiplier = 0.8, MaxMultiplier = 1.2, Description = "Your own two hands." };
        }

        private static bool TryCreateRegional(string type, out Weapon weapon)
        {
            weapon = null;
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
                    string baseType = suffix[..(i + 1)];
                    if (baseType != "sword") return false;
                    weapon = new Weapon
                    {
                        Name = $"{regionName} Sword +{tier}",
                        Slot = EquipmentSlot.LeftHand,
                        StrScaling = 0.6 + 0.1 * tier,
                        DexScaling = 0.4 + 0.05 * tier,
                        MinMultiplier = 1.0 + 0.1 * tier,
                        MaxMultiplier = 1.4 + 0.1 * tier,
                        Price = 80 + 40 * tier,
                        Description = $"A finely crafted sword from {regionName}.",
                        TwoHanded = false
                    };
                    weapon.Stackable = false;
                    return true;
                }
            }
            return false;
        }
    }
}

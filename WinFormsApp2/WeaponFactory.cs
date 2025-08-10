namespace WinFormsApp2
{
    public static class WeaponFactory
    {
        public static Weapon Create(string type)
        {
            return type switch
            {
                "shortsword" => new Weapon { Name = "Shortsword", Slot = EquipmentSlot.LeftHand, DexScaling = 0.75, StrScaling = 0.25, MinMultiplier = 0.8, MaxMultiplier = 1.3 },
                "dagger" => new Weapon { Name = "Dagger", Slot = EquipmentSlot.LeftHand, DexScaling = 0.65, MinMultiplier = 0.9, MaxMultiplier = 1.1, CritDamageBonus = 0.5 },
                "bow" => new Weapon { Name = "Bow", Slot = EquipmentSlot.LeftHand, DexScaling = 0.75, StrScaling = 0.35, MinMultiplier = 0.8, MaxMultiplier = 1.6, TwoHanded = true },
                "longsword" => new Weapon { Name = "Longsword", Slot = EquipmentSlot.LeftHand, StrScaling = 0.45, DexScaling = 0.30, MinMultiplier = 0.9, MaxMultiplier = 1.2, AttackSpeedMod = -0.10 },
                "staff" => new Weapon { Name = "Staff", Slot = EquipmentSlot.LeftHand, IntScaling = 1.10, MinMultiplier = 0.9, MaxMultiplier = 1.6, TwoHanded = true },
                "wand" => new Weapon { Name = "Wand", Slot = EquipmentSlot.LeftHand, IntScaling = 0.75, DexScaling = 0.35, MinMultiplier = 0.6, MaxMultiplier = 1.0, AttackSpeedMod = 0.10 },
                "rod" => new Weapon { Name = "Rod", Slot = EquipmentSlot.LeftHand, StrScaling = 0.35, IntScaling = 0.75, MinMultiplier = 0.7, MaxMultiplier = 1.5 },
                "greataxe" => new Weapon { Name = "Greataxe", Slot = EquipmentSlot.LeftHand, StrScaling = 1.10, MinMultiplier = 1.1, MaxMultiplier = 2.0, CritDamageBonus = 0.25, AttackSpeedMod = -0.50, TwoHanded = true },
                "scythe" => new Weapon { Name = "Scythe", Slot = EquipmentSlot.LeftHand, DexScaling = 1.10, MinMultiplier = 0.4, MaxMultiplier = 2.2, AttackSpeedMod = 0.25, TwoHanded = true },
                "greatsword" => new Weapon { Name = "Greatsword", Slot = EquipmentSlot.LeftHand, DexScaling = 0.55, StrScaling = 0.55, MinMultiplier = 1.1, MaxMultiplier = 1.6, CritChanceBonus = 0.05, CritDamageBonus = 0.05, AttackSpeedMod = 0.05, TwoHanded = true },
                "mace" => new Weapon { Name = "Mace", Slot = EquipmentSlot.LeftHand, IntScaling = 0.15, DexScaling = 0.25, StrScaling = 0.35, MinMultiplier = 0.9, MaxMultiplier = 1.4 },
                "greatmaul" => new Weapon { Name = "Greatmaul", Slot = EquipmentSlot.LeftHand, IntScaling = 0.25, StrScaling = 0.45, DexScaling = 0.45, MinMultiplier = 0.9, MaxMultiplier = 1.6, TwoHanded = true },
                _ => new Weapon { Name = "Fists", Slot = EquipmentSlot.LeftHand, StrScaling = 1.0, MinMultiplier = 0.8, MaxMultiplier = 1.2 }
            };
        }
    }
}

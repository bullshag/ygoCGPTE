using System;
using System.Collections.Generic;

namespace WinFormsApp2
{
    public enum EquipmentSlot
    {
        Weapon,
        LeftHand,
        RightHand,
        Body,
        Legs,
        Head,
        Trinket
    }

    public class Item
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Stackable { get; set; } = true;
        public EquipmentSlot? Slot { get; set; }
        public Dictionary<string, int> FlatBonuses { get; } = new();
        public Dictionary<string, int> PercentBonuses { get; } = new();
    }

    public class InventoryItem
    {
        public Item Item { get; set; } = new Item();
        public int Quantity { get; set; }
    }

    public class Weapon : Item
    {
        public double StrScaling { get; set; }
        public double DexScaling { get; set; }
        public double IntScaling { get; set; }
        public double MinMultiplier { get; set; }
        public double MaxMultiplier { get; set; }
        public double CritChanceBonus { get; set; }
        public double CritDamageBonus { get; set; }
    }

    public class Armor : Item { }

    public class Trinket : Item
    {
        public Dictionary<string, double> Effects { get; } = new();
    }

    public class HealingPotion : Item
    {
        public int HealAmount { get; set; }
    }

    public class AbilityTome : Item
    {
        public int AbilityId { get; }
        public AbilityTome() {}
        public AbilityTome(int id) { AbilityId = id; }
    }

    public static class LootPool
    {
        public static List<Item> GetShopStock(string pool) => new();
    }

    public class MailItem
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

    public static class MailService
    {
        public static List<MailItem> GetUnread(int accountId) => new();
        public static void SendMail(int? senderId, int recipientId, string subject, string body) { }
    }
}

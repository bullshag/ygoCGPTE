using System.Collections.Generic;

namespace WinFormsApp2
{
    public enum EquipmentSlot { Weapon, Head, Body, Trinket }

    public class Item
    {
        public string Name = "";
        public string Description = "";
        public bool Stackable;
        public EquipmentSlot? Slot;
        public Dictionary<string, int> FlatBonuses = new();
        public Dictionary<string, int> PercentBonuses = new();
    }

    public class Weapon : Item
    {
        public float StrScaling;
        public float DexScaling;
        public float IntScaling;
        public float MinMultiplier;
        public float MaxMultiplier;
        public float CritChanceBonus;
        public float CritDamageBonus;
    }

    public class Armor : Item { }
    public class Trinket : Item { }

    public class HealingPotion : Item
    {
        public int HealAmount;
        public HealingPotion()
        {
            Name = "Healing Potion";
            Description = "Restores health";
            Stackable = true;
        }
    }

    public class AbilityTome : Item
    {
        public int AbilityId;
        public AbilityTome() {}
        public AbilityTome(int id) { AbilityId = id; }
    }

    public class InventoryItem
    {
        public Item Item = new Item();
        public int Quantity;
    }

    public static class InventoryService
    {
        public static List<InventoryItem> Items { get; } = new();

        public static void Load(int userId)
        {
            if (Items.Count == 0)
            {
                Items.Add(new InventoryItem { Item = new HealingPotion { HealAmount = 20 }, Quantity = 1 });
            }
        }

        public static void RemoveItem(Item item, int qty = 1)
        {
            var inv = Items.Find(i => i.Item == item);
            if (inv != null)
            {
                inv.Quantity -= qty;
                if (inv.Quantity <= 0) Items.Remove(inv);
            }
        }

        public static void Equip(string target, EquipmentSlot slot, Item item) { }

        public static Item? CreateItem(string name) => new Item { Name = name };

        public static void AddItem(Item item)
        {
            Items.Add(new InventoryItem { Item = item, Quantity = 1 });
        }
    }

    public static class LootPool
    {
        public static List<Item> GetShopStock(string pool) =>
            new List<Item>
            {
                new Item { Name = "Sword", Description = "A sharp blade" },
                new Item { Name = "Shield", Description = "A sturdy shield" }
            };
    }

    public class MailItem
    {
        public string Subject = "";
        public string Body = "";
    }

    public static class MailService
    {
        public static List<MailItem> GetUnread(int accountId) =>
            new List<MailItem> { new MailItem { Subject = "Welcome", Body = "Hello" } };
    }

    public static class TavernService
    {
        public static void NotifyPartyHired(int accountId) { }
    }
}

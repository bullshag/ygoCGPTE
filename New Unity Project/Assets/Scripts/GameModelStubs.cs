using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public abstract class Item
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool Stackable { get; set; }
        public EquipmentSlot? Slot { get; init; }
        public int Price { get; init; }

        public Dictionary<string, int> FlatBonuses { get; } = new();
        public Dictionary<string, int> PercentBonuses { get; } = new();
        public int TotalPoints { get; set; }
        public Color NameColor { get; set; } = Color.black;
        public List<Color>? RainbowColors { get; set; }
    }

    public class InventoryItem
    {
        public Item Item { get; set; } = null!;
        public int Quantity { get; set; }
    }

    public class Ability
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Cost { get; set; }
        public int PointCost { get; set; }
        public int Cooldown { get; set; }
        public int Slot { get; set; }
        public int Priority { get; set; }
    }

    public class Weapon : Item
    {
        public Weapon()
        {
            Stackable = true;
        }
        public double StrScaling { get; init; }
        public double DexScaling { get; init; }
        public double IntScaling { get; init; }
        public double MinMultiplier { get; init; }
        public double MaxMultiplier { get; init; }
        public double CritChanceBonus { get; init; }
        public double CritDamageBonus { get; init; }
        public double AttackSpeedMod { get; init; }
        public bool TwoHanded { get; init; }

        public double ProcChance { get; set; }
        public Ability? ProcAbility { get; set; }
    }

    public class Armor : Item
    {
        public Armor()
        {
            Stackable = false;
        }
    }

    public class Trinket : Item
    {
        public Dictionary<string, double> Effects { get; } = new();
        public Trinket()
        {
            Stackable = false;
            Slot = EquipmentSlot.Trinket;
        }
    }

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

    public class ArenaCoin : Item
    {
        public ArenaCoin()
        {
            Name = "Arena Coin";
            Description = "A token earned in the battle arena. Sell for 200 gold.";
            Stackable = true;
            Price = 200;
        }
    }

    public class AbilityTome : Item
    {
        public int AbilityId { get; }

        public AbilityTome(int abilityId, string abilityName, string abilityDescription)
        {
            AbilityId = abilityId;
            Name = $"Tome: {abilityName}";
            Description = abilityDescription;
            Stackable = false;
        }
    }

    public static class LootPool
    {
        private static readonly Dictionary<string, List<Item>> _shopStocks = new();

        public static List<Item> GetShopStock(string nodeId)
        {
            if (_shopStocks.TryGetValue(nodeId, out var stock))
                return stock;

            var items = new List<Item>();
            var rows = DatabaseClientUnity.QueryAsync(
                "SELECT i.name FROM shop_stock s JOIN items i ON s.item_id=i.id WHERE s.node_id=@id",
                new Dictionary<string, object?> { ["@id"] = nodeId })
                .ConfigureAwait(false).GetAwaiter().GetResult();

            foreach (var row in rows)
            {
                string name = Convert.ToString(row["name"]) ?? string.Empty;
                Item? item = InventoryServiceUnity.CreateItem(name);
                if (item != null)
                    items.Add(item);
            }
            _shopStocks[nodeId] = items;
            return items;
        }
    }

    public class MailItem
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public override string ToString() => $"{SentAt:g} - {Subject}";
    }

    public static class MailService
    {
        public static List<MailItem> GetUnread(int accountId)
        {
            var list = new List<MailItem>();
            var rows = DatabaseClientUnity.QueryAsync(
                "SELECT id,subject,body,sent_at FROM mail_messages WHERE recipient_id=@r AND is_read=0 ORDER BY sent_at",
                new Dictionary<string, object?> { ["@r"] = accountId })
                .ConfigureAwait(false).GetAwaiter().GetResult();

            foreach (var row in rows)
            {
                list.Add(new MailItem
                {
                    Id = Convert.ToInt32(row["id"]),
                    Subject = Convert.ToString(row["subject"]) ?? string.Empty,
                    Body = Convert.ToString(row["body"]) ?? string.Empty,
                    SentAt = Convert.ToDateTime(row["sent_at"])
                });
            }

            foreach (var mail in list)
            {
                DatabaseClientUnity.ExecuteAsync(
                    "UPDATE mail_messages SET is_read=1 WHERE id=@i",
                    new Dictionary<string, object?> { ["@i"] = mail.Id })
                    .ConfigureAwait(false).GetAwaiter().GetResult();
            }

            return list;
        }

        public static void SendMail(int? senderId, int recipientId, string subject, string body)
        {
            DatabaseClientUnity.ExecuteAsync(
                "INSERT INTO mail_messages(sender_id,recipient_id,subject,body) VALUES(@s,@r,@u,@b)",
                new Dictionary<string, object?>
                {
                    ["@s"] = senderId,
                    ["@r"] = recipientId,
                    ["@u"] = subject,
                    ["@b"] = body
                })
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}

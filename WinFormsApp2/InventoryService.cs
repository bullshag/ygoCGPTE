using System.Collections.Generic;
using System.Linq;

namespace WinFormsApp2
{
    public class InventoryItem
    {
        public Item Item { get; set; }
        public int Quantity { get; set; }
    }

    public static class InventoryService
    {
        private static readonly List<InventoryItem> _items = new();
        private static readonly Dictionary<string, Dictionary<EquipmentSlot, Item?>> _equipment = new();

        static InventoryService()
        {
            // starting items
            AddItem(new HealingPotion(), 3);
            AddItem(WeaponFactory.Create("shortsword"));
        }

        public static IReadOnlyList<InventoryItem> Items => _items;

        public static void AddItem(Item item, int qty = 1)
        {
            var existing = _items.FirstOrDefault(i => i.Item.Name == item.Name);
            if (existing == null)
            {
                _items.Add(new InventoryItem { Item = item, Quantity = qty });
            }
            else
            {
                existing.Quantity += qty;
            }
        }

        public static void RemoveItem(Item item, int qty = 1)
        {
            var existing = _items.FirstOrDefault(i => i.Item.Name == item.Name);
            if (existing == null) return;
            existing.Quantity -= qty;
            if (existing.Quantity <= 0)
            {
                _items.Remove(existing);
            }
        }

        public static Item? GetEquippedItem(string character, EquipmentSlot slot)
        {
            if (_equipment.TryGetValue(character, out var dict) && dict.TryGetValue(slot, out var item))
                return item;
            return null;
        }

        public static void Equip(string character, EquipmentSlot slot, Item? item)
        {
            if (!_equipment.ContainsKey(character))
                _equipment[character] = new Dictionary<EquipmentSlot, Item?>();

            if (_equipment[character].TryGetValue(slot, out var existing) && existing != null)
            {
                AddItem(existing);
            }

            if (item != null)
            {
                RemoveItem(item);
            }

            if (item is Weapon w && w.TwoHanded)
            {
                if (_equipment[character].TryGetValue(EquipmentSlot.LeftHand, out var l) && l != null && l != item)
                    AddItem(l);
                if (_equipment[character].TryGetValue(EquipmentSlot.RightHand, out var r) && r != null && r != item)
                    AddItem(r);
                _equipment[character][EquipmentSlot.LeftHand] = item;
                _equipment[character][EquipmentSlot.RightHand] = item;
            }
            else
            {
                _equipment[character][slot] = item;
                var other = slot == EquipmentSlot.LeftHand ? EquipmentSlot.RightHand : EquipmentSlot.LeftHand;
                if (_equipment[character].TryGetValue(other, out var otherItem) && otherItem is Weapon w2 && w2.TwoHanded)
                {
                    AddItem(otherItem);
                    _equipment[character][other] = null;
                }
            }
        }

        public static IEnumerable<Item> GetEquippableItems(EquipmentSlot slot, string character)
        {
            foreach (var inv in _items)
            {
                var item = inv.Item;
                if (item.Slot == null) continue;
                if (slot == EquipmentSlot.LeftHand || slot == EquipmentSlot.RightHand)
                {
                    if (item is Weapon w)
                    {
                        if (w.TwoHanded)
                        {
                            if (slot == EquipmentSlot.LeftHand) yield return item;
                        }
                        else
                        {
                            yield return item;
                        }
                    }
                    else if (item.Slot == EquipmentSlot.LeftHand)
                    {
                        yield return item;
                    }
                }
                else if (item.Slot == slot)
                {
                    yield return item;
                }
            }
        }

        public static void ConsumeEquipped(string character, EquipmentSlot slot)
        {
            if (_equipment.TryGetValue(character, out var dict))
            {
                if (dict.TryGetValue(slot, out var item) && item != null)
                {
                    dict[slot] = null;
                }
            }
        }
    }
}

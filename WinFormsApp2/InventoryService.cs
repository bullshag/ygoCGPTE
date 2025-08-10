using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

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
        private static int _userId;
        private static bool _loaded;

        public static IReadOnlyList<InventoryItem> Items => _items;

        public static void Load(int userId)
        {
            if (_loaded && _userId == userId) return;
            _loaded = true;
            _userId = userId;
            _items.Clear();
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT item_name, quantity FROM user_items WHERE account_id=@id", conn);
            cmd.Parameters.AddWithValue("@id", userId);
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    string name = r.GetString("item_name");
                    int qty = r.GetInt32("quantity");
                    Item? item = CreateItem(name);
                    if (item != null)
                    {
                        _items.Add(new InventoryItem { Item = item, Quantity = qty });
                    }
                }
            }

            using MySqlCommand eq = new MySqlCommand("SELECT character_name, slot, item_name FROM character_equipment WHERE account_id=@id", conn);
            eq.Parameters.AddWithValue("@id", userId);
            using var r2 = eq.ExecuteReader();
            while (r2.Read())
            {
                string charName = r2.GetString("character_name");
                string slot = r2.GetString("slot");
                string itemName = r2.GetString("item_name");
                Item? item = CreateItem(itemName);
                if (item != null)
                {
                    if (!_equipment.ContainsKey(charName))
                        _equipment[charName] = new Dictionary<EquipmentSlot, Item?>();
                    _equipment[charName][Enum.Parse<EquipmentSlot>(slot)] = item;
                }
            }
        }

        public static Item? CreateItem(string name)
        {
            return name switch
            {
                "Healing Potion" => new HealingPotion(),
                _ => WeaponFactory.Create(name.Replace(" ", "").ToLower())
            };
        }

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
            if (_loaded)
            {
                using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
                conn.Open();
                using MySqlCommand cmd = new MySqlCommand("INSERT INTO user_items(account_id,item_name,quantity) VALUES(@id,@name,@qty) ON DUPLICATE KEY UPDATE quantity=quantity+@qty", conn);
                cmd.Parameters.AddWithValue("@id", _userId);
                cmd.Parameters.AddWithValue("@name", item.Name);
                cmd.Parameters.AddWithValue("@qty", qty);
                cmd.ExecuteNonQuery();
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
            if (_loaded)
            {
                using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
                conn.Open();
                using MySqlCommand cmd = new MySqlCommand("UPDATE user_items SET quantity=quantity-@qty WHERE account_id=@id AND item_name=@name", conn);
                cmd.Parameters.AddWithValue("@qty", qty);
                cmd.Parameters.AddWithValue("@id", _userId);
                cmd.Parameters.AddWithValue("@name", item.Name);
                cmd.ExecuteNonQuery();
                using MySqlCommand del = new MySqlCommand("DELETE FROM user_items WHERE account_id=@id AND item_name=@name AND quantity<=0", conn);
                del.Parameters.AddWithValue("@id", _userId);
                del.Parameters.AddWithValue("@name", item.Name);
                del.ExecuteNonQuery();
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
                SaveEquipment(character, EquipmentSlot.LeftHand, item);
                SaveEquipment(character, EquipmentSlot.RightHand, item);
            }
            else
            {
                _equipment[character][slot] = item;
                var other = slot == EquipmentSlot.LeftHand ? EquipmentSlot.RightHand : EquipmentSlot.LeftHand;
                if (_equipment[character].TryGetValue(other, out var otherItem) && otherItem is Weapon w2 && w2.TwoHanded)
                {
                    AddItem(otherItem);
                    _equipment[character][other] = null;
                    SaveEquipment(character, other, null);
                }
                SaveEquipment(character, slot, item);
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
                    SaveEquipment(character, slot, null);
                }
            }
        }

        private static void SaveEquipment(string character, EquipmentSlot slot, Item? item)
        {
            if (!_loaded) return;
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            if (item == null)
            {
                using MySqlCommand cmd = new MySqlCommand("DELETE FROM character_equipment WHERE account_id=@id AND character_name=@c AND slot=@s", conn);
                cmd.Parameters.AddWithValue("@id", _userId);
                cmd.Parameters.AddWithValue("@c", character);
                cmd.Parameters.AddWithValue("@s", slot.ToString());
                cmd.ExecuteNonQuery();
            }
            else
            {
                using MySqlCommand cmd = new MySqlCommand("REPLACE INTO character_equipment(account_id,character_name,slot,item_name) VALUES(@id,@c,@s,@n)", conn);
                cmd.Parameters.AddWithValue("@id", _userId);
                cmd.Parameters.AddWithValue("@c", character);
                cmd.Parameters.AddWithValue("@s", slot.ToString());
                cmd.Parameters.AddWithValue("@n", item.Name);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

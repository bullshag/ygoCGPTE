using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using WinFormsApp2;

namespace WinFormsApp2
{
    /// <summary>
    /// Unity-friendly inventory service that mirrors the data loading behaviour of the
    /// original WinForms InventoryService. It retrieves item and equipment data from
    /// the database and exposes it to Unity scripts.
    /// </summary>
    public static class InventoryServiceUnity
    {
        public static List<InventoryItem> Items { get; } = new();
        private static readonly Dictionary<string, Dictionary<EquipmentSlot, Item?>> _equipment = new();
        private static bool _loaded;
        private static int _userId;

        public static void Load(int userId, bool forceReload = false) =>
            LoadAsync(userId, forceReload).ConfigureAwait(false).GetAwaiter().GetResult();

        public static async Task LoadAsync(int userId, bool forceReload = false)
        {
            if (_loaded && _userId == userId && !forceReload) return;
            _loaded = true;
            _userId = userId;
            Items.Clear();
            _equipment.Clear();

            await using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            await conn.OpenAsync().ConfigureAwait(false);

            await using (MySqlCommand cmd = new MySqlCommand(
                       "SELECT item_name, quantity FROM user_items WHERE account_id=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                await using var r = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await r.ReadAsync().ConfigureAwait(false))
                {
                    string name = r.GetString("item_name");
                    int qty = r.GetInt32("quantity");
                    Item? item = CreateItem(name);
                    if (item != null)
                        Items.Add(new InventoryItem { Item = item, Quantity = qty });
                }
            }

            await using (MySqlCommand cmd = new MySqlCommand(
                       "SELECT character_name, slot, item_name FROM character_equipment WHERE account_id=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                await using var r = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await r.ReadAsync().ConfigureAwait(false))
                {
                    string charName = r.GetString("character_name");
                    string slot = r.GetString("slot");
                    string itemName = r.GetString("item_name");
                    Item? item = CreateItem(itemName);
                    if (item != null)
                    {
                        if (!_equipment.ContainsKey(charName))
                            _equipment[charName] = new Dictionary<EquipmentSlot, Item?>();
                        _equipment[charName][Enum.Parse<EquipmentSlot>(slot)] = item;
                    }
                }
            }
        }

        public static Item? CreateItem(string name)
        {
            if (name == "Healing Potion")
                return new HealingPotion { HealAmount = 20 };

            if (name.StartsWith("Tome: "))
            {
                string abilityName = name.Substring(6);
                using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
                conn.Open();
                using MySqlCommand cmd = new MySqlCommand(
                    "SELECT id, description FROM abilities WHERE name=@n", conn);
                cmd.Parameters.AddWithValue("@n", abilityName);
                using var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    int id = r.GetInt32("id");
                    string desc = r.GetString("description");
                    return new AbilityTome(id) { Name = name, Description = desc, Stackable = false };
                }
            }

            return new Item { Name = name, Description = string.Empty, Stackable = true };
        }

        public static void AddItem(Item item, int qty = 1)
        {
            if (!item.Stackable)
            {
                Items.Add(new InventoryItem { Item = item, Quantity = qty });
            }
            else
            {
                var existing = Items.Find(i => i.Item.Name == item.Name);
                if (existing == null)
                    Items.Add(new InventoryItem { Item = item, Quantity = qty });
                else
                    existing.Quantity += qty;
            }

            if (_loaded)
            {
                using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
                conn.Open();
                using MySqlCommand cmd = new MySqlCommand(
                    "INSERT INTO user_items(account_id,item_name,quantity) VALUES(@id,@name,@qty) " +
                    "ON DUPLICATE KEY UPDATE quantity=quantity+@qty", conn);
                cmd.Parameters.AddWithValue("@id", _userId);
                cmd.Parameters.AddWithValue("@name", item.Name);
                cmd.Parameters.AddWithValue("@qty", qty);
                cmd.ExecuteNonQuery();
            }
        }

        public static void RemoveItem(Item item, int qty = 1)
        {
            var existing = Items.Find(i => i.Item.Name == item.Name);
            if (existing == null) return;
            existing.Quantity -= qty;
            if (existing.Quantity <= 0) Items.Remove(existing);

            if (_loaded)
            {
                using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
                conn.Open();
                using MySqlCommand cmd = new MySqlCommand(
                    "UPDATE user_items SET quantity=quantity-@qty WHERE account_id=@id AND item_name=@name", conn);
                cmd.Parameters.AddWithValue("@qty", qty);
                cmd.Parameters.AddWithValue("@id", _userId);
                cmd.Parameters.AddWithValue("@name", item.Name);
                cmd.ExecuteNonQuery();
                using MySqlCommand del = new MySqlCommand(
                    "DELETE FROM user_items WHERE account_id=@id AND item_name=@name AND quantity<=0", conn);
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
                AddItem(existing);

            if (item != null)
                RemoveItem(item);

            _equipment[character][slot] = item;
            SaveEquipment(character, slot, item);
        }

        private static void SaveEquipment(string character, EquipmentSlot slot, Item? item)
        {
            if (!_loaded) return;
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            if (item == null)
            {
                using MySqlCommand cmd = new MySqlCommand(
                    "DELETE FROM character_equipment WHERE account_id=@id AND character_name=@c AND slot=@s", conn);
                cmd.Parameters.AddWithValue("@id", _userId);
                cmd.Parameters.AddWithValue("@c", character);
                cmd.Parameters.AddWithValue("@s", slot.ToString());
                cmd.ExecuteNonQuery();
            }
            else
            {
                using MySqlCommand cmd = new MySqlCommand(
                    "REPLACE INTO character_equipment(account_id,character_name,slot,item_name) VALUES(@id,@c,@s,@n)", conn);
                cmd.Parameters.AddWithValue("@id", _userId);
                cmd.Parameters.AddWithValue("@c", character);
                cmd.Parameters.AddWithValue("@s", slot.ToString());
                cmd.Parameters.AddWithValue("@n", item.Name);
                cmd.ExecuteNonQuery();
            }
        }
    }
}


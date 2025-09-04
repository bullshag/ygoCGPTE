using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

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

        /// <summary>
        /// Account identifier for the currently loaded inventory.
        /// Exposed so other systems can reference the logged in account.
        /// </summary>
        public static int AccountId => _userId;

        public static void Load(int userId, bool forceReload = false) =>
            LoadAsync(userId, forceReload).ConfigureAwait(false).GetAwaiter().GetResult();

        public static async Task LoadAsync(int userId, bool forceReload = false)
        {
            if (_loaded && _userId == userId && !forceReload) return;
            _loaded = true;
            _userId = userId;
            Items.Clear();
            _equipment.Clear();

            string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_inventory_load.sql");
            string[] queries = File.ReadAllText(sqlPath).Split(';', StringSplitOptions.RemoveEmptyEntries);

            var parameters = new Dictionary<string, object?> { ["@id"] = userId };

            Debug.Log($"Loading inventory items for user {userId}");
            var itemRows = await DatabaseClientUnity.QueryAsync(queries[0], parameters);
            Debug.Log($"Loaded {itemRows.Count} items for user {userId}");
            foreach (var row in itemRows)
            {
                string name = Convert.ToString(row["item_name"]) ?? string.Empty;
                Item? item = CreateItem(name);
                if (item != null)
                    Items.Add(new InventoryItem { Item = item, Quantity = Convert.ToInt32(row["quantity"]) });
            }

            Debug.Log($"Loading equipment for user {userId}");
            var equipRows = await DatabaseClientUnity.QueryAsync(queries[1], parameters);
            Debug.Log($"Loaded {equipRows.Count} equipment records for user {userId}");
            foreach (var row in equipRows)
            {
                string characterName = Convert.ToString(row["character_name"]) ?? string.Empty;
                string slotName = Convert.ToString(row["slot"]) ?? string.Empty;
                string itemName = Convert.ToString(row["item_name"]) ?? string.Empty;
                Item? item = CreateItem(itemName);
                if (item != null)
                {
                    if (!_equipment.ContainsKey(characterName))
                        _equipment[characterName] = new Dictionary<EquipmentSlot, Item?>();
                    _equipment[characterName][Enum.Parse<EquipmentSlot>(slotName)] = item;
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
                var ability = GetAbilityAsync(abilityName).GetAwaiter().GetResult();
                if (ability != null)
                    return new AbilityTome(ability.Value.id, abilityName, ability.Value.description);
            }

            return new GenericItem { Name = name, Description = string.Empty, Stackable = true };
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
                string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_inventory_add.sql");
                var parameters = new Dictionary<string, object?> { ["@id"] = _userId, ["@name"] = item.Name, ["@qty"] = qty };
                Debug.Log($"Adding item {item.Name} x{qty} for user {_userId}");
                DatabaseClientUnity.ExecuteAsync(File.ReadAllText(sqlPath), parameters).GetAwaiter().GetResult();
                Debug.Log($"Added item {item.Name} x{qty} for user {_userId}");
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
                string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_inventory_remove.sql");
                var parameters = new Dictionary<string, object?> { ["@id"] = _userId, ["@name"] = item.Name, ["@qty"] = qty };
                string[] statements = File.ReadAllText(sqlPath).Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var stmt in statements)
                {
                    var sql = stmt.Trim();
                    if (string.IsNullOrWhiteSpace(sql)) continue;
                    Debug.Log($"Executing inventory removal for {item.Name} x{qty} on user {_userId}");
                    DatabaseClientUnity.ExecuteAsync(sql, parameters).GetAwaiter().GetResult();
                    Debug.Log($"Executed inventory removal for {item.Name} x{qty} on user {_userId}");
                }
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
            var parameters = new Dictionary<string, object?>
            {
                ["@id"] = _userId,
                ["@c"] = character,
                ["@s"] = slot.ToString(),
                ["@n"] = item?.Name
            };
            string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_inventory_save_equipment.sql");
            var statements = File.ReadAllText(sqlPath).Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var stmt in statements)
            {
                var sql = stmt.Trim();
                if (string.IsNullOrWhiteSpace(sql)) continue;
                Debug.Log($"Saving equipment for {character} slot {slot} with item {(item?.Name ?? "none")}");
                DatabaseClientUnity.ExecuteAsync(sql, parameters).GetAwaiter().GetResult();
                Debug.Log($"Saved equipment for {character} slot {slot}");
            }
        }

        private static async Task SendRequestAsync(UnityWebRequest req)
        {
            var op = req.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();
        }

        private static async Task<(int id, string description)?> GetAbilityAsync(string abilityName)
        {
            using var req = UnityWebRequest.Get($"{DatabaseConfig.ApiBaseUrl}/ability/{UnityWebRequest.EscapeURL(abilityName)}");
            await SendRequestAsync(req);
            if (req.result == UnityWebRequest.Result.Success)
            {
                var resp = JsonUtility.FromJson<AbilityResponse>(req.downloadHandler.text);
                return (resp.id, resp.description);
            }
            return null;
        }

        [Serializable]
        private class AbilityResponse
        {
            public int id;
            public string description = string.Empty;
        }

        private class GenericItem : Item { }
    }
}

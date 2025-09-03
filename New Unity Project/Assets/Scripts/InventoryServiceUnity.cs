using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

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

            using var req = UnityWebRequest.Get($"{DatabaseConfig.ApiBaseUrl}/inventory/{userId}");
            await SendRequestAsync(req);
            if (req.result != UnityWebRequest.Result.Success) return;
            var data = JsonUtility.FromJson<InventoryPayload>(req.downloadHandler.text);

            if (data.items != null)
            {
                foreach (var it in data.items)
                {
                    Item? item = CreateItem(it.item_name);
                    if (item != null)
                        Items.Add(new InventoryItem { Item = item, Quantity = it.quantity });
                }
            }

            if (data.equipment != null)
            {
                foreach (var eq in data.equipment)
                {
                    Item? item = CreateItem(eq.item_name);
                    if (item != null)
                    {
                        if (!_equipment.ContainsKey(eq.character_name))
                            _equipment[eq.character_name] = new Dictionary<EquipmentSlot, Item?>();
                        _equipment[eq.character_name][Enum.Parse<EquipmentSlot>(eq.slot)] = item;
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
                var ability = GetAbilityAsync(abilityName).GetAwaiter().GetResult();
                if (ability != null)
                    return new AbilityTome(ability.Value.id)
                    {
                        Name = name,
                        Description = ability.Value.description,
                        Stackable = false
                    };
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
                PostJsonAsync($"{DatabaseConfig.ApiBaseUrl}/inventory/add",
                    new InventoryUpdate { userId = _userId, itemName = item.Name, quantity = qty })
                    .GetAwaiter().GetResult();
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
                PostJsonAsync($"{DatabaseConfig.ApiBaseUrl}/inventory/remove",
                    new InventoryUpdate { userId = _userId, itemName = item.Name, quantity = qty })
                    .GetAwaiter().GetResult();
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
            var payload = new EquipRequest
            {
                userId = _userId,
                characterName = character,
                slot = slot.ToString(),
                itemName = item?.Name
            };
            PostJsonAsync($"{DatabaseConfig.ApiBaseUrl}/inventory/equip", payload)
                .GetAwaiter().GetResult();
        }

        private static async Task SendRequestAsync(UnityWebRequest req)
        {
            var op = req.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();
        }

        private static async Task PostJsonAsync(string url, object payload)
        {
            string json = JsonUtility.ToJson(payload);
            using var req = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            await SendRequestAsync(req);
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
        private class InventoryPayload
        {
            public List<ItemData> items = new();
            public List<EquipData> equipment = new();
        }

        [Serializable]
        private class ItemData
        {
            public string item_name = string.Empty;
            public int quantity;
        }

        [Serializable]
        private class EquipData
        {
            public string character_name = string.Empty;
            public string slot = string.Empty;
            public string item_name = string.Empty;
        }

        [Serializable]
        private class InventoryUpdate
        {
            public int userId;
            public string itemName = string.Empty;
            public int quantity;
        }

        [Serializable]
        private class EquipRequest
        {
            public int userId;
            public string characterName = string.Empty;
            public string slot = string.Empty;
            public string? itemName;
        }

        [Serializable]
        private class AbilityResponse
        {
            public int id;
            public string description = string.Empty;
        }
    }
}

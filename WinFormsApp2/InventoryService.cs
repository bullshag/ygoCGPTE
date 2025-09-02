using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
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
        private static readonly Dictionary<string, string> _specialWeaponTypes = new(StringComparer.OrdinalIgnoreCase);
        static InventoryService()
        {
            foreach (var name in new[]
            {
                "Shadowstrike Dagger",
                "Dragonfang Shortsword",
                "Tempest Bow",
                "Eternal Longsword",
                "Mystic Staff",
                "Sorcerer's Wand",
                "Runebound Rod",
                "Titan Greataxe",
                "Reaper Scythe",
                "Colossus Greatsword",
                "Soulcrusher Mace",
                "Earthshaker Maul"
            })
            {
                if (SpecialWeaponGenerator.TryGetBaseType(name, out var type))
                    _specialWeaponTypes[name.Replace(" ", "").ToLower()] = type;
            }
        }
        private static int _userId;
        private static bool _loaded;

        public static IReadOnlyList<InventoryItem> Items => _items;

        public static void Reload(int userId)
        {
            _items.Clear();
            _equipment.Clear();
            _loaded = false;
            Load(userId, true);
        }

        public static void Load(int userId, bool forceReload = false)
        {
            if (_loaded && _userId == userId && !forceReload) return;
            _loaded = true;
            _userId = userId;
            _items.Clear();
            _equipment.Clear();
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
            if (name == "Arena Coin") return new ArenaCoin();
            if (name == "Healing Potion") return new HealingPotion();

            string baseName = name;
            string? abilityName = null;
            var match = Regex.Match(name, @"^(.*) \((.+)\)$");
            if (match.Success)
            {
                baseName = match.Groups[1].Value;
                abilityName = match.Groups[2].Value;
            }
            string specialKey = baseName.Replace(" ", "").ToLower();
            if (_specialWeaponTypes.TryGetValue(specialKey, out var weaponType) && WeaponFactory.TryCreate(weaponType, out var special))
            {
                special.Stackable = false;
                special.Name = name;
                special.ProcChance = 0.05;
                special.NameColor = System.Drawing.Color.Purple;
                if (abilityName != null)
                {
                    using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
                    conn.Open();
                    using MySqlCommand cmd = new MySqlCommand("SELECT id, description, cost, cooldown FROM abilities WHERE name=@n", conn);
                    cmd.Parameters.AddWithValue("@n", abilityName);
                    using var r = cmd.ExecuteReader();
                    if (r.Read())
                    {
                        special.ProcAbility = new Ability
                        {
                            Id = r.GetInt32("id"),
                            Name = abilityName,
                            Description = r.GetString("description"),
                            Cost = r.GetInt32("cost"),
                            Cooldown = r.GetInt32("cooldown")
                        };
                    }
                }
                return special;
            }
            if (name.StartsWith("Tome: "))
            {
                string abilityName = name[6..];
                using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
                conn.Open();
                using MySqlCommand cmd = new MySqlCommand("SELECT id, description FROM abilities WHERE name=@n", conn);
                cmd.Parameters.AddWithValue("@n", abilityName);
                using var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    int id = r.GetInt32("id");
                    string desc = r.GetString("description");
                    return new AbilityTome(id, abilityName, desc);
                }
            }
            using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                using MySqlCommand cmd = new MySqlCommand("SELECT description,effect_json FROM trinkets WHERE name=@n", conn);
                cmd.Parameters.AddWithValue("@n", name);
                using var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    var tr = new Trinket { Name = name, Description = r.GetString("description") };
                    var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(r.GetString("effect_json"));
                    if (dict != null)
                    {
                        foreach (var kv in dict)
                        {
                            if (kv.Value.ValueKind == JsonValueKind.True || kv.Value.ValueKind == JsonValueKind.False)
                                tr.Effects[kv.Key] = kv.Value.GetBoolean() ? 1 : 0;
                            else if (kv.Value.ValueKind == JsonValueKind.Number)
                                tr.Effects[kv.Key] = kv.Value.GetDouble();
                        }
                    }
                    return tr;
                }
            }
            string key = name.Replace(" ", "").Replace("+", "").ToLower();
            if (WeaponFactory.TryCreate(key, out var weapon)) return weapon;
            if (ArmorFactory.TryCreate(key, out var armor)) return armor;

            string lower = name.ToLower().Replace("+", "");

            foreach (var region in RegionData.Keys)
            {
                if (key.StartsWith(region))
                {
                    if (WeaponFactory.TryCreate(key, out weapon))
                    {
                        weapon.Stackable = false;
                        weapon.Name = name;
                        return weapon;
                    }
                    if (ArmorFactory.TryCreate(key, out armor))
                    {
                        armor.Name = name;
                        return armor;
                    }
                }
            }
            foreach (var type in new[] { "shortsword", "dagger", "bow", "longsword", "staff", "wand", "rod", "greataxe", "scythe", "greatsword", "mace", "greatmaul", "blade", "sword", "rapier", "cutlass", "axe", "hammer", "club", "maul", "scepter", "lance", "spear", "claw", "talon", "tome", "codex" })
            {
                if (lower.Contains(type))
                {
                    if (WeaponFactory.TryCreate(type, out weapon))
                    {
                        weapon.Stackable = false;
                        weapon.Name = name;
                        return weapon;
                    }
                }
            }
            foreach (var type in new[] { "clothrobe", "leatherarmor", "leathercap", "leatherboots", "platearmor" })
            {
                if (lower.Replace(" ", "").Contains(type))
                {
                    if (ArmorFactory.TryCreate(type, out armor))
                    {
                        armor.Name = name;
                        return armor;
                    }
                }
            }
            return null;
        }

        public static void AddItem(Item item, int qty = 1)
        {
            if (!item.Stackable)
            {
                _items.Add(new InventoryItem { Item = item, Quantity = qty });
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
                return;
            }

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


        public static Dictionary<EquipmentSlot, Item?> GetNpcEquipment(string npcName)
        {
            var dict = new Dictionary<EquipmentSlot, Item?>();
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT slot, item_name FROM npc_equipment WHERE npc_name=@n", conn);
            cmd.Parameters.AddWithValue("@n", npcName);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var slot = Enum.Parse<EquipmentSlot>(r.GetString("slot"));
                var item = CreateItem(r.GetString("item_name"));
                if (item is Weapon w && w.TwoHanded)
                {
                    dict[EquipmentSlot.LeftHand] = w;
                    dict[EquipmentSlot.RightHand] = w;
                }
                else
                {
                    dict[slot] = item;
                }
            }
            return dict;
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
                if (slot == EquipmentSlot.LeftHand || slot == EquipmentSlot.RightHand)
                {
                    var other = slot == EquipmentSlot.LeftHand ? EquipmentSlot.RightHand : EquipmentSlot.LeftHand;
                    if (_equipment[character].TryGetValue(other, out var otherItem) && otherItem is Weapon w2 && w2.TwoHanded)
                    {
                        AddItem(otherItem);
                        _equipment[character][other] = null;
                        SaveEquipment(character, other, null);
                    }
                }
                SaveEquipment(character, slot, item);
            }
        }

        public static void ApplyEquipmentBonuses(string character, ref int strength, ref int dexterity,
            ref int intelligence, ref int currentHp, ref int maxHp, ref int mana, ref int maxMana)
        {
            if (!_equipment.TryGetValue(character, out var dict)) return;
            foreach (var item in dict.Values)
            {
                if (item == null) continue;
                foreach (var kv in item.FlatBonuses)
                {
                    switch (kv.Key)
                    {
                        case "Strength":
                            strength += kv.Value;
                            break;
                        case "Dexterity":
                            dexterity += kv.Value;
                            break;
                        case "Intelligence":
                            intelligence += kv.Value;
                            maxMana += 5 * kv.Value;
                            mana += 5 * kv.Value;
                            break;
                        case "HP":
                            maxHp += kv.Value;
                            currentHp += kv.Value;
                            break;
                        case "Mana":
                            maxMana += kv.Value;
                            mana += kv.Value;
                            break;
                    }
                }
                foreach (var kv in item.PercentBonuses)
                {
                    switch (kv.Key)
                    {
                        case "Strength":
                            strength = (int)(strength * (1 + kv.Value / 100.0));
                            break;
                        case "Dexterity":
                            dexterity = (int)(dexterity * (1 + kv.Value / 100.0));
                            break;
                        case "Intelligence":
                            int oldInt = intelligence;
                            intelligence = (int)(intelligence * (1 + kv.Value / 100.0));
                            int delta = intelligence - oldInt;
                            maxMana += 5 * delta;
                            mana += 5 * delta;
                            break;
                        case "HP":
                            maxHp = (int)(maxHp * (1 + kv.Value / 100.0));
                            currentHp = (int)(currentHp * (1 + kv.Value / 100.0));
                            break;
                        case "Mana":
                            maxMana = (int)(maxMana * (1 + kv.Value / 100.0));
                            mana = (int)(mana * (1 + kv.Value / 100.0));
                            break;
                    }
                }
                if (item is Trinket tr)
                {
                    foreach (var ev in tr.Effects)
                    {
                        switch (ev.Key)
                        {
                            case "max_hp_pct":
                                maxHp = (int)(maxHp * (1 + ev.Value / 100.0));
                                currentHp = (int)(currentHp * (1 + ev.Value / 100.0));
                                break;
                            case "max_mana_pct":
                                maxMana = (int)(maxMana * (1 + ev.Value / 100.0));
                                mana = (int)(mana * (1 + ev.Value / 100.0));
                                break;
                        }
                    }
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

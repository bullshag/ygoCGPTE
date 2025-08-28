using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MySql.Data.MySqlClient;
using WinFormsApp2;

namespace WinFormsApp2.Multiplayer

{
    public class HireableMember
    {
        public string Name { get; set; } = string.Empty;
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int Experience { get; set; }
        public int ActionSpeed { get; set; }
        public Dictionary<EquipmentSlot, string?> Equipment { get; set; } = new();
        public Dictionary<int, AbilitySlotMapping> AbilitySlots { get; set; } = new();
        public HashSet<int> Abilities { get; set; } = new();
        public Dictionary<int, int> Passives { get; set; } = new();
        public int MaxHp => 10 + Strength * 5;
        public override string ToString() => Name;
    }

    public class AbilitySlotMapping
    {
        public int? AbilityId { get; set; }
        public int Priority { get; set; }
    }

    public class HireableParty
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Cost { get; set; }
        public List<HireableMember> Members { get; set; } = new();
        public bool OnMission { get; set; }
        public int? CurrentHirer { get; set; }
        public DateTime? HiredUntil { get; set; }
        public int GoldEarned { get; set; }
        public override string ToString() => Name;
    }

    public static class PartyHireService
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "party_hires.json");
        private static readonly object _sync = new();
        private static List<HireableParty>? _cache;

        private static List<HireableParty> LoadState()
        {
            lock (_sync)
            {
                if (_cache != null) return _cache;
                if (!File.Exists(FilePath))
                {
                    _cache = new List<HireableParty>();
                }
                else
                {
                    string json = File.ReadAllText(FilePath);
                    _cache = JsonSerializer.Deserialize<List<HireableParty>>(json) ?? new List<HireableParty>();
                }
                return _cache;
            }
        }

        private static void SaveState()
        {
            lock (_sync)
            {
                var json = JsonSerializer.Serialize(_cache ?? new List<HireableParty>());
                File.WriteAllText(FilePath, json);
            }
        }

        private static void CreateMercenaries(int hirerId, HireableParty party, MySqlConnection conn)
        {
            foreach (var m in party.Members)
            {
                using var cmd = new MySqlCommand(
                    "INSERT INTO characters (account_id, name, current_hp, max_hp, mana, experience_points, action_speed, strength, dex, intelligence, melee_defense, magic_defense, level, skill_points, in_tavern, in_arena, is_dead, role, targeting_style, is_mercenary) " +
                    "VALUES (@a,@n,@hp,@max,@mana,@exp,@spd,@str,@dex,@int,0,0,1,0,0,0,0,'DPS','no priorities',1)", conn);
                cmd.Parameters.AddWithValue("@a", hirerId);
                cmd.Parameters.AddWithValue("@n", m.Name);
                int hp = m.MaxHp;
                cmd.Parameters.AddWithValue("@hp", hp);
                cmd.Parameters.AddWithValue("@max", hp);
                cmd.Parameters.AddWithValue("@mana", 10 + 5 * m.Intelligence);
                cmd.Parameters.AddWithValue("@exp", m.Experience);
                cmd.Parameters.AddWithValue("@spd", m.ActionSpeed);
                cmd.Parameters.AddWithValue("@str", m.Strength);
                cmd.Parameters.AddWithValue("@dex", m.Dexterity);
                cmd.Parameters.AddWithValue("@int", m.Intelligence);
                cmd.ExecuteNonQuery();
                long characterId = cmd.LastInsertedId;

                foreach (var eq in m.Equipment)
                {
                    using var eqCmd = new MySqlCommand("INSERT INTO character_equipment(account_id, character_name, slot, item_name) VALUES(@acc,@name,@slot,@item)", conn);
                    eqCmd.Parameters.AddWithValue("@acc", hirerId);
                    eqCmd.Parameters.AddWithValue("@name", m.Name);
                    eqCmd.Parameters.AddWithValue("@slot", eq.Key.ToString());
                    eqCmd.Parameters.AddWithValue("@item", eq.Value);
                    eqCmd.ExecuteNonQuery();
                }

                foreach (var abil in m.Abilities)
                {
                    using var abilCmd = new MySqlCommand("INSERT INTO character_abilities(character_id, ability_id) VALUES(@cid,@aid)", conn);
                    abilCmd.Parameters.AddWithValue("@cid", characterId);
                    abilCmd.Parameters.AddWithValue("@aid", abil);
                    abilCmd.ExecuteNonQuery();
                }

                foreach (var slot in m.AbilitySlots)
                {
                    using var slotCmd = new MySqlCommand("INSERT INTO character_ability_slots(character_id, slot, ability_id, priority) VALUES(@cid,@slot,@aid,@pri)", conn);
                    slotCmd.Parameters.AddWithValue("@cid", characterId);
                    slotCmd.Parameters.AddWithValue("@slot", slot.Key);
                    if (slot.Value.AbilityId.HasValue)
                        slotCmd.Parameters.AddWithValue("@aid", slot.Value.AbilityId.Value);
                    else
                        slotCmd.Parameters.AddWithValue("@aid", DBNull.Value);
                    slotCmd.Parameters.AddWithValue("@pri", slot.Value.Priority);
                    slotCmd.ExecuteNonQuery();
                }

                foreach (var pas in m.Passives)
                {
                    using var passCmd = new MySqlCommand("INSERT INTO character_passives(character_id, passive_id, level) VALUES(@cid,@pid,@lvl)", conn);
                    passCmd.Parameters.AddWithValue("@cid", characterId);
                    passCmd.Parameters.AddWithValue("@pid", pas.Key);
                    passCmd.Parameters.AddWithValue("@lvl", pas.Value);
                    passCmd.ExecuteNonQuery();
                }
            }
        }

        private static void RemoveMercenaries(int hirerId, IEnumerable<string> names)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            string inClause = string.Join(",", names.Select((_, i) => "@n" + i));
            if (string.IsNullOrEmpty(inClause)) return;

            var commands = new List<MySqlCommand>
            {
                new MySqlCommand($"DELETE FROM character_equipment WHERE account_id=@a AND character_name IN ({inClause})", conn),
                new MySqlCommand($"DELETE s FROM character_ability_slots s JOIN characters c ON s.character_id=c.id WHERE c.account_id=@a AND c.is_mercenary=1 AND c.name IN ({inClause})", conn),
                new MySqlCommand($"DELETE a FROM character_abilities a JOIN characters c ON a.character_id=c.id WHERE c.account_id=@a AND c.is_mercenary=1 AND c.name IN ({inClause})", conn),
                new MySqlCommand($"DELETE p FROM character_passives p JOIN characters c ON p.character_id=c.id WHERE c.account_id=@a AND c.is_mercenary=1 AND c.name IN ({inClause})", conn),
                new MySqlCommand($"DELETE FROM characters WHERE account_id=@a AND is_mercenary=1 AND name IN ({inClause})", conn)
            };

            foreach (var cmd in commands)
            {
                cmd.Parameters.AddWithValue("@a", hirerId);
                int idx = 0;
                foreach (var n in names)
                {
                    cmd.Parameters.AddWithValue("@n" + idx, n);
                    idx++;
                }
                cmd.ExecuteNonQuery();
            }
        }

        private static void SyncFromMercenaries(int hirerId, HireableParty party)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            foreach (var m in party.Members)
            {
                int charId = 0;
                using (var cmd = new MySqlCommand("SELECT id, experience_points, action_speed FROM characters WHERE account_id=@a AND name=@n AND is_mercenary=1", conn))
                {
                    cmd.Parameters.AddWithValue("@a", hirerId);
                    cmd.Parameters.AddWithValue("@n", m.Name);
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        charId = reader.GetInt32("id");
                        m.Experience = reader.GetInt32("experience_points");
                        m.ActionSpeed = reader.GetInt32("action_speed");
                    }
                }
                if (charId == 0) continue;

                using (var eqCmd = new MySqlCommand("SELECT slot, item_name FROM character_equipment WHERE account_id=@a AND character_name=@n", conn))
                {
                    eqCmd.Parameters.AddWithValue("@a", hirerId);
                    eqCmd.Parameters.AddWithValue("@n", m.Name);
                    using var eqReader = eqCmd.ExecuteReader();
                    m.Equipment = new();
                    while (eqReader.Read())
                    {
                        var slot = Enum.Parse<EquipmentSlot>(eqReader.GetString("slot"), true);
                        m.Equipment[slot] = eqReader.GetString("item_name");
                    }
                }

                using (var abilCmd = new MySqlCommand("SELECT ability_id FROM character_abilities WHERE character_id=@cid", conn))
                {
                    abilCmd.Parameters.AddWithValue("@cid", charId);
                    using var abilReader = abilCmd.ExecuteReader();
                    m.Abilities = new();
                    while (abilReader.Read())
                    {
                        m.Abilities.Add(abilReader.GetInt32("ability_id"));
                    }
                }

                using (var slotCmd = new MySqlCommand("SELECT slot, ability_id, priority FROM character_ability_slots WHERE character_id=@cid", conn))
                {
                    slotCmd.Parameters.AddWithValue("@cid", charId);
                    using var slotReader = slotCmd.ExecuteReader();
                    m.AbilitySlots = new();
                    while (slotReader.Read())
                    {
                        int slot = slotReader.GetInt32("slot");
                        int priority = slotReader.GetInt32("priority");
                        int? abilityId = slotReader.IsDBNull(slotReader.GetOrdinal("ability_id")) ? (int?)null : slotReader.GetInt32("ability_id");
                        m.AbilitySlots[slot] = new AbilitySlotMapping { AbilityId = abilityId, Priority = priority };
                    }
                }

                using (var passCmd = new MySqlCommand("SELECT passive_id, level FROM character_passives WHERE character_id=@cid", conn))
                {
                    passCmd.Parameters.AddWithValue("@cid", charId);
                    using var passReader = passCmd.ExecuteReader();
                    m.Passives = new();
                    while (passReader.Read())
                    {
                        m.Passives[passReader.GetInt32("passive_id")] = passReader.GetInt32("level");
                    }
                }
            }
        }

        private static void CleanupExpired()
        {
            var list = LoadState();
            bool changed = false;
            var now = DateTime.UtcNow;
            foreach (var p in list.Where(p => p.OnMission && p.HiredUntil <= now).ToList())
            {
                if (p.CurrentHirer.HasValue)
                {
                    SyncFromMercenaries(p.CurrentHirer.Value, p);
                    RemoveMercenaries(p.CurrentHirer.Value, p.Members.Select(m => m.Name));
                }
                p.OnMission = false;
                p.CurrentHirer = null;
                p.HiredUntil = null;
                changed = true;
            }
            if (changed) SaveState();
        }

        public static List<HireableParty> GetAvailableParties()
        {
            CleanupExpired();
            return LoadState().Where(p => !p.OnMission).ToList();
        }

        public static List<HireableParty> GetOwnerParties(int ownerId)
        {
            CleanupExpired();
            return LoadState().Where(p => p.OwnerId == ownerId).ToList();
        }

        public static HashSet<string> GetHiredMemberNames(int ownerId)
        {
            CleanupExpired();
            return LoadState()
                .Where(p => p.OwnerId == ownerId && p.OnMission)
                .SelectMany(p => p.Members.Select(m => m.Name))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public static bool DepositAccountParty(int ownerId, int cost)
        {
            var members = new List<(int Id, HireableMember Member)>();
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT id,name,strength,dex,intelligence,experience_points,action_speed FROM characters WHERE account_id=@id AND is_dead=0 AND in_arena=0 AND in_tavern=0", conn);
            cmd.Parameters.AddWithValue("@id", ownerId);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var member = new HireableMember
                    {
                        Name = reader.GetString("name"),
                        Strength = reader.GetInt32("strength"),
                        Dexterity = reader.GetInt32("dex"),
                        Intelligence = reader.GetInt32("intelligence"),
                        Experience = reader.GetInt32("experience_points"),
                        ActionSpeed = reader.GetInt32("action_speed")
                    };
                    members.Add((reader.GetInt32("id"), member));
                }
            }

            if (members.Count == 0) return false;

            foreach (var (charId, member) in members)
            {
                using (var eqCmd = new MySqlCommand("SELECT slot, item_name FROM character_equipment WHERE account_id=@a AND character_name=@n", conn))
                {
                    eqCmd.Parameters.AddWithValue("@a", ownerId);
                    eqCmd.Parameters.AddWithValue("@n", member.Name);
                    using var eqReader = eqCmd.ExecuteReader();
                    while (eqReader.Read())
                    {
                        var slot = Enum.Parse<EquipmentSlot>(eqReader.GetString("slot"), true);
                        member.Equipment[slot] = eqReader.GetString("item_name");
                    }
                }

                using (var abilCmd = new MySqlCommand("SELECT ability_id FROM character_abilities WHERE character_id=@cid", conn))
                {
                    abilCmd.Parameters.AddWithValue("@cid", charId);
                    using var abilReader = abilCmd.ExecuteReader();
                    while (abilReader.Read())
                    {
                        member.Abilities.Add(abilReader.GetInt32("ability_id"));
                    }
                }

                using (var slotCmd = new MySqlCommand("SELECT slot, ability_id, priority FROM character_ability_slots WHERE character_id=@cid", conn))
                {
                    slotCmd.Parameters.AddWithValue("@cid", charId);
                    using var slotReader = slotCmd.ExecuteReader();
                    while (slotReader.Read())
                    {
                        int slot = slotReader.GetInt32("slot");
                        int priority = slotReader.GetInt32("priority");
                        int? abilityId = slotReader.IsDBNull(slotReader.GetOrdinal("ability_id")) ? (int?)null : slotReader.GetInt32("ability_id");
                        member.AbilitySlots[slot] = new AbilitySlotMapping { AbilityId = abilityId, Priority = priority };
                    }
                }

                using (var passCmd = new MySqlCommand("SELECT passive_id, level FROM character_passives WHERE character_id=@cid", conn))
                {
                    passCmd.Parameters.AddWithValue("@cid", charId);
                    using var passReader = passCmd.ExecuteReader();
                    while (passReader.Read())
                    {
                        member.Passives[passReader.GetInt32("passive_id")] = passReader.GetInt32("level");
                    }
                }
            }

            using (var upd = new MySqlCommand("UPDATE characters SET in_tavern=1 WHERE account_id=@id AND is_dead=0 AND in_arena=0 AND in_tavern=0", conn))
            {
                upd.Parameters.AddWithValue("@id", ownerId);
                upd.ExecuteNonQuery();
            }
            var party = new HireableParty
            {
                OwnerId = ownerId,
                Name = $"{members[0].Member.Name}'s Party",
                Cost = cost,
                Members = members.Select(m => m.Member).ToList()
            };
            var list = LoadState();
            list.Add(party);
            SaveState();
            return true;
        }

        public static bool HireParty(int hirerId, HireableParty party)
        {
            CleanupExpired();
            if (party.OnMission || party.OwnerId == hirerId) return false;
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using (var goldCmd = new MySqlCommand("SELECT gold FROM users WHERE id=@id", conn))
            {
                goldCmd.Parameters.AddWithValue("@id", hirerId);
                int gold = Convert.ToInt32(goldCmd.ExecuteScalar() ?? 0);
                if (gold < party.Cost)
                    return false;
            }
            using (var sizeCmd = new MySqlCommand("SELECT COUNT(*) FROM characters WHERE account_id=@id AND is_dead=0 AND in_arena=0 AND in_tavern=0", conn))
            {
                sizeCmd.Parameters.AddWithValue("@id", hirerId);
                int currentSize = Convert.ToInt32(sizeCmd.ExecuteScalar() ?? 0);
                if (currentSize + party.Members.Count > GameConfig.MAX_PARTY_SIZE)
                    return false;
            }
            using (var pay = new MySqlCommand("UPDATE users SET gold = GREATEST(gold-@c,0) WHERE id=@id", conn))
            {
                pay.Parameters.AddWithValue("@c", party.Cost);
                pay.Parameters.AddWithValue("@id", hirerId);
                pay.ExecuteNonQuery();
            }
            var list = LoadState();
            var target = list.FirstOrDefault(p => p.Id == party.Id);
            if (target == null) return false;
            target.OnMission = true;
            target.CurrentHirer = hirerId;
            target.HiredUntil = DateTime.UtcNow.AddMinutes(30);
            target.GoldEarned += party.Cost;
            SaveState();
            CreateMercenaries(hirerId, target, conn);
            return true;
        }

        public static int RetrieveParty(int ownerId, HireableParty party)
        {
            CleanupExpired();
            var list = LoadState();
            var existing = list.FirstOrDefault(p => p.Id == party.Id && p.OwnerId == ownerId);
            if (existing == null || existing.OnMission) return 0;
            if (existing.CurrentHirer.HasValue)
            {
                SyncFromMercenaries(existing.CurrentHirer.Value, existing);
                RemoveMercenaries(existing.CurrentHirer.Value, existing.Members.Select(m => m.Name));
                existing.CurrentHirer = null;
            }
            list.Remove(existing);
            SaveState();
            int gold = existing.GoldEarned;
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using (var upd = new MySqlCommand("UPDATE characters SET in_tavern=0 WHERE account_id=@id", conn))
            {
                upd.Parameters.AddWithValue("@id", ownerId);
                upd.ExecuteNonQuery();
            }
            if (gold > 0)
            {
                using MySqlCommand add = new MySqlCommand("UPDATE users SET gold = gold + @g WHERE id=@id", conn);
                add.Parameters.AddWithValue("@g", gold);
                add.Parameters.AddWithValue("@id", ownerId);
                add.ExecuteNonQuery();
            }
            return gold;
        }

        public static void ApplyMercenaryExperience(int hirerId, Dictionary<string, int> expGains)
        {
            if (expGains == null || expGains.Count == 0) return;
            CleanupExpired();
            var list = LoadState();
            bool changed = false;
            foreach (var party in list.Where(p => p.OnMission && p.CurrentHirer == hirerId))
            {
                foreach (var member in party.Members)
                {
                    if (expGains.TryGetValue(member.Name, out int gain))
                    {
                        member.Experience += gain;
                        changed = true;
                    }
                }
            }
            if (changed) SaveState();
        }
    }
}


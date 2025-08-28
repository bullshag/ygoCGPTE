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
        public int MaxHp => 10 + Strength * 5;
        public override string ToString() => Name;
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
                    "VALUES (@a,@n,@hp,@max,@mana,@exp,1,@str,@dex,@int,0,0,1,0,0,0,0,'DPS','no priorities',1)", conn);
                cmd.Parameters.AddWithValue("@a", hirerId);
                cmd.Parameters.AddWithValue("@n", m.Name);
                int hp = m.MaxHp;
                cmd.Parameters.AddWithValue("@hp", hp);
                cmd.Parameters.AddWithValue("@max", hp);
                cmd.Parameters.AddWithValue("@mana", 10 + 5 * m.Intelligence);
                cmd.Parameters.AddWithValue("@exp", m.Experience);
                cmd.Parameters.AddWithValue("@str", m.Strength);
                cmd.Parameters.AddWithValue("@dex", m.Dexterity);
                cmd.Parameters.AddWithValue("@int", m.Intelligence);
                cmd.ExecuteNonQuery();
            }
        }

        private static void RemoveMercenaries(int hirerId, IEnumerable<string> names)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            string inClause = string.Join(",", names.Select((_, i) => "@n" + i));
            if (string.IsNullOrEmpty(inClause)) return;
            string sql = $"DELETE FROM characters WHERE account_id=@a AND is_mercenary=1 AND name IN ({inClause})";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@a", hirerId);
            int index = 0;
            foreach (var n in names)
            {
                cmd.Parameters.AddWithValue("@n" + index, n);
                index++;
            }
            cmd.ExecuteNonQuery();
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
            var members = new List<HireableMember>();
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT name,strength,dex,intelligence,experience_points FROM characters WHERE account_id=@id AND is_dead=0 AND in_arena=0 AND in_tavern=0", conn);
            cmd.Parameters.AddWithValue("@id", ownerId);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    members.Add(new HireableMember
                    {
                        Name = reader.GetString("name"),
                        Strength = reader.GetInt32("strength"),
                        Dexterity = reader.GetInt32("dex"),
                        Intelligence = reader.GetInt32("intelligence"),
                        Experience = reader.GetInt32("experience_points")
                    });
                }
            }

            if (members.Count == 0) return false;

            using (var upd = new MySqlCommand("UPDATE characters SET in_tavern=1 WHERE account_id=@id AND is_dead=0 AND in_arena=0 AND in_tavern=0", conn))
            {
                upd.Parameters.AddWithValue("@id", ownerId);
                upd.ExecuteNonQuery();
            }
            var party = new HireableParty
            {
                OwnerId = ownerId,
                Name = $"{members[0].Name}'s Party",
                Cost = cost,
                Members = members
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
    }
}


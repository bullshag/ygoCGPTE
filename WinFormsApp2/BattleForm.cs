using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public partial class BattleForm : Form
    {
        private readonly List<Creature> _players = new();
        private readonly List<Creature> _npcs = new();
        private readonly Dictionary<Creature, System.Windows.Forms.Timer> _timers = new();
        private readonly Random _rng = new();
        private readonly int _userId;

        public BattleForm(int userId)
        {
            _userId = userId;
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            using var cmd = new MySqlCommand("SELECT name, level, current_hp, max_hp, strength, dex, action_speed, melee_defense, role, targeting_style FROM characters WHERE account_id=@id", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    _players.Add(new Creature
                    {
                        Name = r.GetString("name"),
                        Level = r.GetInt32("level"),
                        CurrentHp = r.GetInt32("current_hp"),
                        MaxHp = r.GetInt32("max_hp"),
                        Strength = r.GetInt32("strength"),
                        Dex = r.GetInt32("dex"),
                        ActionSpeed = r.GetInt32("action_speed"),
                        MeleeDefense = r.GetInt32("melee_defense"),
                        Role = r.GetString("role"),
                        TargetingStyle = r.GetString("targeting_style")
                    });
                }
            }

            int totalLevel = _players.Sum(p => p.Level);
            int targetLevel = (int)Math.Ceiling(totalLevel * 1.25);
            int npcLevel = 0;
            while (npcLevel < targetLevel)
            {
                using var npcCmd = new MySqlCommand("SELECT name, level, current_hp, max_hp, strength, dex, action_speed, melee_defense, role, targeting_style FROM npcs ORDER BY RAND() LIMIT 1", conn);
                using var r2 = npcCmd.ExecuteReader();
                if (r2.Read())
                {
                    var npc = new Creature
                    {
                        Name = r2.GetString("name"),
                        Level = r2.GetInt32("level"),
                        CurrentHp = r2.GetInt32("current_hp"),
                        MaxHp = r2.GetInt32("max_hp"),
                        Strength = r2.GetInt32("strength"),
                        Dex = r2.GetInt32("dex"),
                        ActionSpeed = r2.GetInt32("action_speed"),
                        MeleeDefense = r2.GetInt32("melee_defense"),
                        Role = r2.GetString("role"),
                        TargetingStyle = r2.GetString("targeting_style")
                    };
                    _npcs.Add(npc);
                    npcLevel += npc.Level;
                }
            }
        }

        private void BattleForm_Load(object? sender, EventArgs e)
        {
            UpdateLabels();
            StartTimers();
        }

        private void UpdateLabels()
        {
            lblPlayer.Text = string.Join("\n", _players.Select(p => $"{p.Name}: {p.CurrentHp}/{p.MaxHp} HP"));
            lblNpc.Text = string.Join("\n", _npcs.Select(n => $"{n.Name}: {n.CurrentHp}/{n.MaxHp} HP"));
        }

        private void StartTimers()
        {
            foreach (var p in _players)
            {
                var t = new System.Windows.Forms.Timer();
                t.Interval = (int)(3000 / (p.ActionSpeed + p.Dex / 25.0));
                t.Tick += (s, e) => Act(p, _players, _npcs);
                t.Start();
                _timers[p] = t;
            }
            foreach (var n in _npcs)
            {
                var t = new System.Windows.Forms.Timer();
                t.Interval = (int)(3000 / (n.ActionSpeed + n.Dex / 25.0));
                t.Tick += (s, e) => Act(n, _npcs, _players);
                t.Start();
                _timers[n] = t;
            }
        }

        private void Act(Creature actor, List<Creature> allies, List<Creature> opponents)
        {
            if (actor.CurrentHp <= 0) return;
            if (actor.HealCooldown > 0) actor.HealCooldown--;

            Creature? target;
            if (actor.Role == "Healer" && actor.HealCooldown == 0)
            {
                target = ChooseHealerTarget(actor, allies);
                if (target != null && target.CurrentHp < target.MaxHp)
                {
                    int heal = Math.Max(1, actor.Strength);
                    target.CurrentHp = Math.Min(target.MaxHp, target.CurrentHp + heal);
                    lstLog.Items.Add($"{actor.Name} heals {target.Name} for {heal}!");
                    actor.CurrentTarget = target;
                    actor.HealCooldown = 3;
                    CheckEnd();
                    return;
                }
            }

            target = ChooseOpponent(actor, opponents, allies);
            if (target == null) return;

            int dmg = Math.Max(1, actor.Strength - target.MeleeDefense);
            target.CurrentHp -= dmg;
            lstLog.Items.Add($"{actor.Name} hits {target.Name} for {dmg} damage!");
            target.Threat[actor] = target.Threat.GetValueOrDefault(actor) + dmg;
            target.CurrentTarget = actor;
            actor.CurrentTarget = target;
            CheckEnd();
        }

        private Creature? ChooseHealerTarget(Creature actor, List<Creature> allies)
        {
            var injured = allies.Where(a => a.CurrentHp < a.MaxHp).ToList();
            if (!injured.Any()) return null;
            switch (actor.TargetingStyle)
            {
                case "prioritize lowest health ally":
                    return injured.OrderBy(a => (double)a.CurrentHp / a.MaxHp).First();
                case "prioritize different allies each turn":
                    actor.LastHealedIndex = (actor.LastHealedIndex + 1) % injured.Count;
                    return injured[actor.LastHealedIndex];
                case "prioritize self":
                    return actor.CurrentHp < actor.MaxHp ? actor : injured.First();
                default:
                    return injured.First();
            }
        }

        private Creature? ChooseOpponent(Creature actor, List<Creature> opponents, List<Creature> allies)
        {
            var alive = opponents.Where(o => o.CurrentHp > 0).ToList();
            if (!alive.Any()) return null;
            switch (actor.Role)
            {
                case "Tank":
                    switch (actor.TargetingStyle)
                    {
                        case "prioritize strongest foe":
                            return alive.OrderByDescending(o => o.Strength).First();
                        case "prioritize weakest foe":
                            return alive.OrderBy(o => o.Strength).First();
                        case "prioritize targets that arent attack you":
                            var notOnMe = alive.Where(o => o.CurrentTarget != actor).ToList();
                            return notOnMe.Any() ? notOnMe[_rng.Next(notOnMe.Count)] : alive[_rng.Next(alive.Count)];
                        default:
                            return alive[_rng.Next(alive.Count)];
                    }
                case "DPS":
                    switch (actor.TargetingStyle)
                    {
                        case "prioritize target of the strongest tank":
                            var tank = allies.Where(a => a.Role == "Tank").OrderByDescending(a => a.Strength).FirstOrDefault();
                            if (tank?.CurrentTarget != null) return tank.CurrentTarget;
                            break;
                        case "prioritize targets attacking you":
                            var atkMe = alive.Where(o => o.CurrentTarget == actor).ToList();
                            if (atkMe.Any()) return atkMe[_rng.Next(atkMe.Count)];
                            break;
                        case "prioritize targets that attack non-tanks":
                            var atkNonTank = alive.Where(o => o.CurrentTarget != null && o.CurrentTarget.Role != "Tank").ToList();
                            if (atkNonTank.Any()) return atkNonTank[_rng.Next(atkNonTank.Count)];
                            break;
                    }
                    return alive[_rng.Next(alive.Count)];
                default:
                    return alive[_rng.Next(alive.Count)];
            }
        }

        private void CheckEnd()
        {
            UpdateLabels();
            if (_players.All(p => p.CurrentHp <= 0) || _npcs.All(n => n.CurrentHp <= 0))
            {
                foreach (var t in _timers.Values) t.Stop();
                bool playersWin = _players.Any(p => p.CurrentHp > 0);
                lstLog.Items.Add(playersWin ? "Players win!" : "NPCs win!");
                if (playersWin)
                {
                    AwardExperience(_npcs.Sum(n => n.Level));
                }
            }
        }

        private void AwardExperience(int totalEnemyLevels)
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            int partySize = _players.Count;
            if (partySize <= 0) return;
            int expGain = totalEnemyLevels * 20;
            int expPer = expGain / partySize;
            using var updateCmd = new MySqlCommand("UPDATE characters SET experience_points = experience_points + @exp WHERE account_id=@id", conn);
            updateCmd.Parameters.AddWithValue("@exp", expPer);
            updateCmd.Parameters.AddWithValue("@id", _userId);
            updateCmd.ExecuteNonQuery();
            lstLog.Items.Add($"Each party member gains {expPer} EXP!");
        }

        private class Creature
        {
            public string Name { get; set; } = string.Empty;
            public int MaxHp { get; set; }
            public int CurrentHp { get; set; }
            public int Strength { get; set; }
            public int Dex { get; set; }
            public int ActionSpeed { get; set; }
            public int MeleeDefense { get; set; }
            public int Level { get; set; }
            public string Role { get; set; } = "DPS";
            public string TargetingStyle { get; set; } = "no priorities";
            public Creature? CurrentTarget { get; set; }
            public Dictionary<Creature, int> Threat { get; } = new();
            public int LastHealedIndex { get; set; } = -1;
            public int HealCooldown { get; set; }
        }
    }
}

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
        private readonly System.Windows.Forms.Timer _progressTimer = new System.Windows.Forms.Timer();

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

            using var cmd = new MySqlCommand("SELECT id, name, level, current_hp, max_hp, mana, strength, dex, intelligence, action_speed, melee_defense, role, targeting_style FROM characters WHERE account_id=@id", conn);
            cmd.Parameters.AddWithValue("@id", _userId);

            var playerIds = new Dictionary<Creature, int>();
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    int cid = r.GetInt32("id");
                    var player = new Creature
                    {
                        Name = r.GetString("name"),
                        Level = r.GetInt32("level"),
                        CurrentHp = r.GetInt32("current_hp"),
                        MaxHp = r.GetInt32("max_hp"),
                        Mana = r.GetInt32("mana"),
                        MaxMana = r.GetInt32("mana"),
                        Strength = r.GetInt32("strength"),
                        Dex = r.GetInt32("dex"),
                        Intelligence = r.GetInt32("intelligence"),
                        ActionSpeed = r.GetInt32("action_speed"),
                        MeleeDefense = r.GetInt32("melee_defense"),
                        Role = r.GetString("role"),
                        TargetingStyle = r.GetString("targeting_style")
                    };
                    foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
                    {
                        player.Equipment[slot] = InventoryService.GetEquippedItem(player.Name, slot);
                    }

                    _players.Add(player);
                    playerIds[player] = cid;
                }
            }

            // load abilities after closing the reader to avoid multiple active data readers on the same connection
            foreach (var kv in playerIds)
            {
                foreach (var abil in AbilityService.GetEquippedAbilities(kv.Value, conn))
                {
                    kv.Key.Abilities.Add(abil);
                }
                if (!kv.Key.Abilities.Any())
                {
                    kv.Key.Abilities.Add(new Ability { Id = 0, Name = "-basic attack-", Priority = 1, Cost = 0, Slot = 1 });
                }
            }

            int totalLevel = _players.Sum(p => p.Level);
            int minLevel = (int)Math.Floor(totalLevel * 0.7);
            int maxLevel = (int)Math.Ceiling(totalLevel * 1.2);
            int npcLevel = 0;
            while (npcLevel < minLevel)
            {
                using var npcCmd = new MySqlCommand("SELECT name, level, current_hp, max_hp, strength, dex, action_speed, melee_defense, role, targeting_style FROM npcs ORDER BY RAND() LIMIT 1", conn);
                using var r2 = npcCmd.ExecuteReader();
                if (r2.Read())
                {
                    int level = r2.GetInt32("level");
                    if (npcLevel + level > maxLevel)
                    {
                        continue;
                    }
                    var npc = new Creature
                    {
                        Name = r2.GetString("name"),
                        Level = level,
                        CurrentHp = r2.GetInt32("current_hp"),
                        MaxHp = r2.GetInt32("max_hp"),
                        Strength = r2.GetInt32("strength"),
                        Dex = r2.GetInt32("dex"),
                        ActionSpeed = r2.GetInt32("action_speed"),
                        MeleeDefense = r2.GetInt32("melee_defense"),
                        Role = r2.GetString("role"),
                        TargetingStyle = r2.GetString("targeting_style")
                    };
                    npc.Abilities.Add(new Ability { Id = 0, Name = "-basic attack-", Priority = 1, Cost = 0, Slot = 1 });
                    _npcs.Add(npc);
                    npcLevel += level;
                }
            }
        }

        private void BattleForm_Load(object? sender, EventArgs e)
        {
            BuildPanels();
            StartTimers();
        }

        private void StartTimers()
        {
            foreach (var p in _players)
            {
                var t = new System.Windows.Forms.Timer();
                double speed = p.ActionSpeed + p.Dex / 25.0;
                if (p.GetWeapon() is Weapon w && w.AttackSpeedMod != 0)
                {
                    speed *= (1 + w.AttackSpeedMod);
                }
                t.Interval = (int)(3000 / speed);
                p.AttackInterval = t.Interval;
                p.AttackBar.Maximum = t.Interval;
                p.AttackBar.Value = t.Interval;
                t.Tick += (s, e) => Act(p, _players, _npcs);
                t.Start();
                _timers[p] = t;
            }
            foreach (var n in _npcs)
            {
                var t = new System.Windows.Forms.Timer();
                double speed = n.ActionSpeed + n.Dex / 25.0;
                t.Interval = (int)(3000 / speed);
                n.AttackInterval = t.Interval;
                n.AttackBar.Maximum = t.Interval;
                n.AttackBar.Value = t.Interval;
                t.Tick += (s, e) => Act(n, _npcs, _players);
                t.Start();
                _timers[n] = t;
            }
            _progressTimer.Interval = 100;
            _progressTimer.Tick += (s, e) =>
            {
                foreach (var c in _players.Concat(_npcs))
                {
                    if (c.AttackBar.Value > 0)
                    {
                        c.AttackBar.Value = Math.Max(0, c.AttackBar.Value - 100);
                    }
                }
            };
            _progressTimer.Start();
        }

        private void Act(Creature actor, List<Creature> allies, List<Creature> opponents)
        {
            if (actor.CurrentHp <= 0) return;
            if (actor.HealCooldown > 0) actor.HealCooldown--;

            if (actor.CurrentHp <= actor.MaxHp * 0.35)
            {
                if (actor.Equipment.TryGetValue(EquipmentSlot.LeftHand, out var lh) && lh is HealingPotion pot)
                {
                    actor.CurrentHp = Math.Min(actor.MaxHp, actor.CurrentHp + pot.HealAmount);
                    lstLog.Items.Add($"{actor.Name} uses a healing potion!");
                    InventoryService.ConsumeEquipped(actor.Name, EquipmentSlot.LeftHand);
                    actor.Equipment[EquipmentSlot.LeftHand] = null;
                    return;
                }
                if (actor.Equipment.TryGetValue(EquipmentSlot.RightHand, out var rh) && rh is HealingPotion pot2)
                {
                    actor.CurrentHp = Math.Min(actor.MaxHp, actor.CurrentHp + pot2.HealAmount);
                    lstLog.Items.Add($"{actor.Name} uses a healing potion!");
                    InventoryService.ConsumeEquipped(actor.Name, EquipmentSlot.RightHand);
                    actor.Equipment[EquipmentSlot.RightHand] = null;
                    return;
                }
            }

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

            var ability = ChooseAbility(actor);
            if (ability.Id != 0)
            {
                actor.Mana -= ability.Cost;
                if (actor.ManaBar.Maximum > 0)
                {
                    actor.ManaBar.Value = Math.Max(0, actor.Mana);
                }
                lstLog.Items.Add(GenerateAbilityLog(actor, target, ability));
            }

            int dmg = CalculateDamage(actor, target);
            target.CurrentHp -= dmg;
            lstLog.Items.Add(GenerateAttackLog(actor, target, dmg));
            actor.DamageDone += dmg;
            target.DamageTaken += dmg;
            target.HpBar.Value = Math.Max(0, target.CurrentHp);
            actor.AttackBar.Value = actor.AttackInterval;
            target.Threat[actor] = target.Threat.GetValueOrDefault(actor) + dmg;
            target.CurrentTarget = actor;
            actor.CurrentTarget = target;
            CheckEnd();
        }

        private string GenerateAbilityLog(Creature actor, Creature target, Ability ability)
        {
            string[] verbs = { "casts", "unleashes", "channels", "conjures", "invokes" };
            string verb = verbs[_rng.Next(verbs.Length)];
            return $"The {actor.Role.ToLower()} {actor.Name} {verb} {ability.Name} at {target.Name}!";
        }

        private string GenerateAttackLog(Creature actor, Creature target, int dmg)
        {
            bool isNpc = _npcs.Contains(actor);
            var weapon = actor.GetWeapon();
            string[] verbs;
            if (weapon != null)
            {
                string name = weapon.Name.ToLower();
                if (name.Contains("sword") || name.Contains("dagger") || name.Contains("scythe") || name.Contains("axe"))
                {
                    verbs = new[] { "slashes", "slices", "hacks", "cleaves", "carves into" };
                }
                else if (name.Contains("mace") || name.Contains("maul") || name.Contains("hammer"))
                {
                    verbs = new[] { "smashes", "crushes", "bashes", "pummels" };
                }
                else if (name.Contains("bow"))
                {
                    verbs = new[] { "fires an arrow at", "shoots", "looses a shot at", "pierces" };
                }
                else if (name.Contains("staff") || name.Contains("wand") || name.Contains("rod"))
                {
                    verbs = new[] { "zaps", "scorches", "blasts", "unleashes magic at" };
                }
                else
                {
                    verbs = new[] { "strikes", "hits", "attacks" };
                }
            }
            else
            {
                if (isNpc)
                {
                    verbs = new[] { "claws at", "bites", "mauls", "rips into", "gnashes teeth at" };
                }
                else
                {
                    verbs = new[] { "punches", "kicks", "strikes" };
                }
            }

            string verb = verbs[_rng.Next(verbs.Length)];
            return $"The {actor.Role.ToLower()} {actor.Name} {verb} {target.Name} for {dmg} damage!";
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

        private Ability ChooseAbility(Creature actor)
        {
            var abilities = actor.Abilities;
            var basic = abilities.FirstOrDefault(a => a.Id == 0) ?? new Ability { Id = 0, Name = "-basic attack-", Priority = 1, Cost = 0 };
            Ability? chosen = null;
            var usable = abilities.Where(a => a.Id == 0 || actor.Mana >= a.Cost).ToList();
            var nonBasic = usable.Where(a => a.Id != 0).ToList();
            if (nonBasic.Any())
            {
                int minPriority = nonBasic.Min(a => a.Priority);
                int basicPriority = basic.Priority;
                if (minPriority < basicPriority)
                {
                    var top = nonBasic.Where(a => a.Priority == minPriority).ToList();
                    chosen = top[_rng.Next(top.Count)];
                }
            }
            if (chosen == null)
            {
                var weighted = new List<(Ability ability, int weight)>();
                foreach (var a in usable)
                {
                    int weight = Math.Max(1, 6 - a.Priority);
                    weighted.Add((a, weight));
                }
                int total = weighted.Sum(w => w.weight);
                int roll = _rng.Next(total);
                int acc = 0;
                foreach (var w in weighted)
                {
                    acc += w.weight;
                    if (roll < acc)
                    {
                        chosen = w.ability;
                        break;
                    }
                }
            }
            return chosen ?? basic;
        }

        private void CheckEnd()
        {
            foreach (var p in _players) p.HpBar.Value = Math.Max(0, p.CurrentHp);
            foreach (var n in _npcs) n.HpBar.Value = Math.Max(0, n.CurrentHp);
            if (_players.All(p => p.CurrentHp <= 0) || _npcs.All(n => n.CurrentHp <= 0))
            {
                foreach (var t in _timers.Values) t.Stop();
                bool playersWin = _players.Any(p => p.CurrentHp > 0);
                lstLog.Items.Add(playersWin ? "Players win!" : "NPCs win!");
                if (playersWin)
                {
                    AwardExperience(_npcs.Sum(n => n.Level));
                    var loot = LootService.GenerateLoot(_npcs.Select(n => n.Name), _userId);
                    if (loot.Count > 0)
                    {
                        var parts = new List<string>();
                        if (loot.TryGetValue("gold", out int gold)) parts.Add($"{gold} gold");
                        foreach (var kv in loot.Where(k => k.Key != "gold")) parts.Add($"{kv.Value} {kv.Key}");
                        if (parts.Count > 0) lstLog.Items.Add("Loot: " + string.Join(", ", parts));
                    }
                }
                BattleLogService.AddLog(string.Join("\n", lstLog.Items.Cast<string>()));
                SaveHp();
                var playerSummaries = _players.Select(p => new CombatantSummary(p.Name, p.DamageDone, p.DamageTaken));
                var enemySummaries = _npcs.Select(n => new CombatantSummary(n.Name, n.DamageDone, n.DamageTaken));
                using var summary = new BattleSummaryForm(playerSummaries, enemySummaries);
                Hide();
                summary.ShowDialog(this.Owner);
                Close();
            }
        }

        private int CalculateDamage(Creature actor, Creature target)
        {
            var weapon = actor.GetWeapon();
            double statTotal = actor.Strength;
            double min = 0.8, max = 1.2;
            double critChanceBonus = 0, critDamageBonus = 0;
            if (weapon != null)
            {
                statTotal = actor.Strength * weapon.StrScaling + actor.Dex * weapon.DexScaling + actor.Intelligence * weapon.IntScaling;
                min = weapon.MinMultiplier;
                max = weapon.MaxMultiplier;
                critChanceBonus = weapon.CritChanceBonus;
                critDamageBonus = weapon.CritDamageBonus;
            }
            double mult = min + _rng.NextDouble() * (max - min);
            int dmg = (int)Math.Max(1, statTotal * mult - target.MeleeDefense);
            double critChance = 0.05 + actor.Dex / 5 * 0.01 + critChanceBonus;
            if (_rng.NextDouble() < Math.Min(1.0, critChance))
            {
                dmg = (int)(dmg * (1.5 + critDamageBonus));
            }
            return dmg;
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
            public int Mana { get; set; }
            public int MaxMana { get; set; }
            public int Strength { get; set; }
            public int Dex { get; set; }
            public int Intelligence { get; set; }
            public int ActionSpeed { get; set; }
            public int MeleeDefense { get; set; }
            public int Level { get; set; }
            public string Role { get; set; } = "DPS";
            public string TargetingStyle { get; set; } = "no priorities";
            public Creature? CurrentTarget { get; set; }
            public Dictionary<Creature, int> Threat { get; } = new();
            public int LastHealedIndex { get; set; } = -1;
            public int HealCooldown { get; set; }
            public Dictionary<EquipmentSlot, Item?> Equipment { get; } = new();
            public ProgressBar HpBar { get; set; } = new();
            public ProgressBar ManaBar { get; set; } = new();
            public ProgressBar AttackBar { get; set; } = new();
            public int AttackInterval { get; set; }
            public int DamageDone { get; set; }
            public int DamageTaken { get; set; }
            public List<Ability> Abilities { get; } = new();

            public Weapon? GetWeapon()
            {
                if (Equipment.TryGetValue(EquipmentSlot.LeftHand, out var lh) && lh is Weapon w) return w;
                if (Equipment.TryGetValue(EquipmentSlot.RightHand, out var rh) && rh is Weapon w2) return w2;
                return null;
            }
        }

        private void BuildPanels()
        {
            pnlPlayers.Controls.Clear();
            pnlEnemies.Controls.Clear();
            foreach (var p in _players)
            {
                pnlPlayers.Controls.Add(CreatePanel(p));
            }
            foreach (var e in _npcs)
            {
                pnlEnemies.Controls.Add(CreatePanel(e));
            }
        }

        private Control CreatePanel(Creature c)
        {
            var panel = new Panel { Width = 180, Height = 80 };
            var lbl = new Label { Text = c.Name, AutoSize = true };
            c.HpBar = new ProgressBar { Maximum = c.MaxHp, Value = c.CurrentHp, Width = 170, Location = new System.Drawing.Point(0, 15) };
            panel.Controls.Add(lbl);
            panel.Controls.Add(c.HpBar);
            if (c.MaxMana > 0)
            {
                c.ManaBar = new ProgressBar { Maximum = c.MaxMana, Value = c.Mana, Width = 170, Location = new System.Drawing.Point(0, 35) };
                panel.Controls.Add(c.ManaBar);
                c.AttackBar = new ProgressBar { Maximum = 100, Value = 100, Width = 170, Location = new System.Drawing.Point(0, 55) };
            }
            else
            {
                c.AttackBar = new ProgressBar { Maximum = 100, Value = 100, Width = 170, Location = new System.Drawing.Point(0, 35) };
            }
            panel.Controls.Add(c.AttackBar);
            return panel;
        }

        private void SaveHp()
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            foreach (var p in _players)
            {
                using MySqlCommand cmd = new MySqlCommand("UPDATE characters SET current_hp=@hp WHERE account_id=@uid AND name=@name", conn);
                cmd.Parameters.AddWithValue("@hp", Math.Max(0, p.CurrentHp));
                cmd.Parameters.AddWithValue("@uid", _userId);
                cmd.Parameters.AddWithValue("@name", p.Name);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using WinFormsApp2.Multiplayer;

namespace WinFormsApp2
{
    public partial class BattleForm : Form
    {
        private readonly List<Creature> _players = new();
        private readonly List<Creature> _npcs = new();
        private readonly Dictionary<Creature, System.Windows.Forms.Timer> _timers = new();
        private readonly Dictionary<Creature, int> _playerIds = new();
        private readonly HashSet<int> _mercenaryIds = new();
        private readonly Random _rng = new();
        private readonly int _userId;
        private readonly System.Windows.Forms.Timer _gameTimer = new System.Windows.Forms.Timer();
        private readonly Dictionary<string, string> _deathCauses = new();
        private readonly bool _wildEncounter;
        private readonly bool _arenaBattle;
        private readonly int? _arenaOpponentId;
        private readonly int? _areaMinPower;
        private readonly int? _areaMaxPower;
        private readonly bool _darkSpireBattle;
        private readonly string? _areaId;
        private int _opponentAccountId;
        private bool _cancelled;
        private bool _playersWin;
        private bool _battleEnded;

        public bool PlayersWin => _playersWin;
        public bool Cancelled => _cancelled;
        public int OpponentAccountId => _opponentAccountId;
        public IReadOnlyList<string> LogLines => lstLog.Items.Cast<LogEntry>().Select(l => l.Text).ToList();

        private class LogEntry
        {
            public string Text { get; }
            public Color Color { get; }
            public LogEntry(string text, Color color)
            {
                Text = text;
                Color = color;
            }
            public override string ToString() => Text;
        }

        private void AppendLog(string text, bool isPlayerAction, bool isHeal = false)
        {
            Color color = isHeal ? Color.Green : (isPlayerAction ? Color.Blue : Color.Red);
            lstLog.Items.Add(new LogEntry(text, color));
            lstLog.SelectedIndex = lstLog.Items.Count - 1;
        }

        public BattleForm(int userId, bool wildEncounter = false, bool arenaBattle = false, int? arenaOpponentId = null, int? areaMinPower = null, int? areaMaxPower = null, bool darkSpireBattle = false, string? areaId = null)
        {
            _userId = userId;
            _wildEncounter = wildEncounter;
            _arenaBattle = arenaBattle;
            _arenaOpponentId = arenaOpponentId;
            _areaMinPower = areaMinPower;
            _areaMaxPower = areaMaxPower;
            _darkSpireBattle = darkSpireBattle;
            _areaId = areaId;
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            string playerQuery = _arenaBattle
                ? "SELECT id, name, level, current_hp, max_hp, mana, strength, dex, intelligence, action_speed, melee_defense, magic_defense, role, targeting_style, is_mercenary FROM characters WHERE account_id=@id AND is_dead=0 AND in_arena=1 AND in_tavern=0"
                : "SELECT id, name, level, current_hp, max_hp, mana, strength, dex, intelligence, action_speed, melee_defense, magic_defense, role, targeting_style, is_mercenary FROM characters WHERE account_id=@id AND is_dead=0 AND in_arena=0 AND in_tavern=0";
            using var cmd = new MySqlCommand(playerQuery, conn);
            cmd.Parameters.AddWithValue("@id", _userId);

            _playerIds.Clear();
            _mercenaryIds.Clear();
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    int cid = r.GetInt32("id");
                    bool merc = r.GetBoolean("is_mercenary");
                    int intelligence = r.GetInt32("intelligence");
                    var player = new Creature
                    {
                        Name = r.GetString("name"),
                        Level = r.GetInt32("level"),
                        CurrentHp = r.GetInt32("current_hp"),
                        MaxHp = r.GetInt32("max_hp"),
                        Mana = r.GetInt32("mana"),
                        MaxMana = 10 + 5 * intelligence,
                        Strength = r.GetInt32("strength"),
                        Dex = r.GetInt32("dex"),
                        Intelligence = intelligence,
                        ActionSpeed = r.GetInt32("action_speed"),
                        MeleeDefense = r.GetInt32("melee_defense"),
                        MagicDefense = r.GetInt32("magic_defense"),
                        Role = r.GetString("role"),
                        TargetingStyle = r.GetString("targeting_style")
                    };
                    foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
                    {
                        player.Equipment[slot] = InventoryService.GetEquippedItem(player.Name, slot);
                    }
                    ApplyEquipmentBonuses(player);

                    _players.Add(player);
                    _playerIds[player] = cid;
                    if (merc) _mercenaryIds.Add(cid);
                }
            }

            // load abilities after closing the reader to avoid multiple active data readers on the same connection
            foreach (var kv in _playerIds)
            {
                foreach (var abil in AbilityService.GetEquippedAbilities(kv.Value, conn))
                {
                    kv.Key.Abilities.Add(abil);
                }
                if (!kv.Key.Abilities.Any())
                {
                    kv.Key.Abilities.Add(new Ability { Id = 0, Name = "-basic attack-", Priority = 1, Cost = 0, Slot = 1 });
                }
                foreach (var p in PassiveService.GetOwnedPassives(kv.Value, conn))
                    kv.Key.Passives[p.Name] = p.Level;
                ApplyPassiveModifiers(kv.Key);
                int eqCost = kv.Key.Equipment.Values.Sum(i => i?.Price ?? 0);
                kv.Key.Power = PowerCalculator.CalculatePower(kv.Key.Level, eqCost, kv.Key.Abilities.Count);
            }

            if (_arenaBattle)
            {
                LoadArenaOpponents(conn, _arenaOpponentId);
                return;
            }

            int playerPower = _players.Sum(p => p.Power);
            int avgPower = _players.Count > 0 ? (int)Math.Ceiling(playerPower / (double)_players.Count) : 1;

            int areaMin = _areaMinPower ?? 1;
            int areaMax = _areaMaxPower ?? int.MaxValue;

            int effectivePlayerPower = playerPower;
            if (playerPower < areaMin && _areaMaxPower.HasValue)
                effectivePlayerPower = areaMax;

            int minTotal = (int)Math.Ceiling(effectivePlayerPower * 0.8);
            int maxTotal = _wildEncounter ? (int)Math.Ceiling(effectivePlayerPower * 1.0)
                                          : (int)Math.Ceiling(effectivePlayerPower * 1.2);

            int targetAvg = playerPower < areaMin && _areaMaxPower.HasValue ? areaMax : avgPower;

            int avgLevel = _players.Count > 0 ? (int)Math.Ceiling(_players.Average(p => p.Level)) : 1;

            // NPC party power ranges from roughly 60% to 100% of the party's average party power,
            // while still respecting any area power restrictions.
            int perNpcMin = (int)(avgLevel * 0.8);
            int perNpcMax = (int)(avgLevel * 1.2);

            if (_areaMinPower.HasValue)
            {
                perNpcMin = Math.Max(perNpcMin, areaMin);
                perNpcMax = Math.Max(perNpcMax, areaMin);
            }
            if (_areaMaxPower.HasValue)
            {
                perNpcMin = Math.Min(perNpcMin, areaMax);
                perNpcMax = Math.Min(perNpcMax, areaMax);
            }
            if (perNpcMin > perNpcMax)
                perNpcMin = perNpcMax = Math.Min(areaMax, perNpcMin);

            int npcPower = 0;

            Creature? AddNpc(int minPower, int maxPower)
            {
                for (int attempt = 0; attempt < 50; attempt++)
                {
                    using var npcCmd = new MySqlCommand("SELECT name, level, current_hp, max_hp, mana, strength, dex, intelligence, action_speed, melee_defense, magic_defense, role, targeting_style FROM npcs ORDER BY RAND() LIMIT 1", conn);
                    using var r2 = npcCmd.ExecuteReader();
                    if (!r2.Read())
                        return null;

                    string name = r2.GetString("name");
                    int level = r2.GetInt32("level");
                    int currentHp = r2.GetInt32("current_hp");
                    int maxHp = r2.GetInt32("max_hp");
                    int mana = r2.GetInt32("mana");
                    int strength = r2.GetInt32("strength");
                    int dex = r2.GetInt32("dex");
                    int intelligence = r2.GetInt32("intelligence");
                    int action = r2.GetInt32("action_speed");
                    int meleeDef = r2.GetInt32("melee_defense");
                    int magicDef = r2.GetInt32("magic_defense");
                    string role = r2.GetString("role");
                    string style = r2.GetString("targeting_style");
                    r2.Close();

                    var npc = new Creature
                    {
                        Name = name,
                        Level = level,
                        CurrentHp = currentHp,
                        MaxHp = maxHp,
                        Mana = mana,
                        MaxMana = 10 + 5 * intelligence,
                        Strength = strength,
                        Dex = dex,
                        Intelligence = intelligence,
                        ActionSpeed = action,
                        MeleeDefense = meleeDef,
                        MagicDefense = magicDef,
                        Role = role,
                        TargetingStyle = style
                    };

                    foreach (var kv in InventoryService.GetNpcEquipment(name, level))
                        npc.Equipment[kv.Key] = kv.Value;
                    ApplyEquipmentBonuses(npc);

                    using var abilCmd = new MySqlCommand(@"SELECT slot, priority, a.id, a.name, a.description, a.cost, a.cooldown
                                                      FROM npc_abilities na
                                                      JOIN abilities a ON na.ability_id = a.id
                                                      WHERE na.npc_name=@n", conn);
                    abilCmd.Parameters.AddWithValue("@n", name);
                    using var abilR = abilCmd.ExecuteReader();
                    while (abilR.Read())
                    {
                        npc.Abilities.Add(new Ability
                        {
                            Slot = abilR.GetInt32("slot"),
                            Priority = abilR.GetInt32("priority"),
                            Id = abilR.GetInt32("id"),
                            Name = abilR.GetString("name"),
                            Description = abilR.GetString("description"),
                            Cost = abilR.GetInt32("cost"),
                            Cooldown = abilR.GetInt32("cooldown")
                        });
                    }
                    if (!npc.Abilities.Any())
                        npc.Abilities.Add(new Ability { Id = 0, Name = "-basic attack-", Priority = 1, Cost = 0, Slot = 1 });

                    int eqCostNpc = npc.Equipment.Values.Sum(i => i?.Price ?? 0);
                    int power = PowerCalculator.CalculatePower(npc.Level, eqCostNpc, npc.Abilities.Count);
                    if (power < minPower || power > maxPower)
                        continue;
                    npc.Power = power;
                    _npcs.Add(npc);
                    npcPower += power;
                    return npc;
                }
                return null;
            }

            // Start with a foe at the upper end of the allowed range.
            int strongMin = perNpcMax;
            int strongMax = perNpcMax;
            AddNpc(strongMin, strongMax);

            int weakerCount = _rng.Next(1, 3);
            for (int i = 0; i < weakerCount && npcPower < maxTotal; i++)
            {
                int remaining = maxTotal - npcPower;
                int weakMax = Math.Min(targetAvg, remaining);
                if (weakMax < perNpcMin) break;
                if (AddNpc(perNpcMin, weakMax) == null)
                    break;
            }

            while (npcPower < minTotal)
            {
                int remaining = maxTotal - npcPower;
                if (remaining < perNpcMin) break;
                if (AddNpc(perNpcMin, Math.Min(targetAvg, remaining)) == null)
                    break;
            }

            if (_npcs.Count == 0)
            {
                AddNpc(perNpcMin, perNpcMax);
            }
        }

        private void LoadArenaOpponents(MySqlConnection conn, int? specificOpponent)
        {
            int oppId;
            if (specificOpponent.HasValue)
            {
                oppId = specificOpponent.Value;
            }
            else
            {
                using var oppCmd = new MySqlCommand("SELECT account_id FROM arena_teams WHERE account_id!=@id ORDER BY RAND() LIMIT 1", conn);
                oppCmd.Parameters.AddWithValue("@id", _userId);
                object? opp = oppCmd.ExecuteScalar();
                if (opp == null)
                {
                    _cancelled = true;
                    return;
                }
                oppId = Convert.ToInt32(opp);
            }
            _opponentAccountId = oppId;
            InventoryService.Load(oppId);
            using var cmd = new MySqlCommand("SELECT id, name, level, current_hp, max_hp, mana, strength, dex, intelligence, action_speed, melee_defense, magic_defense, role, targeting_style FROM characters WHERE account_id=@aid AND is_dead=0 AND in_arena=1 AND in_tavern=0", conn);
            cmd.Parameters.AddWithValue("@aid", oppId);
            var npcIds = new Dictionary<Creature, int>();
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    int cid = r.GetInt32("id");
                    int intel = r.GetInt32("intelligence");
                    var npc = new Creature
                    {
                        Name = r.GetString("name"),
                        Level = r.GetInt32("level"),
                        CurrentHp = r.GetInt32("current_hp"),
                        MaxHp = r.GetInt32("max_hp"),
                        Mana = r.GetInt32("mana"),
                        MaxMana = 10 + 5 * intel,
                        Strength = r.GetInt32("strength"),
                        Dex = r.GetInt32("dex"),
                        Intelligence = intel,
                        ActionSpeed = r.GetInt32("action_speed"),
                        MeleeDefense = r.GetInt32("melee_defense"),
                        MagicDefense = r.GetInt32("magic_defense"),
                        Role = r.GetString("role"),
                        TargetingStyle = r.GetString("targeting_style")
                    };
                    foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
                        npc.Equipment[slot] = InventoryService.GetEquippedItem(npc.Name, slot);
                    ApplyEquipmentBonuses(npc);
                    _npcs.Add(npc);
                    npcIds[npc] = cid;
                }
            }
            foreach (var kv in npcIds)
            {
                foreach (var abil in AbilityService.GetEquippedAbilities(kv.Value, conn))
                    kv.Key.Abilities.Add(abil);
                if (!kv.Key.Abilities.Any())
                    kv.Key.Abilities.Add(new Ability { Id = 0, Name = "-basic attack-", Priority = 1, Cost = 0, Slot = 1 });
                foreach (var p in PassiveService.GetOwnedPassives(kv.Value, conn))
                    kv.Key.Passives[p.Name] = p.Level;
                ApplyPassiveModifiers(kv.Key);
                int eqCostNpc = kv.Key.Equipment.Values.Sum(i => i?.Price ?? 0);
                kv.Key.Power = PowerCalculator.CalculatePower(kv.Key.Level, eqCostNpc, kv.Key.Abilities.Count);
            }
            InventoryService.Load(_userId);
            if (_npcs.Count == 0)
                _cancelled = true;
        }

        private void BattleForm_Load(object? sender, EventArgs e)
        {
            if (_cancelled)
            {
                Close();
                return;
            }
            BuildPanels();
            StartTimers();
        }

        private void StartTimers()
        {
            foreach (var p in _players)
            {
                double speed = p.ActionSpeed * (1 + p.Dex / 1000.0) * p.AttackSpeedMultiplier;
                Weapon? left = p.Equipment.GetValueOrDefault(EquipmentSlot.LeftHand) as Weapon;
                Weapon? right = p.Equipment.GetValueOrDefault(EquipmentSlot.RightHand) as Weapon;
                if (left != null) speed *= (1 + left.AttackSpeedMod);
                if (right != null) speed *= (1 + right.AttackSpeedMod);
                if (left != null && right != null && left.Name != right.Name) speed *= 1.5;
                p.AttackInterval = (int)(3000 / speed);
                p.AttackBar.Maximum = p.AttackInterval;
                p.AttackBar.Value = p.AttackInterval;
                p.NextActionMs = p.AttackInterval;
            }
            foreach (var n in _npcs)
            {
                double speed = n.ActionSpeed * (1 + n.Dex / 1000.0) * n.AttackSpeedMultiplier;
                n.AttackInterval = (int)(3000 / speed);
                n.AttackBar.Maximum = n.AttackInterval;
                n.AttackBar.Value = n.AttackInterval;
                n.NextActionMs = n.AttackInterval;
            }
            _gameTimer.Interval = 100;
            _gameTimer.Tick += (s, e) =>
            {
                foreach (var c in _players.Concat(_npcs))
                {
                    c.NextActionMs = Math.Max(0, c.NextActionMs - 100);
                    if (c.NextActionMs == 0)
                    {
                        if (_players.Contains(c))
                            Act(c, _players, _npcs);
                        else
                            Act(c, _npcs, _players);
                        c.NextActionMs = c.AttackInterval;
                    }
                    c.AttackBar.Value = Math.Min(c.AttackBar.Maximum, c.NextActionMs);

                    var cdKeys = c.Cooldowns.Keys.ToList();
                    foreach (var k in cdKeys)
                    {
                        if (c.Cooldowns[k] > 0)
                        {
                            c.Cooldowns[k] = Math.Max(0, c.Cooldowns[k] - 100);
                        }
                    }

                    for (int i = c.Effects.Count - 1; i >= 0; i--)
                    {
                        var eff = c.Effects[i];
                        eff.RemainingMs -= 100;
                        eff.TimeUntilTickMs -= 100;
                        if (eff.TimeUntilTickMs <= 0)
                        {
                            if (eff.Kind == EffectKind.HoT)
                            {
                                c.CurrentHp = Math.Min(c.MaxHp, c.CurrentHp + eff.AmountPerTick);
                                if (eff.ManaPerTick > 0)
                                {
                                    c.Mana = Math.Min(c.MaxMana, c.Mana + eff.ManaPerTick);
                                    UpdateManaBar(c);
                                }
                                if (!string.IsNullOrEmpty(eff.SourceName))
                                    AppendLog($"{eff.SourceName} heals {c.Name} for {eff.AmountPerTick}.", eff.SourceIsPlayer, true);
                                else
                                    AppendLog($"{c.Name} is healed for {eff.AmountPerTick}.", eff.SourceIsPlayer, true);
                            }
                            else if (eff.Kind == EffectKind.Bleed || eff.Kind == EffectKind.Poison)
                            {
                                int tick = eff.AmountPerTick;
                                ApplyShieldReduction(c, ref tick);
                                c.CurrentHp -= tick;
                                if (c.CurrentHp <= 0)
                                {
                                    if (!TryCheatDeath(c) && _players.Contains(c) && !_deathCauses.ContainsKey(c.Name))
                                    {
                                        _deathCauses[c.Name] = $"{c.Name} succumbed to {eff.Kind}.";
                                    }
                                }
                                AppendLog($"{c.Name} takes {tick} {eff.Kind.ToString().ToLower()} damage.", eff.SourceIsPlayer);
                                c.HpBar.Value = Math.Min(c.HpBar.Maximum, Math.Max(0, c.CurrentHp));
                                eff.TimeUntilTickMs += eff.TickIntervalMs;
                            }
                        }
                        if (eff.RemainingMs <= 0)
                        {
                            c.Effects.RemoveAt(i);
                            if (eff.Kind == EffectKind.Shield)
                            {
                                c.ShieldBar.Visible = false;
                                c.ShieldBar.Value = 0;
                                c.ShieldBar.Maximum = 1;
                            }
                            else if (eff.Kind == EffectKind.DamageDown)
                            {
                                double factor = 1 - eff.AmountPerTick / 100.0;
                                c.DamageDealtMultiplier /= factor;
                            }
                        }
                    }
                    if (c.ForcedTargetMs > 0)
                    {
                        c.ForcedTargetMs = Math.Max(0, c.ForcedTargetMs - 100);
                        if (c.ForcedTargetMs == 0) c.ForcedTarget = null;
                    }
                    if (c.IsVanished)
                    {
                        c.VanishRemainingMs = Math.Max(0, c.VanishRemainingMs - 100);
                        if (c.VanishRemainingMs == 0)
                        {
                            c.IsVanished = false;
                            AppendLog($"{c.Name} reappears.", _players.Contains(c));
                        }
                    }
                }
                CheckEnd();
            };
            _gameTimer.Start();
        }

        private void Act(Creature actor, List<Creature> allies, List<Creature> opponents)
        {
            if (actor.CurrentHp <= 0 || actor.IsVanished) return;
            if (actor.HealCooldown > 0) actor.HealCooldown--;

            if (actor.CurrentHp <= actor.MaxHp * 0.35)
            {
                if (actor.Equipment.TryGetValue(EquipmentSlot.LeftHand, out var lh) && lh is HealingPotion pot)
                {
                    actor.CurrentHp = Math.Min(actor.MaxHp, actor.CurrentHp + pot.HealAmount);
                    AppendLog($"{actor.Name} uses a healing potion!", _players.Contains(actor), true);
                    InventoryService.ConsumeEquipped(actor.Name, EquipmentSlot.LeftHand);
                    actor.Equipment[EquipmentSlot.LeftHand] = null;
                    return;
                }
                if (actor.Equipment.TryGetValue(EquipmentSlot.RightHand, out var rh) && rh is HealingPotion pot2)
                {
                    actor.CurrentHp = Math.Min(actor.MaxHp, actor.CurrentHp + pot2.HealAmount);
                    AppendLog($"{actor.Name} uses a healing potion!", _players.Contains(actor), true);
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
                    AppendLog($"{actor.Name} heals {target.Name} for {heal}!", _players.Contains(actor), true);
                    actor.CurrentTarget = target;
                    actor.HealCooldown = 3;
                    CheckEnd();
                    return;
                }
            }

            var ability = ChooseAbility(actor);
            bool isHealAbility = ability.Name == "Regenerate" || ability.Name == "Heal" || ability.Name == "Rejuvenate" || ability.Name == "Healing Wave" || ability.Name == "Chain Heal" || ability.Name == "Prayer of Healing" || ability.Name == "Holy Light";
            if (isHealAbility)
            {
                if (actor.Role == "Healer")
                {
                    target = ChooseHealerTarget(actor, allies);
                    if (target == null)
                    {
                        if (ability.Name == "Healing Wave" || ability.Name == "Prayer of Healing") target = actor; else return;
                    }
                }
                else
                {
                    target = actor;
                }
            }
            else
            {
                target = ChooseOpponent(actor, opponents, allies);
                if (target == null) return;
            }

            if (ability.Id != 0)
            {
                int cost = (int)(ability.Cost * actor.AbilityCostMultiplier);
                actor.Mana = Math.Max(0, Math.Min(actor.MaxMana, actor.Mana - cost));
                UpdateManaBar(actor);
                actor.Cooldowns[ability.Id] = (int)(ability.Cooldown * actor.CooldownMultiplier * 1000);
                bool actorIsPlayer = _players.Contains(actor);
                AppendLog(GenerateAbilityLog(actor, target, ability), actorIsPlayer, isHealAbility);
                if (ability.Name == "Bleed") ApplyBleed(actor, target);
                else if (ability.Name == "Poison") ApplyPoison(actor, target);
                else if (ability.Name == "Curse") { ApplyCurse(actor, target); actor.AttackBar.Value = actor.AttackInterval; CheckEnd(); return; }
                else if (ability.Name == "Regenerate") { ApplyHot(actor, target); CheckEnd(); return; }
                else if (ability.Name == "Rejuvenate") { ApplyRejuvenate(actor, target); CheckEnd(); return; }
                else if (ability.Name == "Heal")
                {
                    int healAmt = (int)Math.Max(1, (5 + actor.Level + actor.Intelligence * 1.2) * actor.HealingDealtMultiplier);
                    healAmt = (int)(healAmt * target.HealingReceivedMultiplier);
                    target.CurrentHp = Math.Min(target.MaxHp, target.CurrentHp + healAmt);
                    target.HpBar.Value = Math.Min(target.MaxHp, target.CurrentHp);
                    AppendLog($"{actor.Name} restores {healAmt} HP to {target.Name}!", _players.Contains(actor), true);
                    AfterHeal(actor, target, healAmt);
                    actor.AttackBar.Value = actor.AttackInterval;
                    CheckEnd();
                    return;
                }
                else if (ability.Name == "Healing Wave")
                {
                    int healAmt = (int)Math.Max(1, (4 + actor.Level + actor.Intelligence * 1.0) * actor.HealingDealtMultiplier);
                    foreach (var ally in allies.Where(a => a.CurrentHp > 0))
                    {
                        int final = (int)(healAmt * ally.HealingReceivedMultiplier);
                        ally.CurrentHp = Math.Min(ally.MaxHp, ally.CurrentHp + final);
                        ally.HpBar.Value = Math.Min(ally.MaxHp, ally.CurrentHp);
                        AfterHeal(actor, ally, final);
                    }
                    AppendLog($"{actor.Name}'s healing wave restores {healAmt} HP to all allies!", _players.Contains(actor), true);
                    actor.AttackBar.Value = actor.AttackInterval;
                    CheckEnd();
                    return;
                }
                else if (ability.Name == "Chain Heal")
                {
                    int healAmt = (int)Math.Max(1, (5 + actor.Level + actor.Intelligence * 1.0) * actor.HealingDealtMultiplier);
                    healAmt = (int)(healAmt * target.HealingReceivedMultiplier);
                    target.CurrentHp = Math.Min(target.MaxHp, target.CurrentHp + healAmt);
                    target.HpBar.Value = Math.Min(target.MaxHp, target.CurrentHp);
                    AfterHeal(actor, target, healAmt);
                    var others = allies.Where(a => a != target && a.CurrentHp > 0 && a.CurrentHp < a.MaxHp).ToList();
                    if (others.Any())
                    {
                        var second = others[_rng.Next(others.Count)];
                        int heal2 = (int)(healAmt * 0.5 * second.HealingReceivedMultiplier);
                        second.CurrentHp = Math.Min(second.MaxHp, second.CurrentHp + heal2);
                        second.HpBar.Value = Math.Min(second.MaxHp, second.CurrentHp);
                        AfterHeal(actor, second, heal2);
                    }
                    actor.AttackBar.Value = actor.AttackInterval;
                    CheckEnd();
                    return;
                }
                else if (ability.Name == "Prayer of Healing")
                {
                    int healAmt = (int)Math.Max(1, (6 + actor.Level + actor.Intelligence * 0.8) * actor.HealingDealtMultiplier);
                    foreach (var ally in allies.Where(a => a.CurrentHp > 0))
                    {
                        int final = (int)(healAmt * ally.HealingReceivedMultiplier);
                        ally.CurrentHp = Math.Min(ally.MaxHp, ally.CurrentHp + final);
                        ally.HpBar.Value = Math.Min(ally.MaxHp, ally.CurrentHp);
                        AfterHeal(actor, ally, final);
                    }
                    actor.AttackBar.Value = actor.AttackInterval;
                    CheckEnd();
                    return;
                }
                else if (ability.Name == "Holy Light")
                {
                    int healAmt = (int)Math.Max(1, (8 + actor.Level + actor.Intelligence * 1.5) * actor.HealingDealtMultiplier);
                    healAmt = (int)(healAmt * target.HealingReceivedMultiplier);
                    target.CurrentHp = Math.Min(target.MaxHp, target.CurrentHp + healAmt);
                    target.HpBar.Value = Math.Min(target.MaxHp, target.CurrentHp);
                    AfterHeal(actor, target, healAmt);
                    actor.AttackBar.Value = actor.AttackInterval;
                    CheckEnd();
                    return;
                }
                else if (ability.Name == "Taunting Blows") ApplyTaunt(actor, opponents);
                else if (ability.Name == "Vanish") { actor.IsVanished = true; actor.VanishRemainingMs = 5000; actor.AttackBar.Value = actor.AttackInterval; CheckEnd(); return; }

                else if (ability.Name == "Arcane Shield")
                {
                    int shield = (int)Math.Max(1, 5 + actor.Intelligence * 1.5);
                    ApplyShield(actor, target, shield, 15000);
                    actor.AttackBar.Value = actor.AttackInterval;
                    CheckEnd();
                    return;
                }
                else if (ability.Name == "Fortify")
                {
                    int shield = (int)Math.Max(1, 3 + actor.Intelligence * 0.6);
                    ApplyShield(actor, target, shield, 8000);

                    actor.AttackBar.Value = actor.AttackInterval;
                    CheckEnd();
                    return;
                }
                else if (ability.Name == "Guardian Ward")
                {
                    int shield = (int)Math.Max(1, 4 + actor.Intelligence * 1.2);
                    foreach (var ally in allies.Where(a => a.CurrentHp > 0))
                    {
                        ApplyShield(actor, ally, shield, 15000);
                    }
                    actor.AttackBar.Value = actor.AttackInterval;
                    CheckEnd();
                    return;
                }
                else if (ability.Name == "Divine Aegis") { ApplyShield(actor, target, 2.5); actor.AttackBar.Value = actor.AttackInterval; CheckEnd(); return; }

                else if (ability.Name == "Shockwave" || ability.Name == "Frost Nova" || ability.Name == "Earthquake" || ability.Name == "Meteor" || ability.Name == "Flame Strike" || ability.Name == "Arcane Blast")
                {
                    foreach (var o in opponents.Where(o => o.CurrentHp > 0))
                    {
                        if (o.SpellDodgeChance > 0 && _rng.NextDouble() < o.SpellDodgeChance)
                        {
                            AppendLog($"{o.Name} deflects the spell!", actorIsPlayer);
                            continue;
                        }
                        int damage = CalculateSpellDamage(actor, o, ability);
                        if (o.DamageReductionCurrent > 0)
                        {
                            damage = (int)(damage * (1 - o.DamageReductionCurrent));
                            o.DamageReductionCurrent = Math.Max(0, o.DamageReductionCurrent - o.DamageReductionStep);
                        }
                        ApplyShieldReduction(o, ref damage);
                        ApplyManaShield(o, ref damage);
                        if (actor.DrainManaFirst && o.Mana > 0)
                        {
                            int drain = Math.Min(damage, o.Mana);
                            o.Mana -= drain;
                            UpdateManaBar(o);
                            damage -= drain;
                        }
                        o.CurrentHp -= damage;
                        if (o.CurrentHp <= 0)
                        {
                            if (!TryCheatDeath(o) && _players.Contains(o) && !_deathCauses.ContainsKey(o.Name))
                            {
                                _deathCauses[o.Name] = $"{actor.Name}'s {ability.Name} hits {o.Name} for {damage} damage!"; 
                            }
                        }
                        string spellLog = $"{actor.Name}'s {ability.Name} hits {o.Name} for {damage} damage!";
                        AppendLog(spellLog, actorIsPlayer);
                        actor.DamageDone += damage;
                        o.DamageTaken += damage;
                        o.HpBar.Value = Math.Min(o.HpBar.Maximum, Math.Max(0, o.CurrentHp));
                        o.Threat[actor] = o.Threat.GetValueOrDefault(actor) + damage;
                        o.CurrentTarget = actor;
                        AfterDamageDealt(actor, o, damage);
                    }
                    actor.AttackBar.Value = actor.AttackInterval;
                    actor.CurrentTarget = target;
                    CheckEnd();
                    return;
                }
                else
                {
                    if (target.SpellDodgeChance > 0 && _rng.NextDouble() < target.SpellDodgeChance)
                    {
                        AppendLog($"{target.Name} deflects the spell!", actorIsPlayer);
                        actor.AttackBar.Value = actor.AttackInterval;
                        return;
                    }
                    int spellDamage = CalculateSpellDamage(actor, target, ability);
                    if (target.DamageReductionCurrent > 0)
                    {
                        spellDamage = (int)(spellDamage * (1 - target.DamageReductionCurrent));
                        target.DamageReductionCurrent = Math.Max(0, target.DamageReductionCurrent - target.DamageReductionStep);
                    }
                    ApplyShieldReduction(target, ref spellDamage);
                    ApplyManaShield(target, ref spellDamage);
                    if (actor.DrainManaFirst && target.Mana > 0)
                    {
                        int drain = Math.Min(spellDamage, target.Mana);
                        target.Mana -= drain;
                        UpdateManaBar(target);
                        spellDamage -= drain;
                    }
                    target.CurrentHp -= spellDamage;
                    if (target.CurrentHp <= 0)
                    {
                        if (!TryCheatDeath(target) && _players.Contains(target) && !_deathCauses.ContainsKey(target.Name))
                        {
                            _deathCauses[target.Name] = ability.Name == "Drain Life"
                                ? $"{actor.Name} siphons {spellDamage} life from {target.Name}!"
                                : $"{actor.Name}'s {ability.Name} hits {target.Name} for {spellDamage} damage!";
                        }
                    }
                    string spellLog = ability.Name == "Drain Life"
                        ? $"{actor.Name} siphons {spellDamage} life from {target.Name}!"
                        : $"{actor.Name}'s {ability.Name} hits {target.Name} for {spellDamage} damage!";
                    AppendLog(spellLog, actorIsPlayer);
                    if (ability.Name == "Drain Life")
                    {
                        actor.CurrentHp = Math.Min(actor.MaxHp, actor.CurrentHp + spellDamage);
                        actor.HpBar.Value = Math.Min(actor.HpBar.Maximum, Math.Max(0, actor.CurrentHp));
                        AppendLog($"{actor.Name} absorbs {spellDamage} health!", actorIsPlayer, true);
                    }

                    actor.DamageDone += spellDamage;
                    target.DamageTaken += spellDamage;
                    target.HpBar.Value = Math.Min(target.HpBar.Maximum, Math.Max(0, target.CurrentHp));
                    actor.AttackBar.Value = actor.AttackInterval;
                    target.Threat[actor] = target.Threat.GetValueOrDefault(actor) + spellDamage;
                    target.CurrentTarget = actor;
                    actor.CurrentTarget = target;
                    AfterDamageDealt(actor, target, spellDamage);
                    CheckEnd();
                    return;
                }
            }

            if (target.Passives.TryGetValue("Parry", out int parryLevel))
            {
                int chance = (5 + target.Strength / 30 + target.Dex / 30) * parryLevel;
                if (_rng.Next(100) < chance)
                {
                    AppendLog($"{target.Name} parries {actor.Name}'s attack!", _players.Contains(target));
                    actor.AttackBar.Value = actor.AttackInterval;
                    return;
                }
            }
            if (target.Passives.TryGetValue("Nimble", out int nimbleLevel))
            {
                int chance = (target.Dex / 10) * nimbleLevel;
                if (_rng.Next(100) < chance)
                {
                    AppendLog($"{target.Name} dodges {actor.Name}'s attack!", _players.Contains(target));
                    actor.AttackBar.Value = actor.AttackInterval;
                    return;
                }
            }

            bool isAbilityAttack = ability.Name != "-basic attack-";
            int dmg = CalculateDamage(actor, target, isAbilityAttack);
            if (target.DamageReductionCurrent > 0)
            {
                dmg = (int)(dmg * (1 - target.DamageReductionCurrent));
                target.DamageReductionCurrent = Math.Max(0, target.DamageReductionCurrent - target.DamageReductionStep);
            }
            ApplyShieldReduction(target, ref dmg);
            ApplyManaShield(target, ref dmg);
            if (target.Passives.ContainsKey("Bloodlust"))
            {
                double missing = 1 - target.CurrentHp / (double)target.MaxHp;
                double mult = 1.75 - missing;
                dmg = (int)(dmg * mult);
            }
            if (actor.DrainManaFirst && target.Mana > 0)
            {
                int drain = Math.Min(dmg, target.Mana);
                target.Mana -= drain;
                UpdateManaBar(target);
                dmg -= drain;
            }
            target.CurrentHp -= dmg;
            if (target.CurrentHp <= 0)
            {
                if (!TryCheatDeath(target) && _players.Contains(target) && !_deathCauses.ContainsKey(target.Name))
                {
                    _deathCauses[target.Name] = GenerateAttackLog(actor, target, dmg);
                }
            }
            string attackLog = GenerateAttackLog(actor, target, dmg);
            AppendLog(attackLog, _players.Contains(actor));
            actor.DamageDone += dmg;
            target.DamageTaken += dmg;
            target.HpBar.Value = Math.Min(target.HpBar.Maximum, Math.Max(0, target.CurrentHp));
            actor.AttackBar.Value = actor.AttackInterval;
            target.Threat[actor] = target.Threat.GetValueOrDefault(actor) + dmg;
            target.CurrentTarget = actor;
            actor.CurrentTarget = target;
            AfterDamageDealt(actor, target, dmg);
            if (target.RetaliateLowestStat && dmg > 0)
            {
                int low = Math.Min(target.Strength, Math.Min(target.Dex, target.Intelligence));
                actor.CurrentHp -= low;
                if (actor.CurrentHp <= 0)
                {
                    if (!TryCheatDeath(actor) && _players.Contains(actor) && !_deathCauses.ContainsKey(actor.Name))
                    {
                        _deathCauses[actor.Name] = $"{actor.Name} was slain by thorns.";
                    }
                }
                actor.HpBar.Value = Math.Min(actor.HpBar.Maximum, Math.Max(0, actor.CurrentHp));
                actor.DamageTaken += low;
                AppendLog($"{actor.Name} suffers {low} retaliatory damage!", _players.Contains(target));
            }
            if (ability.Name == "Shield Bash" && dmg > 0)
            {
                int shield = (int)(dmg * 0.5);
                ApplyShield(actor, actor, shield, 15000);
            }
            if (actor.SplashDamagePercent > 0)
            {
                var splashTargets = opponents.Where(o => o != target && o.CurrentHp > 0).ToList();
                if (splashTargets.Any())
                {
                    var st = splashTargets[_rng.Next(splashTargets.Count)];
                    int splash = (int)(dmg * actor.SplashDamagePercent);
                    ApplyShieldReduction(st, ref splash);
                    ApplyManaShield(st, ref splash);
                    st.CurrentHp -= splash;
                    AppendLog($"{st.Name} is hit by cleaving damage for {splash}!", _players.Contains(actor));
                    st.HpBar.Value = Math.Min(st.HpBar.Maximum, Math.Max(0, st.CurrentHp));
                    st.DamageTaken += splash;
                    st.Threat[actor] = st.Threat.GetValueOrDefault(actor) + splash;
                    AfterDamageDealt(actor, st, splash);
                }
            }
            if (actor.Passives.ContainsKey("Flesh Rip"))
            {
                int chance = 5 + actor.Strength / 15;
                double dmgPct = 0.10 + (actor.Strength / 15) * 0.01;
                if (_rng.Next(100) < chance)
                {
                    int bleed = (int)Math.Max(1, dmg * dmgPct);
                    target.Effects.Add(new StatusEffect { Kind = EffectKind.Bleed, RemainingMs = 6000, TickIntervalMs = 500, TimeUntilTickMs = 500, AmountPerTick = bleed, SourceIsPlayer = _players.Contains(actor) });
                    AppendLog($"{target.Name} starts bleeding!", _players.Contains(actor));
                }
            }
            if (actor.Passives.ContainsKey("Battle Mage"))
            {
                int mana = (int)(actor.Intelligence * 0.15);
                actor.Mana = Math.Min(actor.MaxMana, actor.Mana + mana);
                UpdateManaBar(actor);
            }
            CheckEnd();
        }

        private string GenerateAbilityLog(Creature actor, Creature target, Ability ability)
        {
            return ability.Name switch
            {
                "Fireball" => $"{actor.Name} hurls a blazing fireball at {target.Name}!",
                "Lightning Bolt" => $"{actor.Name} summons a crackling bolt of lightning toward {target.Name}!",
                "Ice Lance" => $"{actor.Name} launches an icy lance at {target.Name}!",
                "Arcane Blast" => $"{actor.Name} releases a wave of arcane force!",
                "Frost Nova" => $"{actor.Name} unleashes a frost nova!",
                "Shockwave" => $"{actor.Name} releases a powerful shockwave!",
                "Earthquake" => $"{actor.Name} causes the ground to quake violently!",
                "Meteor" => $"{actor.Name} calls down a fiery meteor!",
                "Flame Strike" => $"{actor.Name} smashes the ground with a flaming strike!",
                "Healing Wave" => $"{actor.Name} releases a healing wave!",
                "Chain Heal" => $"{actor.Name} channels a chain heal!",
                "Prayer of Healing" => $"{actor.Name} utters a prayer of healing!",
                "Holy Light" => $"{actor.Name} bathes {target.Name} in holy light!",
                "Bleed" => $"{actor.Name} rends {target.Name}, drawing rivers of blood!",
                "Poison" => $"{actor.Name} envenoms {target.Name} with a vile toxin!",
                "Curse" => $"{actor.Name} curses {target.Name}, sapping their strength!",
                "Regenerate" => $"{actor.Name} calls forth rejuvenating winds around {target.Name}!",
                "Rejuvenate" => $"{actor.Name} bathes {target.Name} in rejuvenating energy!",
                "Heal" => $"{actor.Name} channels soothing light into {target.Name}!",
                "Stone Skin" => $"{actor.Name} hardens {target.Name}'s skin like stone, reducing damage taken!",
                "Taunting Blows" => $"{actor.Name} bellows a challenge, daring foes to attack!",
                "Shield Bash" => $"{actor.Name} slams their shield into {target.Name}!",
                "Poison Arrow" => $"{actor.Name} fires a venom-tipped arrow at {target.Name}!",
                "Cleanse" => $"{actor.Name} purges foul magic from {target.Name}!",
                "Berserk" => $"{actor.Name} enters a berserk fury, increasing damage!",
                "Drain Life" => $"{actor.Name} siphons vitality from {target.Name}!",
                "Vanish" => $"{actor.Name} melts into the shadows, avoiding attacks!",
                "Arcane Shield" => $"{actor.Name} conjures a shimmering shield around {target.Name}, absorbing damage!",
                "Guardian Ward" => $"{actor.Name} forms a guardian ward that absorbs damage for the party!",
                "Divine Aegis" => $"{actor.Name} blesses {target.Name} with a divine aegis that absorbs damage!",
                "Fortify" => $"{actor.Name} fortifies {target.Name} with a protective shield!",
                _ => $"{actor.Name} uses {ability.Name} on {target.Name}!"
            };
        }

        private void ApplyBleed(Creature actor, Creature target)
        {
            int amt = (int)Math.Max(1, 1 + actor.Strength * 0.25);
            target.Effects.Add(new StatusEffect { Kind = EffectKind.Bleed, RemainingMs = 6000, TickIntervalMs = 500, TimeUntilTickMs = 500, AmountPerTick = amt, SourceIsPlayer = _players.Contains(actor) });
        }

        private void ApplyPoison(Creature actor, Creature target)
        {
            int amt = (int)Math.Max(1, 1 + actor.Dex * 0.35);
            int duration = actor.PoisonMastery ? 9000 : 6000;
            target.Effects.Add(new StatusEffect { Kind = EffectKind.Poison, RemainingMs = duration, TickIntervalMs = 1000, TimeUntilTickMs = 1000, AmountPerTick = amt, SourceIsPlayer = _players.Contains(actor) });
        }

        private void ApplyCurse(Creature actor, Creature target)
        {
            int percent = 10 + actor.Intelligence / 20;
            double factor = 1 - percent / 100.0;
            target.DamageDealtMultiplier *= factor;
            target.Effects.Add(new StatusEffect { Kind = EffectKind.DamageDown, RemainingMs = 10000, TickIntervalMs = int.MaxValue, TimeUntilTickMs = int.MaxValue, AmountPerTick = percent, SourceIsPlayer = _players.Contains(actor) });
        }

        private void ApplyHot(Creature actor, Creature target)
        {
            int amt = (int)Math.Max(1, (1 + target.Intelligence * 0.80) * actor.HealingDealtMultiplier * target.HealingReceivedMultiplier);
            int tick = actor.NatureGrace ? 1500 : 3000;
            target.Effects.Add(new StatusEffect { Kind = EffectKind.HoT, RemainingMs = 6000, TickIntervalMs = tick, TimeUntilTickMs = tick, AmountPerTick = amt, SourceIsPlayer = _players.Contains(actor), SourceName = actor.Name });
        }

        private void ApplyRejuvenate(Creature actor, Creature target)
        {
            int amt = (int)Math.Max(1, (1 + actor.Intelligence * 0.60) * actor.HealingDealtMultiplier * target.HealingReceivedMultiplier);
            int tick = actor.NatureGrace ? 1000 : 2000;
            target.Effects.Add(new StatusEffect { Kind = EffectKind.HoT, RemainingMs = 6000, TickIntervalMs = tick, TimeUntilTickMs = tick, AmountPerTick = amt, SourceIsPlayer = _players.Contains(actor), SourceName = actor.Name });
        }

        private void UpdateManaBar(Creature creature)
        {
            if (creature.ManaBar.Maximum > 0)
            {
                creature.ManaBar.Value = Math.Max(creature.ManaBar.Minimum,
                    Math.Min(creature.ManaBar.Maximum, creature.Mana));
            }
        }

        private void ApplyTaunt(Creature actor, List<Creature> opponents)
        {
            int duration = 2000 + (actor.Strength / 30) * 1000;
            foreach (var o in opponents.Where(o => o.CurrentHp > 0))
            {
                o.ForcedTarget = actor;
                o.ForcedTargetMs = duration;
            }
        }

        private void ApplyShield(Creature actor, Creature target, int shieldAmount, int durationMs)
        {
            target.Effects.Add(new StatusEffect
            {
                Kind = EffectKind.Shield,
                RemainingMs = durationMs,
                TickIntervalMs = int.MaxValue,
                TimeUntilTickMs = int.MaxValue,
                AmountPerTick = shieldAmount,
                SourceIsPlayer = _players.Contains(actor)
            });

            target.ShieldBar.Maximum = Math.Max(1, shieldAmount);
            target.ShieldBar.Value = Math.Max(0, shieldAmount);
            target.ShieldBar.Visible = true;
            AppendLog($"{target.Name} is protected by a magical shield ({shieldAmount}).", _players.Contains(actor), true);
        }

        private void ApplyShield(Creature actor, Creature target, double intMultiplier, int durationMs = 15000)
        {
            int shieldAmount = (int)Math.Max(1, 5 + actor.Intelligence * intMultiplier);
            ApplyShield(actor, target, shieldAmount, durationMs);
        }

        private void ApplyShieldReduction(Creature target, ref int dmg)
        {
            var shield = target.Effects.FirstOrDefault(e => e.Kind == EffectKind.Shield);
            if (shield != null && dmg > 0)
            {
                int absorb = Math.Min(dmg, shield.AmountPerTick);
                dmg -= absorb;
                shield.AmountPerTick -= absorb;
                target.ShieldBar.Maximum = Math.Max(1, shield.AmountPerTick);
                target.ShieldBar.Value = Math.Max(0, shield.AmountPerTick);
                if (shield.AmountPerTick <= 0)
                {
                    target.Effects.Remove(shield);
                    target.ShieldBar.Visible = false;
                }
            }
        }

        private void ApplyManaShield(Creature target, ref int dmg)
        {
            if (target.ManaBarrierThreshold > 0 && dmg > 0 && target.Mana > target.MaxMana * target.ManaBarrierThreshold)
            {
                int excess = target.Mana - (int)(target.MaxMana * target.ManaBarrierThreshold);
                int absorb = Math.Min(dmg, excess);
                target.Mana -= absorb;
                UpdateManaBar(target);
                dmg -= absorb;
            }
            if (target.ManaShield && dmg > 0 && target.Mana > 0)
            {
                int absorb = Math.Min(target.Mana, dmg / 2);
                target.Mana -= absorb;
                UpdateManaBar(target);
                dmg -= absorb;
            }
        }

        private bool TryCheatDeath(Creature target)
        {
            if (target.CheatDeathTrinket != null)
            {
                target.CurrentHp = target.MaxHp;
                target.Mana = target.MaxMana;
                target.HpBar.Value = target.MaxHp;
                UpdateManaBar(target);
                InventoryService.RemoveItem(target.CheatDeathTrinket);
                target.Equipment[EquipmentSlot.Trinket] = null;
                AppendLog($"{target.Name} is revived by {target.CheatDeathTrinket.Name}!", _players.Contains(target), true);
                target.CheatDeathTrinket = null;
                return true;
            }
            return false;
        }

        private void AfterDamageDealt(Creature actor, Creature target, int dmg)
        {
            if (actor.LeechPercent > 0 && dmg > 0)
            {
                int heal = (int)(dmg * actor.LeechPercent);
                actor.CurrentHp = Math.Min(actor.MaxHp, actor.CurrentHp + heal);
                actor.HpBar.Value = Math.Min(actor.MaxHp, actor.CurrentHp);
            }
            if (actor.SpellManaLeechPercent > 0 && dmg > 0)
            {
                actor.Mana = Math.Min(actor.MaxMana, actor.Mana + (int)(dmg * actor.SpellManaLeechPercent));
                UpdateManaBar(actor);
            }
            if (actor.Momentum)
            {
                actor.MomentumBonus = Math.Min(0.25, actor.MomentumBonus + 0.05);
                actor.DamageDealtMultiplier = 1 + actor.MomentumBonus;
            }
            if (target.ReturnDamagePercent > 0 && dmg > 0)
            {
                int ret = (int)(dmg * target.ReturnDamagePercent);
                actor.CurrentHp -= ret;
                actor.HpBar.Value = Math.Max(0, actor.CurrentHp);
                actor.DamageTaken += ret;
            }
            if (target.Momentum)
            {
                target.MomentumBonus = 0;
                target.DamageDealtMultiplier = 1.0;
            }
            if (target.SecondWindAvailable && target.CurrentHp <= target.MaxHp * 0.3 && target.CurrentHp > 0)
            {
                int heal = (int)(target.MaxHp * 0.25);
                target.CurrentHp = Math.Min(target.MaxHp, target.CurrentHp + heal);
                target.HpBar.Value = Math.Min(target.MaxHp, target.CurrentHp);
                target.SecondWindAvailable = false;
                AppendLog($"{target.Name} rallies with a second wind!", _players.Contains(target), true);
            }
            var weapon = actor.GetWeapon();
            if (weapon?.ProcAbility != null && _rng.NextDouble() <= weapon.ProcChance)
            {
                int procDmg = CalculateSpellDamage(actor, target, weapon.ProcAbility);
                target.CurrentHp -= procDmg;
                target.HpBar.Value = Math.Max(0, target.CurrentHp);
                AppendLog($"{actor.Name}'s {weapon.Name} triggers {weapon.ProcAbility.Name} for {procDmg} damage!", _players.Contains(actor), false);
                actor.DamageDone += procDmg;
                target.DamageTaken += procDmg;
                if (target.CurrentHp <= 0)
                {
                    _deathCauses[target.Name] = $"{actor.Name}'s {weapon.ProcAbility.Name} hits {target.Name} for {procDmg} damage!";
                    CheckEnd();
                }
            }
        }

        private void AfterHeal(Creature actor, Creature target, int healAmt)
        {
            if (actor.ManaOnHealPercent > 0 && target != actor)
            {
                int mana = (int)(healAmt * actor.ManaOnHealPercent);
                target.Mana = Math.Min(target.MaxMana, target.Mana + mana);
                UpdateManaBar(target);
            }
            if (actor.ShieldOnHealPercent > 0 && target != actor)
            {
                int shield = (int)(healAmt * actor.ShieldOnHealPercent);
                if (shield > 0)
                {
                    target.Effects.Add(new StatusEffect { Kind = EffectKind.Shield, RemainingMs = 15000, TickIntervalMs = int.MaxValue, TimeUntilTickMs = int.MaxValue, AmountPerTick = shield, SourceIsPlayer = _players.Contains(actor) });
                    target.ShieldBar.Maximum = Math.Max(1, shield);
                    target.ShieldBar.Value = Math.Max(0, shield);
                    target.ShieldBar.Visible = true;
                }
            }
            if (actor.SelfHealOnHealPercent > 0 && target != actor)
            {
                int self = (int)(healAmt * actor.SelfHealOnHealPercent);
                actor.CurrentHp = Math.Min(actor.MaxHp, actor.CurrentHp + self);
                actor.HpBar.Value = Math.Min(actor.MaxHp, actor.CurrentHp);
            }
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
            return $"{actor.Name} {verb} {target.Name} for {dmg} damage!";
        }

        private Creature? ChooseHealerTarget(Creature actor, List<Creature> allies)
        {
            var injured = allies.Where(a => a.CurrentHp < a.MaxHp && !a.IsVanished).ToList();
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
            if (actor.ForcedTarget != null && actor.ForcedTargetMs > 0 && actor.ForcedTarget.CurrentHp > 0 && !actor.ForcedTarget.IsVanished)
                return actor.ForcedTarget;
            var alive = opponents.Where(o => o.CurrentHp > 0 && !o.IsVanished).ToList();
            if (!alive.Any()) return null;
            bool actorIsPlayer = _players.Contains(actor);
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
                            return notOnMe.Any() ? ChooseRandomTarget(notOnMe, actorIsPlayer) : ChooseRandomTarget(alive, actorIsPlayer);
                        default:
                            return ChooseRandomTarget(alive, actorIsPlayer);
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
                            if (atkMe.Any()) return ChooseRandomTarget(atkMe, actorIsPlayer);
                            break;
        
                        case "prioritize targets that attack non-tanks":
                            var atkNonTank = alive.Where(o => o.CurrentTarget != null && o.CurrentTarget.Role != "Tank").ToList();
                            if (atkNonTank.Any()) return ChooseRandomTarget(atkNonTank, actorIsPlayer);
                            break;
                    }
                    return ChooseRandomTarget(alive, actorIsPlayer);
                default:
                    return ChooseRandomTarget(alive, actorIsPlayer);
            }
        }

        private Creature ChooseRandomTarget(List<Creature> candidates, bool actorIsPlayer)
        {
            if (actorIsPlayer)
                return candidates[_rng.Next(candidates.Count)];
            var weighted = new List<Creature>();
            foreach (var c in candidates)
            {
                weighted.Add(c);
                if (c.Role == "Tank") weighted.Add(c);
            }
            return weighted[_rng.Next(weighted.Count)];
        }

        private Ability ChooseAbility(Creature actor)
        {
            var abilities = actor.Abilities;
            var basic = abilities.FirstOrDefault(a => a.Id == 0) ?? new Ability { Id = 0, Name = "-basic attack-", Priority = 1, Cost = 0 };
            Ability? chosen = null;
            var usable = abilities.Where(a => a.Id == 0 || (actor.Mana >= a.Cost * actor.AbilityCostMultiplier && actor.Cooldowns.GetValueOrDefault(a.Id) <= 0)).ToList();
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
            if (_battleEnded) return;
            foreach (var p in _players) p.HpBar.Value = Math.Min(p.HpBar.Maximum, Math.Max(0, p.CurrentHp));
            foreach (var n in _npcs) n.HpBar.Value = Math.Min(n.HpBar.Maximum, Math.Max(0, n.CurrentHp));
            if (_players.All(p => p.CurrentHp <= 0) || _npcs.All(n => n.CurrentHp <= 0))
            {
                _battleEnded = true;
                _gameTimer.Stop();
                foreach (var c in _players.Concat(_npcs))
                {
                    c.ShieldBar.Visible = false;
                }
                bool playersWin = _players.Any(p => p.CurrentHp > 0);
                _playersWin = playersWin;
                string lootSummary = string.Empty;
                AppendLog(playersWin ? "Players win!" : "NPCs win!", playersWin);
                if (playersWin)
                {
                    AwardExperience(_npcs.Sum(n => n.Level), _playerIds.Values);
                    foreach (var npc in _npcs)
                    {
                        EnemyKnowledgeService.RecordKill(_userId, npc.Name);
                    }
                    var loot = LootService.GenerateLoot(_npcs.Select(n => (n.Name, n.Level)), _userId, _areaId);
                    if (loot.Count > 0)
                    {
                        var parts = new List<string>();
                        if (loot.TryGetValue("gold", out int gold)) parts.Add($"{gold} gold");
                        foreach (var kv in loot.Where(k => k.Key != "gold")) parts.Add($"{kv.Value} {kv.Key}");
                        lootSummary = string.Join(", ", parts);
                        if (parts.Count > 0) AppendLog("Loot: " + lootSummary, true);
                    }
                }
                BattleLogService.AddLog(string.Join(Environment.NewLine, lstLog.Items.Cast<LogEntry>().Select(l => l.Text)));
                HandlePlayerDeaths();
                SaveState();
                if (_darkSpireBattle)
                {
                    using var dsConn = new MySqlConnection(DatabaseConfig.ConnectionString);
                    dsConn.Open();
                    using var dsCmd = new MySqlCommand("UPDATE dark_spire_state SET current_min_power = current_min_power + 5, current_max_power = current_max_power + 5 WHERE account_id=@id", dsConn);
                    dsCmd.Parameters.AddWithValue("@id", _userId);
                    dsCmd.ExecuteNonQuery();
                }
                var playerSummaries = _players.Select(p => new CombatantSummary(p.Name, p.DamageDone, p.DamageTaken));
                var enemySummaries = _npcs.Select(n => new CombatantSummary(n.Name, n.DamageDone, n.DamageTaken));
                var summary = new BattleSummaryForm(playerSummaries, enemySummaries, playersWin, lootSummary);
                Hide();
                summary.FormClosed += (_, __) => summary.Dispose();
                summary.Show(this.Owner);
                Close();
            }
        }

        private int CalculateDamage(Creature actor, Creature target, bool isAbility = false)
        {
            var weapon = actor.GetWeapon();
            double statTotal = actor.Strength * 0.3 + actor.Dex * 0.3;
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
            double weaponDamage = Math.Max(1, statTotal * mult);
            int dmg = (int)Math.Max(1, weaponDamage - target.MeleeDefense);
            double critChance = 0.05 + actor.Dex / 5 * 0.01 + critChanceBonus + actor.CritChanceBonus;
            if (target.NoCrits) critChance = 0;
            if (actor.Passives.TryGetValue("Deadly Strikes", out int dsLvl))
            {
                critChance += dsLvl * (actor.Dex / 10 * 0.01);
            }
            if (_rng.NextDouble() < Math.Min(1.0, critChance))
            {
                double critMult = 1.5 + critDamageBonus + actor.CritDamageBonus;
                dmg = (int)(dmg * critMult);
                weaponDamage *= critMult;
            }
            if (actor.Passives.ContainsKey("Bloodlust"))
            {
                double missing = 1 - actor.CurrentHp / (double)actor.MaxHp;
                double bonusPercent = 0.02 + missing * 2;
                dmg += (int)(weaponDamage * bonusPercent);
            }
            dmg += actor.AttackFlatBonus + (int)(actor.Intelligence * actor.AttackIntBonusMultiplier);
            dmg = (int)(dmg * actor.DamageDealtMultiplier);
            dmg = (int)(dmg * target.DamageTakenMultiplier);
            if (isAbility) dmg += actor.Level;
            return dmg;
        }

        private int CalculateSpellDamage(Creature actor, Creature target, Ability ability)
        {
            var match = Regex.Match(ability.Description, "(\\d+) \\+ (\\d+)% of your (STR|DEX|INT)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                double baseVal = double.Parse(match.Groups[1].Value);
                double percent = double.Parse(match.Groups[2].Value) / 100.0;
                double stat = match.Groups[3].Value.ToUpper() switch
                {
                    "STR" => actor.Strength,
                    "DEX" => actor.Dex,
                    "INT" => actor.Intelligence,
                    _ => 0
                };
                double dmg = (baseVal + stat * percent) * actor.SpellDamageMultiplier * target.DamageTakenMultiplier;
                int total = (int)Math.Max(1, dmg - target.MagicDefense);
                return total + actor.Level;
            }
            return 0;
        }

        private void AwardExperience(int totalEnemyLevels, IEnumerable<int> participantIds)
        {
            var ids = participantIds.ToList();
            int partySize = ids.Count;
            if (partySize <= 0) return;

            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            int expGain = totalEnemyLevels * 10;
            double expPer = expGain / (double)partySize;
            int baseExp = (int)Math.Floor(expPer);
            int remainder = expGain - baseExp * partySize;

            var idToCreature = _playerIds.ToDictionary(kv => kv.Value, kv => kv.Key);

            var expIds = ids.Where(id => !idToCreature[id].ConvertExpToGold).ToList();
            if (expIds.Count > 0)
            {
                string allClause = string.Join(",", expIds.Select((_, i) => "@id" + i));
                using var updateCmd = new MySqlCommand($"UPDATE characters SET experience_points = experience_points + @exp WHERE id IN ({allClause})", conn);
                updateCmd.Parameters.AddWithValue("@exp", baseExp);
                for (int i = 0; i < expIds.Count; i++)
                    updateCmd.Parameters.AddWithValue("@id" + i, expIds[i]);
                updateCmd.ExecuteNonQuery();
            }

            var bonusIds = new List<int>();
            if (remainder > 0)
            {
                var ordered = ids.OrderByDescending(id => idToCreature[id].Level).Take(remainder).ToList();
                bonusIds.AddRange(ordered);
                var bonusExpIds = ordered.Where(id => !idToCreature[id].ConvertExpToGold).ToList();
                if (bonusExpIds.Count > 0)
                {
                    string bonusClause = string.Join(",", bonusExpIds.Select((id, i) => "@bid" + i));
                    using var bonusCmd = new MySqlCommand($"UPDATE characters SET experience_points = experience_points + 1 WHERE id IN ({bonusClause})", conn);
                    for (int i = 0; i < bonusExpIds.Count; i++)
                        bonusCmd.Parameters.AddWithValue("@bid" + i, bonusExpIds[i]);
                    bonusCmd.ExecuteNonQuery();
                }
            }

            var mercGains = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (int id in ids)
            {
                var c = idToCreature[id];
                int gain = baseExp + (bonusIds.Contains(id) ? 1 : 0);
                if (c.ConvertExpToGold)
                {
                    using var goldCmd = new MySqlCommand("UPDATE users SET gold = gold + @g WHERE id=@uid", conn);
                    goldCmd.Parameters.AddWithValue("@g", gain);
                    goldCmd.Parameters.AddWithValue("@uid", _userId);
                    goldCmd.ExecuteNonQuery();
                }
                else if (_mercenaryIds.Contains(id))
                    mercGains[c.Name] = gain;
            }
            if (mercGains.Count > 0)
                PartyHireService.ApplyMercenaryExperience(_userId, mercGains);

            if (remainder > 0)
                AppendLog($"Each party member gains {baseExp} EXP ({remainder} member(s) gain +1).", true);
            else
                AppendLog($"Each party member gains {baseExp} EXP!", true);
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
            public int MagicDefense { get; set; }
            public int Level { get; set; }
            public int Power { get; set; }
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
            public ProgressBar ShieldBar { get; set; } = new();
            public int AttackInterval { get; set; }
            public int NextActionMs { get; set; }
            public int DamageDone { get; set; }
            public int DamageTaken { get; set; }
            public List<Ability> Abilities { get; } = new();
            public Dictionary<int, int> Cooldowns { get; } = new();
            public List<StatusEffect> Effects { get; } = new();
            public Dictionary<string, int> Passives { get; } = new();
            public Creature? ForcedTarget { get; set; }
            public int ForcedTargetMs { get; set; }
            public bool IsVanished { get; set; }
            public int VanishRemainingMs { get; set; }
            public double DamageDealtMultiplier { get; set; } = 1.0;
            public double HealingDealtMultiplier { get; set; } = 1.0;
            public double DamageTakenMultiplier { get; set; } = 1.0;
            public double HealingReceivedMultiplier { get; set; } = 1.0;
            public double SpellDamageMultiplier { get; set; } = 1.0;
            public double AttackSpeedMultiplier { get; set; } = 1.0;
            public double AbilityCostMultiplier { get; set; } = 1.0;
            public double CooldownMultiplier { get; set; } = 1.0;
            public double SplashDamagePercent { get; set; } = 0.0;
            public double ManaOnHealPercent { get; set; } = 0.0;
            public double ReturnDamagePercent { get; set; } = 0.0;
            public double LeechPercent { get; set; } = 0.0;
            public bool NoCrits { get; set; }
            public double ShieldStartPercent { get; set; } = 0.0;
            public double DamageReductionCurrent { get; set; } = 0.0;
            public double DamageReductionStep { get; set; } = 0.0;
            public bool Momentum { get; set; }
            public double MomentumBonus { get; set; }
            public bool PoisonMastery { get; set; }
            public bool ManaShield { get; set; }
            public bool SecondWindAvailable { get; set; }
            public bool NatureGrace { get; set; }
            public double SpellDodgeChance { get; set; } = 0.0;
            public double ShieldOnHealPercent { get; set; } = 0.0;
            public double SelfHealOnHealPercent { get; set; } = 0.0;
            public double SpellManaLeechPercent { get; set; } = 0.0;
            public double AttackIntBonusMultiplier { get; set; } = 0.0;
            public int AttackFlatBonus { get; set; } = 0;
            public double CritChanceBonus { get; set; } = 0.0;
            public double CritDamageBonus { get; set; } = 0.0;
            public double ManaBarrierThreshold { get; set; } = 0.0;
            public bool ConvertExpToGold { get; set; }
            public Trinket? CheatDeathTrinket { get; set; }
            public bool RetaliateLowestStat { get; set; }
            public bool DrainManaFirst { get; set; }

            public Weapon? GetWeapon()
            {
                if (Equipment.TryGetValue(EquipmentSlot.LeftHand, out var lh) && lh is Weapon w) return w;
                if (Equipment.TryGetValue(EquipmentSlot.RightHand, out var rh) && rh is Weapon w2) return w2;
                return null;
            }
        }

        private enum EffectKind { Bleed, Poison, HoT, Shield, DamageDown }

        private class StatusEffect
        {
            public EffectKind Kind { get; set; }
            public int RemainingMs { get; set; }
            public int TickIntervalMs { get; set; }
            public int TimeUntilTickMs { get; set; }
            public int AmountPerTick { get; set; }
            public bool SourceIsPlayer { get; set; }
            public string? SourceName { get; set; }
            public int ManaPerTick { get; set; }
        }


        private void ApplyEquipmentBonuses(Creature c)
        {
            foreach (var item in c.Equipment.Values)
            {
                if (item == null) continue;
                foreach (var kv in item.FlatBonuses)
                {
                    switch (kv.Key)
                    {
                        case "Strength":
                            c.Strength += kv.Value;
                            break;
                        case "Dexterity":
                            c.Dex += kv.Value;
                            break;
                        case "Intelligence":
                            c.Intelligence += kv.Value;
                            c.MaxMana += kv.Value;
                            c.Mana += kv.Value;
                            break;
                        case "HP":
                            c.MaxHp += kv.Value;
                            c.CurrentHp += kv.Value;
                            break;
                        case "Mana":
                            c.MaxMana += kv.Value;
                            c.Mana += kv.Value;
                            break;
                        case "Melee Defense":
                            c.MeleeDefense += kv.Value;
                            break;
                        case "Magic Defense":
                            c.MagicDefense += kv.Value;
                            break;
                        case "Attack":
                            c.AttackFlatBonus += kv.Value;
                            break;
                    }
                }
                foreach (var kv in item.PercentBonuses)
                {
                    switch (kv.Key)
                    {
                        case "Damage Dealt":
                            c.DamageDealtMultiplier *= 1 + kv.Value / 100.0;
                            break;
                        case "Damage Taken":
                            c.DamageTakenMultiplier *= 1 + kv.Value / 100.0;
                            break;
                        case "Attack Speed":
                            c.AttackSpeedMultiplier *= 1 + kv.Value / 100.0;
                            break;
                        case "Healing Done":
                            c.HealingDealtMultiplier *= 1 + kv.Value / 100.0;
                            break;
                        case "Healing Received":
                            c.HealingReceivedMultiplier *= 1 + kv.Value / 100.0;
                            break;
                        case "Strength":
                            c.Strength = (int)(c.Strength * (1 + kv.Value / 100.0));
                            break;
                        case "Dexterity":
                            c.Dex = (int)(c.Dex * (1 + kv.Value / 100.0));
                            break;
                        case "Intelligence":
                            c.Intelligence = (int)(c.Intelligence * (1 + kv.Value / 100.0));
                            c.MaxMana = (int)(c.MaxMana * (1 + kv.Value / 100.0));
                            c.Mana = (int)(c.Mana * (1 + kv.Value / 100.0));
                            break;
                        case "HP":
                            c.MaxHp = (int)(c.MaxHp * (1 + kv.Value / 100.0));
                            c.CurrentHp = (int)(c.CurrentHp * (1 + kv.Value / 100.0));
                            break;
                        case "Mana":
                            c.MaxMana = (int)(c.MaxMana * (1 + kv.Value / 100.0));
                            c.Mana = (int)(c.Mana * (1 + kv.Value / 100.0));
                            break;
                        case "Melee Defense":
                            c.MeleeDefense = (int)(c.MeleeDefense * (1 + kv.Value / 100.0));
                            break;
                        case "Magic Defense":
                            c.MagicDefense = (int)(c.MagicDefense * (1 + kv.Value / 100.0));
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
                                c.MaxHp = (int)(c.MaxHp * (1 + ev.Value / 100.0));
                                c.CurrentHp = (int)(c.CurrentHp * (1 + ev.Value / 100.0));
                                break;
                            case "max_mana_pct":
                                c.MaxMana = (int)(c.MaxMana * (1 + ev.Value / 100.0));
                                c.Mana = (int)(c.Mana * (1 + ev.Value / 100.0));
                                break;
                            case "damage_dealt_pct":
                                c.DamageDealtMultiplier *= 1 + ev.Value / 100.0;
                                c.SpellDamageMultiplier *= 1 + ev.Value / 100.0;
                                break;
                            case "ability_damage_per5_lowest_stat_pct":
                                int low = Math.Min(c.Strength, Math.Min(c.Dex, c.Intelligence));
                                int bonus = (low / 5) * (int)ev.Value;
                                c.SpellDamageMultiplier *= 1 + bonus / 100.0;
                                break;
                            case "auto_attack_damage_per5_lowest_stat_pct":
                                int low2 = Math.Min(c.Strength, Math.Min(c.Dex, c.Intelligence));
                                int bonus2 = (low2 / 5) * (int)ev.Value;
                                c.DamageDealtMultiplier *= 1 + bonus2 / 100.0;
                                break;
                            case "convert_exp_to_gold":
                                c.ConvertExpToGold = true;
                                break;
                            case "cheat_death":
                                c.CheatDeathTrinket = tr;
                                break;
                            case "combat_regen_pct":
                                int amt = (int)(c.MaxHp * (ev.Value / 100.0));
                                int manaAmt = amt;
                                double sec = tr.Effects.ContainsKey("combat_regen_interval_sec") ? tr.Effects["combat_regen_interval_sec"] : 3;
                                int interval = (int)(sec * 1000);
                                c.Effects.Add(new StatusEffect { Kind = EffectKind.HoT, RemainingMs = int.MaxValue, TickIntervalMs = interval, TimeUntilTickMs = interval, AmountPerTick = amt, ManaPerTick = manaAmt, SourceIsPlayer = _players.Contains(c) });
                                break;
                            case "mana_shield_threshold_pct":
                                c.ManaBarrierThreshold = ev.Value / 100.0;
                                break;
                            case "retaliate_lowest_stat":
                                c.RetaliateLowestStat = true;
                                break;
                            case "crit_chance_pct":
                                c.CritChanceBonus += ev.Value / 100.0;
                                break;
                            case "crit_damage_pct":
                                c.CritDamageBonus += ev.Value / 100.0;
                                break;
                            case "tank_health_defense_pct":
                                if (c.Role.Equals("Tank", StringComparison.OrdinalIgnoreCase))
                                {
                                    double mult = 1 + ev.Value / 100.0;
                                    c.MaxHp = (int)(c.MaxHp * mult);
                                    c.CurrentHp = (int)(c.CurrentHp * mult);
                                    c.MeleeDefense = (int)(c.MeleeDefense * mult);
                                    c.MagicDefense = (int)(c.MagicDefense * mult);
                                }
                                break;
                            case "dps_damage_pct":
                                if (c.Role.Equals("DPS", StringComparison.OrdinalIgnoreCase))
                                {
                                    c.DamageDealtMultiplier *= 1 + ev.Value / 100.0;
                                    c.SpellDamageMultiplier *= 1 + ev.Value / 100.0;
                                }
                                break;
                            case "healer_healing_pct":
                                if (c.Role.Equals("Healer", StringComparison.OrdinalIgnoreCase))
                                {
                                    c.HealingDealtMultiplier *= 1 + ev.Value / 100.0;
                                }
                                break;
                            case "damage_pct":
                                c.DamageDealtMultiplier *= 1 + ev.Value / 100.0;
                                c.SpellDamageMultiplier *= 1 + ev.Value / 100.0;
                                break;
                            case "drain_mana_first":
                                c.DrainManaFirst = true;
                                break;
                            case "damage_healing_pct":
                                c.DamageDealtMultiplier *= 1 + ev.Value / 100.0;
                                c.SpellDamageMultiplier *= 1 + ev.Value / 100.0;
                                c.HealingDealtMultiplier *= 1 + ev.Value / 100.0;
                                break;
                            case "action_speed_pct":
                                c.AttackSpeedMultiplier *= 1 + ev.Value / 100.0;
                                break;
                        }
                    }
                }
            }
        }

        private void ApplyPassiveModifiers(Creature c)
        {
            foreach (var kv in c.Passives)
            {
                switch (kv.Key)
                {
                    case "Pacifist":
                        c.DamageDealtMultiplier *= 0.5;
                        c.HealingDealtMultiplier *= 1.5;
                        break;
                    case "Cleaving Strikes":
                        c.SplashDamagePercent += 0.20;
                        break;
                    case "Iron Wall":
                        c.DamageReductionCurrent = 1.0;
                        c.DamageReductionStep = 0.10;
                        break;
                    case "Mana Conduit":
                        c.ManaOnHealPercent += 0.15;
                        break;
                    case "Thornmail":
                        c.ReturnDamagePercent += 0.10;
                        break;
                    case "Vampiric Strikes":
                        c.LeechPercent += 0.10;
                        break;
                    case "Berserker":
                        c.DamageDealtMultiplier *= 1.25;
                        c.DamageTakenMultiplier *= 1.15;
                        break;
                    case "Bulwark":
                        c.MeleeDefense = (int)(c.MeleeDefense * 1.20);
                        c.AttackSpeedMultiplier *= 0.90;
                        break;
                    case "Arcane Mastery":
                        c.SpellDamageMultiplier *= 1.20;
                        break;
                    case "Fleet Footed":
                        c.AttackSpeedMultiplier *= 1.10;
                        break;
                    case "Regenerative":
                        c.HealingReceivedMultiplier *= 1.10;
                        break;
                    case "Mana Efficiency":
                        c.AbilityCostMultiplier *= 0.80;
                        break;
                    case "Quick Recovery":
                        c.CooldownMultiplier *= 0.80;
                        break;
                    case "Steadfast":
                        c.NoCrits = true;
                        break;
                    case "Guardian's Grace":
                        c.ShieldOnHealPercent += 0.10;
                        break;
                    case "Protective Barrier":
                        c.ShieldStartPercent = 0.20;
                        break;
                    case "Momentum":
                        c.Momentum = true;
                        break;
                    case "Firebrand":
                        c.AttackFlatBonus += 5;
                        c.AttackIntBonusMultiplier += 0.20;
                        break;
                    case "Poison Mastery":
                        c.PoisonMastery = true;
                        break;
                    case "Mana Shield":
                        c.ManaShield = true;
                        break;
                    case "Second Wind":
                        c.SecondWindAvailable = true;
                        break;
                    case "Nature's Grace":
                        c.NatureGrace = true;
                        break;
                    case "Spell Deflection":
                        c.SpellDodgeChance += 0.10;
                        break;
                    case "Rejuvenating Healer":
                        c.SelfHealOnHealPercent += 0.20;
                        break;
                    case "Arcane Siphon":
                        c.SpellManaLeechPercent += 0.05;
                        break;
                    case "Plate Mastery":
                        foreach (var item in c.Equipment.Values)
                        {
                            if (item is Armor a && a.Name.Contains("Plate", StringComparison.OrdinalIgnoreCase))
                            {
                                if (a.FlatBonuses.TryGetValue("Melee Defense", out int md))
                                    c.MeleeDefense += (int)(md * 0.5);
                                if (a.FlatBonuses.TryGetValue("Magic Defense", out int mgd))
                                    c.MagicDefense += (int)(mgd * 0.5);
                            }
                        }
                        break;
                    case "Cloth Mastery":
                        foreach (var item in c.Equipment.Values)
                        {
                            if (item is Armor a && (a.Name.Contains("Cloth", StringComparison.OrdinalIgnoreCase) || a.Name.Contains("Robe", StringComparison.OrdinalIgnoreCase)))
                            {
                                if (a.FlatBonuses.TryGetValue("Melee Defense", out int md))
                                    c.MeleeDefense += (int)(md * 0.5);
                                if (a.FlatBonuses.TryGetValue("Magic Defense", out int mgd))
                                    c.MagicDefense += (int)(mgd * 0.5);
                            }
                        }
                        break;
                    case "Leather Mastery":
                        foreach (var item in c.Equipment.Values)
                        {
                            if (item is Armor a && a.Name.Contains("Leather", StringComparison.OrdinalIgnoreCase))
                            {
                                if (a.FlatBonuses.TryGetValue("Melee Defense", out int md))
                                    c.MeleeDefense += (int)(md * 0.5);
                                if (a.FlatBonuses.TryGetValue("Magic Defense", out int mgd))
                                    c.MagicDefense += (int)(mgd * 0.5);
                            }
                        }
                        break;
                }
            }
            if (c.ShieldStartPercent > 0)
            {
                int shield = (int)(c.MaxHp * c.ShieldStartPercent);
                c.Effects.Add(new StatusEffect
                {
                    Kind = EffectKind.Shield,
                    RemainingMs = int.MaxValue,
                    TickIntervalMs = int.MaxValue,
                    TimeUntilTickMs = int.MaxValue,
                    AmountPerTick = shield,
                    SourceIsPlayer = _players.Contains(c)
                });
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
            var panel = new Panel { Width = 180, Height = 100 };
            var lbl = new Label { Text = c.Name, AutoSize = true };
            // shield bar, hidden by default and placed above HP
            c.ShieldBar = CloneProgressBar(manaTemplate);
            c.ShieldBar.Visible = false;
            c.ShieldBar.Location = new Point(0, 15);

            c.HpBar = CloneProgressBar(hpTemplate);
            c.HpBar.Maximum = Math.Max(1, c.MaxHp);
            c.HpBar.Value = Math.Min(c.HpBar.Maximum, Math.Max(0, c.CurrentHp));
            c.HpBar.Location = new Point(0, 35);

            panel.Controls.Add(lbl);
            panel.Controls.Add(c.ShieldBar);
            panel.Controls.Add(c.HpBar);

            if (c.MaxMana > 0)
            {
                c.ManaBar = CloneProgressBar(manaTemplate);
                c.ManaBar.Maximum = Math.Max(1, c.MaxMana);
                UpdateManaBar(c);
                c.ManaBar.Location = new Point(0, 55);
                panel.Controls.Add(c.ManaBar);
                c.AttackBar = CloneProgressBar(attackTemplate);
                c.AttackBar.Maximum = 100;
                c.AttackBar.Value = 100;
                c.AttackBar.Location = new Point(0, 75);
            }
            else
            {
                c.AttackBar = CloneProgressBar(attackTemplate);
                c.AttackBar.Maximum = 100;
                c.AttackBar.Value = 100;
                c.AttackBar.Location = new Point(0, 55);
            }
            panel.Controls.Add(c.AttackBar);

            var existingShield = c.Effects.FirstOrDefault(e => e.Kind == EffectKind.Shield);
            if (existingShield != null)
            {
                c.ShieldBar.Maximum = Math.Max(1, existingShield.AmountPerTick);
                c.ShieldBar.Value = Math.Max(0, existingShield.AmountPerTick);
                c.ShieldBar.Visible = true;
            }

            return panel;
        }

        private ColoredProgressBar CloneProgressBar(ColoredProgressBar template)
        {
            return new ColoredProgressBar
            {
                Width = template.Width,
                Height = template.Height,
                ProgressColor = template.ProgressColor,
                BackColor = template.BackColor,
                Style = template.Style
            };
        }

        private void lstLog_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            var entry = (LogEntry)lstLog.Items[e.Index];
            e.DrawBackground();
            using var brush = new SolidBrush(entry.Color);
            e.Graphics.DrawString(entry.Text, e.Font, brush, e.Bounds);
            e.DrawFocusRectangle();
        }

        private void SaveState()
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            foreach (var p in _players)
            {
                using MySqlCommand cmd = new MySqlCommand("UPDATE characters SET current_hp=@hp, mana=@mana WHERE account_id=@uid AND name=@name", conn);
                cmd.Parameters.AddWithValue("@hp", Math.Max(0, p.CurrentHp));
                cmd.Parameters.AddWithValue("@mana", Math.Max(0, p.Mana));
                cmd.Parameters.AddWithValue("@uid", _userId);
                cmd.Parameters.AddWithValue("@name", p.Name);
                cmd.ExecuteNonQuery();
            }
        }

        private void HandlePlayerDeaths()
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            foreach (var p in _players.Where(pl => pl.CurrentHp <= 0))
            {
                foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
                {
                    InventoryService.Equip(p.Name, slot, null);
                }
                string cause = _deathCauses.GetValueOrDefault(p.Name, "Unknown");
                DialogResult res = MessageBox.Show($"Send {p.Name}'s body to the nearest graveyard?", "Graveyard", MessageBoxButtons.YesNo);
                using (var deadCmd = new MySqlCommand("UPDATE characters SET is_dead=1, in_arena=0, in_tavern=0 WHERE account_id=@id AND name=@name", conn))
                {
                    deadCmd.Parameters.AddWithValue("@id", _userId);
                    deadCmd.Parameters.AddWithValue("@name", p.Name);
                    deadCmd.ExecuteNonQuery();
                }
                if (res == DialogResult.Yes)
                {
                    using var listCmd = new MySqlCommand("SELECT id FROM characters WHERE account_id=@id AND in_graveyard=1 ORDER BY death_time ASC", conn);
                    listCmd.Parameters.AddWithValue("@id", _userId);
                    var ids = new List<int>();
                    using (var r = listCmd.ExecuteReader())
                    {
                        while (r.Read()) ids.Add(r.GetInt32("id"));
                    }
                    if (ids.Count >= 3)
                    {
                        using var rem = new MySqlCommand("UPDATE characters SET in_graveyard=0 WHERE id=@cid", conn);
                        rem.Parameters.AddWithValue("@cid", ids[0]);
                        rem.ExecuteNonQuery();
                    }
                    using var add = new MySqlCommand("UPDATE characters SET in_graveyard=1, cause_of_death=@cause, death_time=NOW() WHERE account_id=@id AND name=@name", conn);
                    add.Parameters.AddWithValue("@cause", cause);
                    add.Parameters.AddWithValue("@id", _userId);
                    add.Parameters.AddWithValue("@name", p.Name);
                    add.ExecuteNonQuery();
                }
            }
        }
    }
}

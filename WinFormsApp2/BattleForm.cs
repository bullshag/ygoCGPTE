using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
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
        private readonly Dictionary<string, string> _deathCauses = new();

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

            using var cmd = new MySqlCommand("SELECT id, name, level, current_hp, max_hp, mana, strength, dex, intelligence, action_speed, melee_defense, magic_defense, role, targeting_style FROM characters WHERE account_id=@id AND is_dead=0", conn);
            cmd.Parameters.AddWithValue("@id", _userId);

            var playerIds = new Dictionary<Creature, int>();
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    int cid = r.GetInt32("id");
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
                foreach (var p in PassiveService.GetCharacterPassives(kv.Value, conn))
                {
                    kv.Key.Passives[p.Key] = p.Value;
                }
            }

            int totalLevel = _players.Sum(p => p.Level);
            int minLevel = Math.Max(1, (int)Math.Ceiling(totalLevel * 1.0));
            int maxLevel = Math.Max(minLevel, (int)Math.Ceiling(totalLevel * 1.4));
            int npcLevel = 0;
            while (_npcs.Count == 0 || npcLevel < minLevel)
            {
                using var npcCmd = new MySqlCommand("SELECT name, level, current_hp, max_hp, mana, strength, dex, intelligence, action_speed, melee_defense, magic_defense, role, targeting_style FROM npcs WHERE level <= @maxLevel ORDER BY RAND() LIMIT 1", conn);
                npcCmd.Parameters.AddWithValue("@maxLevel", Math.Max(1, maxLevel - npcLevel));
                using var r2 = npcCmd.ExecuteReader();
                if (r2.Read())
                {
                    int level = r2.GetInt32("level");
                    string name = r2.GetString("name");
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
                    _npcs.Add(npc);
                    npcLevel += level;
                }
                else
                {
                    break;
                }
            }

            if (_npcs.Count == 0)
            {
                using var npcCmd = new MySqlCommand("SELECT name, level, current_hp, max_hp, mana, strength, dex, intelligence, action_speed, melee_defense, magic_defense, role, targeting_style FROM npcs ORDER BY level ASC LIMIT 1", conn);
                using var r2 = npcCmd.ExecuteReader();
                if (r2.Read())
                {
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
                    _npcs.Add(npc);
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
                Weapon? left = p.Equipment.GetValueOrDefault(EquipmentSlot.LeftHand) as Weapon;
                Weapon? right = p.Equipment.GetValueOrDefault(EquipmentSlot.RightHand) as Weapon;
                if (left != null) speed *= (1 + left.AttackSpeedMod);
                if (right != null) speed *= (1 + right.AttackSpeedMod);
                if (left != null && right != null && left.Name != right.Name) speed *= 1.5;
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
                                AppendLog($"{c.Name} is healed for {eff.AmountPerTick}.", eff.SourceIsPlayer, true);
                            }
                            else
                            {
                                c.CurrentHp -= eff.AmountPerTick;
                                AppendLog($"{c.Name} takes {eff.AmountPerTick} {eff.Kind.ToString().ToLower()} damage.", eff.SourceIsPlayer);
                                if (c.CurrentHp <= 0 && _players.Contains(c) && !_deathCauses.ContainsKey(c.Name))
                                {
                                    _deathCauses[c.Name] = $"{c.Name} succumbed to {eff.Kind}.";
                                }
                            }
                            c.HpBar.Value = Math.Max(0, c.CurrentHp);
                            eff.TimeUntilTickMs += eff.TickIntervalMs;
                        }
                        if (eff.RemainingMs <= 0)
                        {
                            c.Effects.RemoveAt(i);
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
            };
            _progressTimer.Start();
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
            if (ability.Name == "Regenerate" || ability.Name == "Heal")
            {
                target = ChooseHealerTarget(actor, allies);
                if (target == null) return;
            }
            else
            {
                target = ChooseOpponent(actor, opponents, allies);
                if (target == null) return;
            }

            if (ability.Id != 0)
            {
                actor.Mana -= ability.Cost;
                if (actor.ManaBar.Maximum > 0)
                {
                    actor.ManaBar.Value = Math.Max(0, actor.Mana);
                }
                actor.Cooldowns[ability.Id] = ability.Cooldown * 1000;
                bool actorIsPlayer = _players.Contains(actor);
                AppendLog(GenerateAbilityLog(actor, target, ability), actorIsPlayer, ability.Name == "Heal" || ability.Name == "Regenerate");
                if (ability.Name == "Bleed") ApplyBleed(actor, target);
                else if (ability.Name == "Poison") ApplyPoison(actor, target);
                else if (ability.Name == "Regenerate") { ApplyHot(actor, target); CheckEnd(); return; }
                else if (ability.Name == "Heal")
                {
                    int healAmt = (int)Math.Max(1, 5 + actor.Intelligence * 1.2);
                    target.CurrentHp = Math.Min(target.MaxHp, target.CurrentHp + healAmt);
                    target.HpBar.Value = Math.Min(target.MaxHp, target.CurrentHp);
                    AppendLog($"{actor.Name} restores {healAmt} HP to {target.Name}!", _players.Contains(actor), true);
                    actor.AttackBar.Value = actor.AttackInterval;
                    CheckEnd();
                    return;
                }
                else if (ability.Name == "Taunting Blows") ApplyTaunt(actor, opponents);
                else if (ability.Name == "Vanish") { actor.IsVanished = true; actor.VanishRemainingMs = 5000; actor.AttackBar.Value = actor.AttackInterval; CheckEnd(); return; }
                else if (ability.Name == "Fireball" || ability.Name == "Lightning Bolt")
                {
                    int spellDamage = CalculateSpellDamage(actor, target, ability);
                    target.CurrentHp -= spellDamage;
                    string spellLog = ability.Name switch
                    {
                        "Fireball" => $"{actor.Name}'s fireball engulfs {target.Name} for {spellDamage} damage!",
                        "Lightning Bolt" => $"{actor.Name}'s lightning bolt smites {target.Name} for {spellDamage} damage!",
                        "Ice Lance" => $"{actor.Name}'s ice lance impales {target.Name} for {spellDamage} damage!",
                        "Arcane Blast" => $"{actor.Name}'s arcane blast rips through {target.Name} for {spellDamage} damage!",
                        "Shield Bash" => $"{actor.Name} shield bashes {target.Name} for {spellDamage} damage!",
                        "Drain Life" => $"{actor.Name} siphons {spellDamage} life from {target.Name}!",
                        _ => $"{actor.Name}'s {ability.Name} hits {target.Name} for {spellDamage} damage!"
                    };


                    if (target.CurrentHp <= 0 && _players.Contains(target) && !_deathCauses.ContainsKey(target.Name))
                    {
                        _deathCauses[target.Name] = spellLog;
                    }
                    AppendLog(spellLog, actorIsPlayer);
                    if (ability.Name == "Drain Life")
                    {
                        actor.CurrentHp = Math.Min(actor.MaxHp, actor.CurrentHp + spellDamage);
                        actor.HpBar.Value = Math.Max(0, actor.CurrentHp);
                        AppendLog($"{actor.Name} absorbs {spellDamage} health!", actorIsPlayer, true);
                    }

                    actor.DamageDone += spellDamage;
                    target.DamageTaken += spellDamage;
                    target.HpBar.Value = Math.Max(0, target.CurrentHp);
                    actor.AttackBar.Value = actor.AttackInterval;
                    target.Threat[actor] = target.Threat.GetValueOrDefault(actor) + spellDamage;
                    target.CurrentTarget = actor;
                    actor.CurrentTarget = target;
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

            int dmg = CalculateDamage(actor, target);
            if (target.Passives.ContainsKey("Bloodlust"))
            {
                double missing = 1 - target.CurrentHp / (double)target.MaxHp;
                double mult = 1.75 - missing;
                dmg = (int)(dmg * mult);
            }
            target.CurrentHp -= dmg;
            string attackLog = GenerateAttackLog(actor, target, dmg);
            if (target.CurrentHp <= 0 && _players.Contains(target) && !_deathCauses.ContainsKey(target.Name))
            {
                _deathCauses[target.Name] = attackLog;
            }
            AppendLog(attackLog, _players.Contains(actor));
            actor.DamageDone += dmg;
            target.DamageTaken += dmg;
            target.HpBar.Value = Math.Max(0, target.CurrentHp);
            actor.AttackBar.Value = actor.AttackInterval;
            target.Threat[actor] = target.Threat.GetValueOrDefault(actor) + dmg;
            target.CurrentTarget = actor;
            actor.CurrentTarget = target;
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
                if (actor.ManaBar.Maximum > 0)
                    actor.ManaBar.Value = actor.Mana;
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
                "Bleed" => $"{actor.Name} rends {target.Name}, drawing rivers of blood!",
                "Poison" => $"{actor.Name} envenoms {target.Name} with a vile toxin!",
                "Regenerate" => $"{actor.Name} calls forth rejuvenating winds around {target.Name}!",
                "Rejuvenate" => $"{actor.Name} bathes {target.Name} in rejuvenating energy!",
                "Heal" => $"{actor.Name} channels soothing light into {target.Name}!",
                "Stone Skin" => $"{actor.Name} hardens {target.Name}'s skin like stone!",
                "Taunting Blows" => $"{actor.Name} bellows a challenge, daring foes to attack!",
                "Shield Bash" => $"{actor.Name} slams their shield into {target.Name}!",
                "Poison Arrow" => $"{actor.Name} fires a venom-tipped arrow at {target.Name}!",
                "Cleanse" => $"{actor.Name} purges foul magic from {target.Name}!",
                "Berserk" => $"{actor.Name} enters a berserk fury!",
                "Drain Life" => $"{actor.Name} siphons vitality from {target.Name}!",
                "Vanish" => $"{actor.Name} melts into the shadows!",
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
            int amt = (int)Math.Max(1, 1 + actor.Dex * 0.50);
            target.Effects.Add(new StatusEffect { Kind = EffectKind.Poison, RemainingMs = 6000, TickIntervalMs = 1000, TimeUntilTickMs = 1000, AmountPerTick = amt, SourceIsPlayer = _players.Contains(actor) });
        }

        private void ApplyHot(Creature actor, Creature target)
        {
            int amt = (int)Math.Max(1, 1 + target.Intelligence * 0.80);
            target.Effects.Add(new StatusEffect { Kind = EffectKind.HoT, RemainingMs = 6000, TickIntervalMs = 3000, TimeUntilTickMs = 3000, AmountPerTick = amt, SourceIsPlayer = _players.Contains(actor) });
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
            var usable = abilities.Where(a => a.Id == 0 || (actor.Mana >= a.Cost && actor.Cooldowns.GetValueOrDefault(a.Id) <= 0)).ToList();
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
                string lootSummary = string.Empty;
                AppendLog(playersWin ? "Players win!" : "NPCs win!", playersWin);
                if (playersWin)
                {
                    AwardExperience(_npcs.Sum(n => n.Level));
                    var loot = LootService.GenerateLoot(_npcs.Select(n => (n.Name, n.Level)), _userId);
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
                var playerSummaries = _players.Select(p => new CombatantSummary(p.Name, p.DamageDone, p.DamageTaken));
                var enemySummaries = _npcs.Select(n => new CombatantSummary(n.Name, n.DamageDone, n.DamageTaken));
                using var summary = new BattleSummaryForm(playerSummaries, enemySummaries, playersWin, lootSummary);
                Hide();
                summary.ShowDialog(this.Owner);
                Close();
            }
        }

        private int CalculateDamage(Creature actor, Creature target)
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
            double critChance = 0.05 + actor.Dex / 5 * 0.01 + critChanceBonus;
            if (actor.Passives.TryGetValue("Deadly Strikes", out int dsLvl))
            {
                critChance += dsLvl * (actor.Dex / 10 * 0.01);
            }
            if (_rng.NextDouble() < Math.Min(1.0, critChance))
            {
                dmg = (int)(dmg * (1.5 + critDamageBonus));
                weaponDamage *= (1.5 + critDamageBonus);
            }
            if (actor.Passives.ContainsKey("Bloodlust"))
            {
                double missing = 1 - actor.CurrentHp / (double)actor.MaxHp;
                double bonusPercent = 0.02 + missing * 2;
                dmg += (int)(weaponDamage * bonusPercent);
            }
            return dmg;
        }

        private int CalculateSpellDamage(Creature actor, Creature target, Ability ability)
        {
            double baseDamage = ability.Name switch
            {
                "Fireball" => 5 + actor.Intelligence * 1.0,
                "Lightning Bolt" => 4 + actor.Intelligence * 1.2,
                "Ice Lance" => 6 + actor.Intelligence * 1.1,
                "Arcane Blast" => 8 + actor.Intelligence * 0.9,
                "Shield Bash" => 2 + actor.Strength * 0.5,
                "Drain Life" => 3 + actor.Intelligence * 0.7,

                _ => 0
            };
            return (int)Math.Max(1, baseDamage - target.MagicDefense);
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
            AppendLog($"Each party member gains {expPer} EXP!", true);
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
            public Dictionary<int, int> Cooldowns { get; } = new();
            public List<StatusEffect> Effects { get; } = new();
            public Dictionary<string, int> Passives { get; } = new();
            public Creature? ForcedTarget { get; set; }
            public int ForcedTargetMs { get; set; }
            public bool IsVanished { get; set; }
            public int VanishRemainingMs { get; set; }

            public Weapon? GetWeapon()
            {
                if (Equipment.TryGetValue(EquipmentSlot.LeftHand, out var lh) && lh is Weapon w) return w;
                if (Equipment.TryGetValue(EquipmentSlot.RightHand, out var rh) && rh is Weapon w2) return w2;
                return null;
            }
        }

        private enum EffectKind { Bleed, Poison, HoT }

        private class StatusEffect
        {
            public EffectKind Kind { get; set; }
            public int RemainingMs { get; set; }
            public int TickIntervalMs { get; set; }
            public int TimeUntilTickMs { get; set; }
            public int AmountPerTick { get; set; }
            public bool SourceIsPlayer { get; set; }
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
            c.HpBar = CloneProgressBar(hpTemplate);
            c.HpBar.Maximum = c.MaxHp;
            c.HpBar.Value = c.CurrentHp;
            c.HpBar.Location = new Point(0, 15);
            panel.Controls.Add(lbl);
            panel.Controls.Add(c.HpBar);
            if (c.MaxMana > 0)
            {
                c.ManaBar = CloneProgressBar(manaTemplate);
                c.ManaBar.Maximum = c.MaxMana;
                c.ManaBar.Value = c.Mana;
                c.ManaBar.Location = new Point(0, 35);
                panel.Controls.Add(c.ManaBar);
                c.AttackBar = CloneProgressBar(attackTemplate);
                c.AttackBar.Maximum = 100;
                c.AttackBar.Value = 100;
                c.AttackBar.Location = new Point(0, 55);
            }
            else
            {
                c.AttackBar = CloneProgressBar(attackTemplate);
                c.AttackBar.Maximum = 100;
                c.AttackBar.Value = 100;
                c.AttackBar.Location = new Point(0, 35);
            }
            panel.Controls.Add(c.AttackBar);
            return panel;
        }

        private ProgressBar CloneProgressBar(ProgressBar template)
        {
            return new ProgressBar
            {
                Width = template.Width,
                Height = template.Height,
                ForeColor = template.ForeColor,
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
                using (var deadCmd = new MySqlCommand("UPDATE characters SET is_dead=1 WHERE account_id=@id AND name=@name", conn))
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

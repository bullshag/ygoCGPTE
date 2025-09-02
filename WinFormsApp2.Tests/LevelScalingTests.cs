using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using WinFormsApp2;
using Xunit;

namespace WinFormsApp2.Tests;

public class LevelScalingTests
{
    private static readonly Type BattleFormType = typeof(BattleForm);
    private static readonly Type CreatureType = BattleFormType.GetNestedType("Creature", BindingFlags.NonPublic)!;
    private static readonly Type StatusEffectType = BattleFormType.GetNestedType("StatusEffect", BindingFlags.NonPublic)!;

    private static BattleForm CreateBattleForm()
    {
        var bf = (BattleForm)FormatterServices.GetUninitializedObject(BattleFormType);
        BattleFormType.GetField("_rng", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(bf, new Random(0));
        BattleFormType.GetField("_players", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(bf, Activator.CreateInstance(typeof(List<>).MakeGenericType(CreatureType)));
        BattleFormType.GetField("_npcs", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(bf, Activator.CreateInstance(typeof(List<>).MakeGenericType(CreatureType)));
        BattleFormType.GetField("_deathCauses", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(bf, new Dictionary<string, string>());
        BattleFormType.GetField("_battleEnded", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(bf, true);
        BattleFormType.GetField("_gameTimer", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(bf, new Timer());
        BattleFormType.GetField("lstLog", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(bf, new ListBox());
        return bf;
    }

    private static object CreateCreature(int level = 0, int str = 0, int dex = 0, int intel = 0)
    {
        var c = Activator.CreateInstance(CreatureType)!;
        CreatureType.GetProperty("Level")!.SetValue(c, level);
        CreatureType.GetProperty("Strength")!.SetValue(c, str);
        CreatureType.GetProperty("Dex")!.SetValue(c, dex);
        CreatureType.GetProperty("Intelligence")!.SetValue(c, intel);
        CreatureType.GetProperty("MaxHp")!.SetValue(c, 100);
        CreatureType.GetProperty("CurrentHp")!.SetValue(c, 50);
        CreatureType.GetProperty("Mana")!.SetValue(c, 100);
        CreatureType.GetProperty("MaxMana")!.SetValue(c, 100);
        CreatureType.GetProperty("MeleeDefense")!.SetValue(c, 0);
        CreatureType.GetProperty("MagicDefense")!.SetValue(c, 0);
        CreatureType.GetProperty("DamageDealtMultiplier")!.SetValue(c, 1.0);
        CreatureType.GetProperty("DamageTakenMultiplier")!.SetValue(c, 1.0);
        CreatureType.GetProperty("HealingDealtMultiplier")!.SetValue(c, 1.0);
        CreatureType.GetProperty("HealingReceivedMultiplier")!.SetValue(c, 1.0);
        return c;
    }

    [Fact]
    public void SpellDamage_Increases_By_Level()
    {
        var bf = CreateBattleForm();
        var ability = new Ability { Name = "Test", Description = "10 + 0% of your INT" };
        var method = BattleFormType.GetMethod("CalculateSpellDamage", BindingFlags.Instance | BindingFlags.NonPublic)!;

        var actor0 = CreateCreature(level: 0, intel: 5);
        var target0 = CreateCreature();
        int dmg0 = (int)method.Invoke(bf, new object[] { actor0, target0, ability })!;

        var actor5 = CreateCreature(level: 5, intel: 5);
        var target5 = CreateCreature();
        int dmg5 = (int)method.Invoke(bf, new object[] { actor5, target5, ability })!;

        Assert.Equal(dmg0 + 5, dmg5);
    }

    [Fact]
    public void PhysicalAbilityDamage_Increases_By_Level()
    {
        var bf = CreateBattleForm();
        var method = BattleFormType.GetMethod("CalculateDamage", BindingFlags.Instance | BindingFlags.NonPublic)!;

        var actor = CreateCreature(level: 5);
        var target = CreateCreature();
        CreatureType.GetProperty("NoCrits")!.SetValue(target, true);

        int basic = (int)method.Invoke(bf, new object[] { actor, target, false })!;
        int ability = (int)method.Invoke(bf, new object[] { actor, target, true })!;

        Assert.Equal(basic + 5, ability);
    }

    [Fact]
    public void DirectHeal_Increases_By_Level()
    {
        int HealForLevel(int lvl)
        {
            var bf = CreateBattleForm();
            var actor = CreateCreature(level: lvl, intel: 10);
            CreatureType.GetProperty("Role")!.SetValue(actor, "DPS");
            var ability = new Ability { Id = 1, Name = "Heal", Cost = 0, Cooldown = 0, Priority = 1 };
            var abilities = (IList)CreatureType.GetProperty("Abilities")!.GetValue(actor)!;
            abilities.Add(ability);

            var allies = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(CreatureType))!;
            allies.Add(actor);
            var opponents = Activator.CreateInstance(typeof(List<>).MakeGenericType(CreatureType))!;

            int before = (int)CreatureType.GetProperty("CurrentHp")!.GetValue(actor)!;
            var actMethod = BattleFormType.GetMethod("Act", BindingFlags.Instance | BindingFlags.NonPublic)!;
            actMethod.Invoke(bf, new object[] { actor, allies, opponents });
            int after = (int)CreatureType.GetProperty("CurrentHp")!.GetValue(actor)!;
            return after - before;
        }

        int heal0 = HealForLevel(0);
        int heal5 = HealForLevel(5);
        Assert.Equal(heal0 + 5, heal5);
    }

    [Fact]
    public void DotAndHot_NotAffectedByLevel()
    {
        int Bleed(int lvl)
        {
            var bf = CreateBattleForm();
            var actor = CreateCreature(level: lvl, str: 10);
            var target = CreateCreature();
            var method = BattleFormType.GetMethod("ApplyBleed", BindingFlags.Instance | BindingFlags.NonPublic)!;
            method.Invoke(bf, new object[] { actor, target });
            var effects = (IList)CreatureType.GetProperty("Effects")!.GetValue(target)!;
            var effect = effects[0];
            return (int)StatusEffectType.GetProperty("AmountPerTick")!.GetValue(effect)!;
        }

        int Hot(int lvl)
        {
            var bf = CreateBattleForm();
            var actor = CreateCreature(level: lvl);
            var target = CreateCreature(intel: 10);
            var method = BattleFormType.GetMethod("ApplyHot", BindingFlags.Instance | BindingFlags.NonPublic)!;
            method.Invoke(bf, new object[] { actor, target });
            var effects = (IList)CreatureType.GetProperty("Effects")!.GetValue(target)!;
            var effect = effects[0];
            return (int)StatusEffectType.GetProperty("AmountPerTick")!.GetValue(effect)!;
        }

        Assert.Equal(Bleed(0), Bleed(5));
        Assert.Equal(Hot(0), Hot(5));
    }
}

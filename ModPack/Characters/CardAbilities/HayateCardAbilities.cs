using ModPack21341.Characters.Buffs;
using ModPack21341.Utilities;
using Sound;

namespace ModPack21341.Characters.CardAbilities
{
    public class DiceCardSelfAbility_Fingersnap : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Kill the Target and make them disappear from the field. This page cannot be redirected";
        public override void OnStartBattle()
        {
            SoundEffectPlayer.PlaySound("Creature/FingerSnap");
            card.target.Die(owner);
            BattleObjectManager.instance.UnregisterUnit(card.target);
            UnitUtilities.RefreshCombatUI();
        }

        public override bool IsTargetChangable(BattleUnitModel attacker) => false;
    }
    public class DiceCardSelfAbility_FingersnapEnd2Phase : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Kill all enemies on the field and make them disappear";
        public override void OnStartBattle()
        {
            SoundEffectPlayer.PlaySound("Creature/FingerSnap");
            foreach (var unit in BattleObjectManager.instance.GetAliveList(Faction.Player))
            {
                unit.Die();
                BattleObjectManager.instance.UnregisterUnit(unit);
            }
            UnitUtilities.RefreshCombatUI();
        }
    }
    public class DiceCardSelfAbility_FingersnapPlayer : DiceCardSelfAbilityBase
    {
        public static string Desc = "[Single Use]\nCan only be used at Emotion Level 5 and require 100 stacks of [Entertain Me!] buff.[On Use] [Kill] all enemies where [Max Hp] are lower then 250.If the [Max Hp] of the enemy is higher then 250, do 100 [Fixed Damage] instead.";
        public override bool OnChooseCard(BattleUnitModel owner) => owner.emotionDetail.EmotionLevel >= 4 && owner.bufListDetail.GetActivatedBufList().Find(x => x is BattleUnitBuf_EntertainMeBuf).stack >= 100;
        public override void OnStartBattle()
        {
            if (owner.bufListDetail.GetActivatedBufList().Find(x => x is BattleUnitBuf_EntertainMeBuf) is BattleUnitBuf_EntertainMeBuf buf)
            {
                buf.stack = 0;
            }
            SoundEffectPlayer.PlaySound("Creature/FingerSnap");
            foreach (var unit in BattleObjectManager.instance.GetAliveList(Faction.Enemy))
            {
                if (unit.MaxHp < 250)
                    unit.Die(owner);
                else
                {
                    unit.TakeDamage(100, DamageType.ETC);
                    unit.breakDetail.TakeBreakDamage(100, DamageType.ETC);
                }
            }
        }
    }

    public class DiceCardSelfAbility_Str1Light2 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use]Restore 2 Light,Gain 1 Strength next Scene";
        public override void OnUseCard()
        {
            owner.cardSlotDetail.RecoverPlayPoint(2);
            owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 1, owner);
        }
    }

    public class DiceCardSelfAbility_TrueGodAura : DiceCardSelfAbilityBase
    {
        public static string Desc = "Can be used at Emotion Level 4 or above\n[On Use] Unleash The True Power of a God next Scene";
        public override bool OnChooseCard(BattleUnitModel owner) => owner.emotionDetail.EmotionLevel >= 4;
    }
}

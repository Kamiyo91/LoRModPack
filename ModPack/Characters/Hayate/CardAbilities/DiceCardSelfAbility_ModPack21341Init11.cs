using ModPack21341.Characters.Hayate.Buffs;
using Sound;

namespace ModPack21341.Characters.Hayate.CardAbilities
{
    //FingersnapPlayer
    public class DiceCardSelfAbility_ModPack21341Init11 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[Single Use]\nCan only be used at Emotion Level 5 and require 100 stacks of [Entertain Me!] buff.[On Use] [Kill] all enemies where [Max Hp] are lower then 250.If the [Max Hp] of the enemy is higher then 250, do 100 [Fixed Damage] instead.";

        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return owner.emotionDetail.EmotionLevel >= 4 && owner.bufListDetail.GetActivatedBufList()
                .Find(x => x is BattleUnitBuf_ModPack21341Init8).stack >= 100;
        }

        public override void OnStartBattle()
        {
            if (owner.bufListDetail.GetActivatedBufList().Find(x => x is BattleUnitBuf_ModPack21341Init8) is
                BattleUnitBuf_ModPack21341Init8 buf) buf.stack = 0;
            SoundEffectPlayer.PlaySound("Creature/FingerSnap");
            foreach (var unit in BattleObjectManager.instance.GetAliveList(Faction.Enemy))
                if (unit.MaxHp < 250)
                {
                    unit.Die(owner);
                }
                else
                {
                    unit.TakeDamage(100, DamageType.ETC);
                    unit.breakDetail.TakeBreakDamage(100, DamageType.ETC);
                }
        }
    }
}
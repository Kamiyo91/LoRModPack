using LOR_DiceSystem;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.Kamiyo.PassiveAbilities
{
    //KurosawaBlade
    public class PassiveAbility_ModPack21341Init31 : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail != BehaviourDetail.Slash) return;
            UnitUtilities.SetPassiveCombatLog(this, owner);
            behavior.ApplyDiceStatBonus(new DiceStatBonus {power = 1});
        }

        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            RecoverHpAndStagger();
        }

        private void RecoverHpAndStagger()
        {
            owner.RecoverHP(2);
            owner.breakDetail.RecoverBreak(2);
        }
    }
}
using ModPack21341.Utilities;

namespace ModPack21341.Characters.Hayate.PassiveAbilities
{
    //TrueKurosawaBlade
    public class PassiveAbility_ModPack21341Init26 : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            UnitUtilities.SetPassiveCombatLog(this, owner);
            behavior.ApplyDiceStatBonus(new DiceStatBonus {power = 1});
        }

        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            RecoverHpAndStagger();
        }

        private void RecoverHpAndStagger()
        {
            owner.RecoverHP(3);
            owner.breakDetail.RecoverBreak(3);
        }
    }
}
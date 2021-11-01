using ModPack21341.Utilities;

namespace ModPack21341.Characters.OldSamurai.PassiveAbilities
{
    //DeepBreath
    public class PassiveAbility_ModPack21341Init42 : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            UnitUtilities.SetPassiveCombatLog(this, owner);
            behavior.ApplyDiceStatBonus(
                new DiceStatBonus
                {
                    max = 3
                });
        }
    }
}
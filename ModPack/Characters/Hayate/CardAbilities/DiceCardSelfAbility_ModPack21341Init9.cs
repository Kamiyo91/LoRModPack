using ModPack21341.Utilities;
using Sound;

namespace ModPack21341.Characters.Hayate.CardAbilities
{
    //Fingersnap
    public class DiceCardSelfAbility_ModPack21341Init9 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[On Use] Kill the Target and make them disappear from the field. This page cannot be redirected";

        public override void OnStartBattle()
        {
            SoundEffectPlayer.PlaySound("Creature/FingerSnap");
            card.target.Die(owner);
            BattleObjectManager.instance.UnregisterUnit(card.target);
            UnitUtilities.RefreshCombatUI();
        }

        public override bool IsTargetChangable(BattleUnitModel attacker)
        {
            return false;
        }
    }
}
using ModPack21341.Utilities;
using Sound;

namespace ModPack21341.Characters.Hayate.CardAbilities
{
    //FingersnapEnd2Phase
    public class DiceCardSelfAbility_ModPack21341Init10 : DiceCardSelfAbilityBase
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
}
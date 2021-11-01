using ModPack21341.Characters.OldSamurai.DiceCardAbilities;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.OldSamurai.PassiveAbilities
{
    //ZeroBlade
    public class PassiveAbility_ModPack21341Init46 : PassiveAbilityBase
    {
        private bool _counterReload;

        public override void OnStartBattle()
        {
            UnitUtilities.ReadyCounterCard(owner, 2);
        }

        public override void OnLoseParrying(BattleDiceBehavior behavior)
        {
            _counterReload = false;
        }

        public override void OnDrawParrying(BattleDiceBehavior behavior)
        {
            _counterReload = false;
        }

        public override void OnWinParrying(BattleDiceBehavior behavior)
        {
            _counterReload = behavior.abilityList.Exists(x => x is DiceCardAbility_ModPack21341Init2);
        }

        public override void OnEndBattle(BattlePlayingCardDataInUnitModel curCard)
        {
            if (!_counterReload) return;
            _counterReload = false;
            UnitUtilities.SetPassiveCombatLog(this, owner);
            UnitUtilities.ReadyCounterCard(owner, 2);
        }
    }
}
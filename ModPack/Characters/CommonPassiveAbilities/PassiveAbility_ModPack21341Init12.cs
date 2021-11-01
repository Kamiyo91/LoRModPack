using ModPack21341.Harmony;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.CommonPassiveAbilities
{
    //EvadeCounter
    public class PassiveAbility_ModPack21341Init12 : PassiveAbilityBase
    {
        private bool _recoveryCheck;

        public override void OnStartBattle()
        {
            _recoveryCheck = false;
            UnitUtilities.ReadyCounterCard(owner, 30);
        }

        public override void OnWinParrying(BattleDiceBehavior behavior)
        {
            if (behavior.card.card.GetID() != new LorId(ModPack21341Init.PackageId, 30)) return;
            if (_recoveryCheck) return;
            _recoveryCheck = true;
            UnitUtilities.SetPassiveCombatLog(this, owner);
            owner.cardSlotDetail.RecoverPlayPoint(1);
        }
    }
}
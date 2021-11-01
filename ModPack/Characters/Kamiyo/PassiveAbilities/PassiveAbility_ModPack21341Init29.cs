using ModPack21341.Characters.Hayate.Buffs;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.Kamiyo.PassiveAbilities
{
    //KamiyoHayate
    public class PassiveAbility_ModPack21341Init29 : PassiveAbilityBase
    {
        public override void Init(BattleUnitModel self)
        {
            base.Init(self);
            Hide();
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.TargetDice.owner.bufListDetail.GetActivatedBufList()
                .Exists(x => x is BattleUnitBuf_ModPack21341Init10))
                UnitUtilities.SetPassiveCombatLog(this, owner);
        }
    }
}
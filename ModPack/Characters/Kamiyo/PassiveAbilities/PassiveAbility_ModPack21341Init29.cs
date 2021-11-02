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
            UnitUtilities.SetPassiveCombatLog(this, owner);
        }

        public override void OnRoundEnd_before()
        {
            if (BattleObjectManager.instance.GetAliveList(Faction.Enemy).Count < 1) owner.Die();
        }
    }
}
using ModPack21341.Utilities;

namespace ModPack21341.Characters.Kamiyo.PassiveAbilities
{
    //OverflowingFire
    public class PassiveAbility_ModPack21341Init33 : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            foreach (var unit in BattleObjectManager.instance.GetAliveList(owner.faction == Faction.Player
                ? Faction.Enemy
                : Faction.Player)) unit.bufListDetail.AddKeywordBufByCard(KeywordBuf.Burn, 1, unit);
        }

        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            UnitUtilities.SetPassiveCombatLog(this, owner);
            behavior.card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Burn, 1, behavior.card.target);
        }
    }
}
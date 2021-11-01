using LOR_DiceSystem;
using ModPack21341.Characters.PurpleTear.PassiveAbilities;

namespace ModPack21341.Characters.PurpleTear.Buffs
{
    //CustomPurpleHit
    public class BattleUnitBuf_ModPack21341Init22 : BattleUnitBuf
    {
        public override KeywordBuf bufType => KeywordBuf.PurpleHit;

        protected override string keywordId => !PassiveExists() ? "" : "StanceHit";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (!PassiveExists() || behavior.Detail != BehaviourDetail.Hit)
                return;
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = 2,
                breakRate = 50
            });
        }

        private bool PassiveExists()
        {
            return _owner.passiveDetail.HasPassive<PassiveAbility_ModPack21341Init47>();
        }
    }
}
using LOR_DiceSystem;
using ModPack21341.Characters.PurpleTear.PassiveAbilities;

namespace ModPack21341.Characters.PurpleTear.Buffs
{
    //CustomPurplePenetrate
    public class BattleUnitBuf_ModPack21341Init24 : BattleUnitBuf
    {
        public override KeywordBuf bufType => KeywordBuf.PurplePenetrate;

        protected override string keywordId => !PassiveExists() ? "" : "StancePenetrate";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (!PassiveExists() || behavior.Detail != BehaviourDetail.Penetrate)
                return;
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = 2
            });
        }

        public override int GetMultiplierOnGiveKeywordBufByCard(
            BattleUnitBuf cardBuf,
            BattleUnitModel target)
        {
            return cardBuf.positiveType == BufPositiveType.Negative ? 2 : 1;
        }

        private bool PassiveExists()
        {
            return _owner.passiveDetail.HasPassive<PassiveAbility_ModPack21341Init47>();
        }
    }
}
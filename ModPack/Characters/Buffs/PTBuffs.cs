using LOR_DiceSystem;

namespace ModPack21341.Characters.Buffs
{
    public class BattleUnitBuf_CustomPurpleSlash : BattleUnitBuf
    {
        public override KeywordBuf bufType => KeywordBuf.PurpleSlash;

        protected override string keywordId => !PassiveExists() ? "" : "StanceSlash";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (!PassiveExists() || behavior.Detail != BehaviourDetail.Slash)
                return;
            behavior.ApplyDiceStatBonus(new DiceStatBonus()
            {
                power = 2,
                dmgRate = 50
            });
        }

        private bool PassiveExists() => _owner.passiveDetail.HasPassive<PassiveAbility_CustomPTSkinStance>();
    }
    public class BattleUnitBuf_CustomPurplePenetrate : BattleUnitBuf
    {
        public override KeywordBuf bufType => KeywordBuf.PurplePenetrate;

        protected override string keywordId => !PassiveExists() ? "" : "StancePenetrate";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (!PassiveExists() || behavior.Detail != BehaviourDetail.Penetrate)
                return;
            behavior.ApplyDiceStatBonus(new DiceStatBonus()
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

        private bool PassiveExists() => _owner.passiveDetail.HasPassive<PassiveAbility_CustomPTSkinStance>();
    }
    public class BattleUnitBuf_CustomPurpleHit : BattleUnitBuf
    {
        public override KeywordBuf bufType => KeywordBuf.PurpleHit;

        protected override string keywordId => !PassiveExists() ? "" : "StanceHit";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (!PassiveExists() || behavior.Detail != BehaviourDetail.Hit)
                return;
            behavior.ApplyDiceStatBonus(new DiceStatBonus()
            {
                power = 2,
                breakRate = 50
            });
        }

        private bool PassiveExists() => _owner.passiveDetail.HasPassive<PassiveAbility_CustomPTSkinStance>();
    }
    public class BattleUnitBuf_CustomPurpleDefense : BattleUnitBuf
    {
        public override KeywordBuf bufType => KeywordBuf.PurpleDefense;

        protected override string keywordId => !PassiveExists() ? "" : "StanceDefense";

        public override bool IsImmune(BufPositiveType posType) => PassiveExists() && posType == BufPositiveType.Negative;

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (!PassiveExists() || !IsDefenseDice(behavior.Detail))
                return;
            behavior.ApplyDiceStatBonus(new DiceStatBonus()
            {
                power = 2
            });
        }

        private bool PassiveExists() => _owner.passiveDetail.HasPassive<PassiveAbility_CustomPTSkinStance>();
    }

}

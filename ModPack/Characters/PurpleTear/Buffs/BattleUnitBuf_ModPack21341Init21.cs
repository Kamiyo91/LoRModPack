using ModPack21341.Characters.PurpleTear.PassiveAbilities;

namespace ModPack21341.Characters.PurpleTear.Buffs
{
    //CustomPurpleDefense
    public class BattleUnitBuf_ModPack21341Init21 : BattleUnitBuf
    {
        public override KeywordBuf bufType => KeywordBuf.PurpleDefense;

        protected override string keywordId => !PassiveExists() ? "" : "StanceDefense";

        public override bool IsImmune(BufPositiveType posType)
        {
            return PassiveExists() && posType == BufPositiveType.Negative;
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (!PassiveExists() || !IsDefenseDice(behavior.Detail))
                return;
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = 2
            });
        }

        private bool PassiveExists()
        {
            return _owner.passiveDetail.HasPassive<PassiveAbility_ModPack21341Init47>();
        }
    }
}
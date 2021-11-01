namespace ModPack21341.Characters.Kamiyo.Buffs
{
    //KamiyoAndHayate
    public class BattleUnitBuf_ModPack21341Init11 : BattleUnitBuf
    {
        public BattleUnitBuf_ModPack21341Init11()
        {
            stack = 0;
        }

        protected override string keywordId => "KamiyoHayate";
        public override int paramInBufDesc => 0;
        protected override string keywordIconId => "BlackFrantic";
        public override string bufActivatedText => "Power +2 against Hayate, on battle end Die.";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus {power = 2});
        }

        public override void OnKill(BattleUnitModel target)
        {
            _owner.Die();
        }
    }
}
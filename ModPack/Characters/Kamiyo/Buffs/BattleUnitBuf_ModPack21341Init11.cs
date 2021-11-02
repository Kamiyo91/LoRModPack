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
        public override string bufActivatedText => "Power +4 against Hayate, when the battle ends,Die.";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus {power = 4});
        }

        public override void OnKill(BattleUnitModel target)
        {
            _owner.Die();
        }
    }
}
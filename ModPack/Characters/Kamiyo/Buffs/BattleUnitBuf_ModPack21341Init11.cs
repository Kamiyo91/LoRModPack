namespace ModPack21341.Characters.Kamiyo.Buffs
{
    //KamiyoAndHayate
    public class BattleUnitBuf_ModPack21341Init11 : BattleUnitBuf
    {
        public BattleUnitBuf_ModPack21341Init11()
        {
            stack = 0;
        }

        protected override string keywordId => "ModPack21341Init11";
        public override int paramInBufDesc => 0;
        protected override string keywordIconId => "BlackFrantic";

        public override string bufActivatedText =>
            "At the end of each scene full recover Light.Power +4 against Hayate, when the battle ends,Die.";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus {power = 4});
        }

        public override void OnRoundEndTheLast()
        {
            _owner.cardSlotDetail.RecoverPlayPoint(_owner.cardSlotDetail.GetMaxPlayPoint());
        }
    }
}
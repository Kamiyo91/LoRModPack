namespace ModPack21341.Characters.Buffs
{
    public class BattleUnitBuf_BlackMaskSilence : BattleUnitBuf
    {
        public BattleUnitBuf_BlackMaskSilence() => stack = 0;
        
        protected override string keywordId => "BlackMaskEgo";
        public override int paramInBufDesc => 0;
        protected override string keywordIconId => "BlackFrantic";
        public override string bufActivatedText => "Power +2,draw 1 additional Card and Restore 1 Light each scene";
        public override bool isAssimilation => true;

        public override void BeforeRollDice(BattleDiceBehavior behavior) => behavior.ApplyDiceStatBonus(
            new DiceStatBonus
            {
                power = 2
            });
        public override void OnRoundStart()
        {
            _owner.cardSlotDetail.RecoverPlayPoint(1);
            _owner.allyCardDetail.DrawCards(1);
        }
    }
}

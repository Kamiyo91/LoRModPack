﻿namespace ModPack21341.Characters.Roland.Buffs
{
    //BlackSilenceMaskBuf
    public class BattleUnitBuf_ModPack21341Init25 : BattleUnitBuf
    {
        public BattleUnitBuf_ModPack21341Init25()
        {
            stack = 0;
        }

        protected override string keywordId => "BlackMaskEgo";
        public override int paramInBufDesc => 0;
        protected override string keywordIconId => "BlackFrantic";
        public override string bufActivatedText => "Power +1,draw 1 additional Card and Restore 1 Light each scene";
        public override bool isAssimilation => true;

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(
                new DiceStatBonus
                {
                    power = 1
                });
        }

        public override void OnRoundStart()
        {
            _owner.cardSlotDetail.RecoverPlayPoint(1);
            _owner.allyCardDetail.DrawCards(1);
        }
    }
}
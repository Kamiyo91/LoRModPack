namespace ModPack21341.Characters.Roland.PassiveAbilities
{
    //TheBlackSilence
    public class PassiveAbility_ModPack21341Init51 : PassiveAbilityBase
    {
        private int _count;

        public override void OnWaveStart()
        {
            owner.allyCardDetail.DrawCards(2);
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (_count == 2)
            {
                curCard.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 2
                });
                _count = 0;
            }
            else
            {
                _count++;
            }

            owner.bufListDetail.RemoveBufAll(typeof(PassiveAbility_10013.BattleUnitBuf_blackSilenceCardCount));
            if (_count > 0)
                owner.bufListDetail.AddBuf(new PassiveAbility_10013.BattleUnitBuf_blackSilenceCardCount
                {
                    stack = _count
                });
        }
    }
}
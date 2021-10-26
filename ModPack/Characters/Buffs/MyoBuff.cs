namespace ModPack21341.Characters.Buffs
{
    public class BattleUnitBuf_MyoBerserkCustomCheck : BattleUnitBuf
    {
        public override KeywordBuf bufType => KeywordBuf.MyoBerserk;
        protected override string keywordId => "MyoBerserk";
        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            if(owner.bufListDetail.GetActivatedBufList().Exists(x => x is DiceCardSelfAbility_myoBerserk.BattleUnitBuf_myoBerserk)) Destroy();
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
            {
                owner.view.SetAltSkin("Myo2");
                owner.view.charAppearance.SetAltMotion(ActionDetail.Fire, ActionDetail.Penetrate);
            }
            foreach (var battleDiceCardModel in _owner.allyCardDetail.GetAllDeck())
            {
                battleDiceCardModel.ChangeFarToNearForMyo();
            }
        }

        public override void OnRoundStart()
        {
            if (_owner.bufListDetail.GetActivatedBufList().Exists(x => x is DiceCardSelfAbility_myoBerserk.BattleUnitBuf_myoBerserk)) Destroy();
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = 1
            });
        }
    }
}

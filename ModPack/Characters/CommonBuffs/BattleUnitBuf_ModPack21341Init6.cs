namespace ModPack21341.Characters.CommonBuffs
{
    //MyoBerserkCustomCheck
    public class BattleUnitBuf_ModPack21341Init6 : BattleUnitBuf
    {
        private bool _init;
        public override KeywordBuf bufType => KeywordBuf.MyoBerserk;
        protected override string keywordId => "MyoBerserk";

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            _init = false;
        }

        public override void OnRoundEndTheLast()
        {
            if (_init) return;
            if (_owner.bufListDetail.GetActivatedBufList()
                .Exists(x => x is DiceCardSelfAbility_myoBerserk.BattleUnitBuf_myoBerserk)) Destroy();
            if (string.IsNullOrEmpty(_owner.UnitData.unitData.workshopSkin) &&
                _owner.UnitData.unitData.bookItem == _owner.UnitData.unitData.CustomBookItem &&
                _owner.UnitData.unitData.bookItem.BookId.id == 250024 ||
                _owner.UnitData.unitData.CustomBookItem.BookId.id == 250024)
            {
                _owner.view.SetAltSkin("Myo2");
                _owner.view.charAppearance.SetAltMotion(ActionDetail.Fire, ActionDetail.Penetrate);
            }

            foreach (var battleDiceCardModel in _owner.allyCardDetail.GetAllDeck())
                battleDiceCardModel.ChangeFarToNearForMyo();
            _init = true;
        }

        public override void OnRoundStart()
        {
            if (_owner.bufListDetail.GetActivatedBufList()
                .Exists(x => x is DiceCardSelfAbility_myoBerserk.BattleUnitBuf_myoBerserk)) Destroy();
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
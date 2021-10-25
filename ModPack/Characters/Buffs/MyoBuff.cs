using LOR_DiceSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModPack21341.Characters.Buffs
{
    public class BattleUnitBuf_myoBerserkCustomCheck : BattleUnitBuf
    {
        private bool _init;
        public override KeywordBuf bufType => KeywordBuf.MyoBerserk;
        protected override string keywordId => "MyoBerserk";
        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
            {
                owner.view.SetAltSkin("Myo2");
                owner.view.charAppearance.SetAltMotion(ActionDetail.Fire, ActionDetail.Penetrate);
            }
            foreach (var battleDiceCardModel in _owner.allyCardDetail.GetAllDeck())
            {
                if (string.IsNullOrEmpty(_owner.UnitData.unitData.workshopSkin) &&
                    _owner.UnitData.unitData.bookItem == _owner.UnitData.unitData.CustomBookItem)
                    battleDiceCardModel.ChangeFarToNearForMyo();
                else
                    ChangeFarToNearForMyoCustom(battleDiceCardModel);
            }
        }
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = 1
            });
        }

        private void ChangeFarToNearForMyoCustom(BattleDiceCardModel card)
        {
            if (ItemXmlDataList.instance.GetCardItem(this._xmlData.id) != null)
            {
                if (this._xmlData.Spec.Ranged != CardRange.Far)
                    return;
            }
            else if (this._xmlData.Spec.Ranged != CardRange.Far)
                return;
            DiceCardSpec diceCardSpec = this._xmlData.Spec.Copy();
            diceCardSpec.Ranged = CardRange.Near;
            this._xmlData.Spec = diceCardSpec;
            List<DiceBehaviour> diceBehaviourList = new List<DiceBehaviour>();
            foreach (DiceBehaviour diceBehaviour1 in this._xmlData.DiceBehaviourList)
            {
                DiceBehaviour diceBehaviour2 = diceBehaviour1.Copy();
                diceBehaviour2.EffectRes = "RCorp_Z";
                diceBehaviourList.Add(diceBehaviour2);
            }
            this._xmlData.DiceBehaviourList = diceBehaviourList;
        }
    }
}

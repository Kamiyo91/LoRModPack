using System;
using ModPack21341.Characters.PurpleTear.PassiveAbilities;

namespace ModPack21341.Characters.PurpleTear.CardAbilities
{
    //CustomPierceStance
    public class DiceCardSelfAbility_ModPack21341Init28 : DiceCardSelfAbilityBase
    {
        public override bool OnChooseCard(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetActivatedBuf(KeywordBuf.PurpleCoolTime) != null) return false;

            return owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init47) is
                       PassiveAbility_ModPack21341Init47 passiveAbility &&
                   passiveAbility.CurrentStance != PurpleStance.Penetrate;
        }

        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            try
            {
                var passiveAbility =
                    unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init47) as
                        PassiveAbility_ModPack21341Init47;
                passiveAbility?.ChangeStance_penetrate();
                unit.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.PenetratePowerUp, 1);
                unit.bufListDetail.AddBuf(new BattleUnitBuf_purpleCooltime());
                if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                    owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
                    SingletonBehavior<BattleManagerUI>.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
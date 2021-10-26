using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModPack21341.Characters.CardAbilities
{
    public class DiceCardSelfAbility_CustomSlashStance : DiceCardSelfAbilityBase
    {
        public override bool OnChooseCard(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetActivatedBuf(KeywordBuf.PurpleCoolTime) != null)
            {
                return false;
            }

            return owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_CustomPTSkinStance) is PassiveAbility_CustomPTSkinStance passiveAbility && passiveAbility.CurrentStance != PurpleStance.Slash;
        }
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            try
            {
                var passiveAbility = unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_CustomPTSkinStance) as PassiveAbility_CustomPTSkinStance;
                passiveAbility?.ChangeStance_slash();
                unit.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.SlashPowerUp, 1);
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
    public class DiceCardSelfAbility_CustomDefenseStance : DiceCardSelfAbilityBase
    {
        public override bool OnChooseCard(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetActivatedBuf(KeywordBuf.PurpleCoolTime) != null)
            {
                return false;
            }

            return owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_CustomPTSkinStance) is PassiveAbility_CustomPTSkinStance passiveAbility && passiveAbility.CurrentStance != PurpleStance.Defense;
        }
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            try
            {
                var passiveAbility = unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_CustomPTSkinStance) as PassiveAbility_CustomPTSkinStance;
                passiveAbility?.ChangeStance_defense();
                unit.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Endurance, 1);
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
    public class DiceCardSelfAbility_CustomHitStance : DiceCardSelfAbilityBase
    {
        public override bool OnChooseCard(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetActivatedBuf(KeywordBuf.PurpleCoolTime) != null)
            {
                return false;
            }

            return owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_CustomPTSkinStance) is PassiveAbility_CustomPTSkinStance passiveAbility && passiveAbility.CurrentStance != PurpleStance.Hit;
        }
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            try
            {
                var passiveAbility = unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_CustomPTSkinStance) as PassiveAbility_CustomPTSkinStance;
                passiveAbility?.ChangeStance_hit();
                unit.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.HitPowerUp, 1);
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
    public class DiceCardSelfAbility_CustomPierceStance : DiceCardSelfAbilityBase
    {
        public override bool OnChooseCard(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetActivatedBuf(KeywordBuf.PurpleCoolTime) != null)
            {
                return false;
            }

            return owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_CustomPTSkinStance) is PassiveAbility_CustomPTSkinStance passiveAbility && passiveAbility.CurrentStance != PurpleStance.Penetrate;
        }
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            try
            {
                var passiveAbility = unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_CustomPTSkinStance) as PassiveAbility_CustomPTSkinStance;
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

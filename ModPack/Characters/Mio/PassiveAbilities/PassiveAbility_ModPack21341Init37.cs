using LOR_XML;
using ModPack21341.Characters.CommonBuffs;
using ModPack21341.Characters.Mio.Buffs;
using ModPack21341.Harmony;
using ModPack21341.StageManager;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.Mio.PassiveAbilities
{
    //God Fragment
    public class PassiveAbility_ModPack21341Init37 : PassiveAbilityBase
    {
        private bool _auraCheck;
        private bool _deathCheck;
        private BattleDialogueModel _dlg;
        private BookModel _originalBook;
        private bool _phaseChanged;
        private bool _specialCase;
        private bool _usedMassEgo;

        public override void OnBattleEnd()
        {
            if (owner.faction == Faction.Player && !_specialCase)
                UnitUtilities.ReturnToTheOriginalPlayerUnit(owner, _originalBook, _dlg);
        }

        public void SetSpecialCase()
        {
            _specialCase = true;
        }

        private void SetPhaseChange()
        {
            _phaseChanged = true;
            _auraCheck = true;
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ModPack21341Init4());
            owner.SetHp(owner.MaxHp / 2);
            if (Singleton<StageController>.Instance.EnemyStageManager != null &&
                Singleton<StageController>.Instance.EnemyStageManager is EnemyTeamStageManager_ModPack21341Init2 stage)
                stage.SetPhaseChange();
        }

        private void SetMassAttackReady()
        {
            owner.RecoverHP(owner.MaxHp * 45 / 100);
            if (owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init38) is
                PassiveAbility_ModPack21341Init38 passive) passive.SetCountValue(4);
        }

        private void ImmortalOnLethalDamage()
        {
            _deathCheck = true;
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ModPack21341Init4());
            owner.SetHp(1);
            owner.breakDetail.nextTurnBreak = false;
            owner.breakDetail.ResetGauge();
            owner.breakDetail.RecoverBreakLife(1, true);
            owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "0");
        }

        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            if (owner.faction == Faction.Enemy && owner.hp - dmg <= owner.MaxHp * 0.5f && !_phaseChanged)
                SetPhaseChange();
            if (!(owner.hp - dmg <= 0) || _deathCheck)
                return base.BeforeTakeDamage(attacker, dmg);
            ImmortalOnLethalDamage();
            if (owner.faction == Faction.Enemy)
                SetMassAttackReady();
            else
                owner.RecoverHP(owner.MaxHp * 70 / 100);
            return base.BeforeTakeDamage(attacker, dmg);
        }

        public override void OnStartBattle()
        {
            RemoveImmortalBuff();
        }

        private void RemoveImmortalBuff()
        {
            if (owner.bufListDetail.GetActivatedBufList().Find(x => x is BattleUnitBuf_ModPack21341Init4) is
                BattleUnitBuf_ModPack21341Init4 buf)
                owner.bufListDetail.RemoveBuf(buf);
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            OnAttackEgoCardUse(curCard);
            OnEgoCardUse(curCard);
        }

        private void OnAttackEgoCardUse(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 904))
                owner.personalEgoDetail.RemoveCard(curCard.card.GetID());
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 19) ||
                curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 25))
                owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 905))
                _usedMassEgo = true;
        }

        private void OnEgoCardUse(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetID() != new LorId(ModPack21341Init.PackageId, 903)) return;
            _auraCheck = true;
            owner.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 903));
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 904));
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 905));
        }

        private void StartEgoTransform()
        {
            _auraCheck = false;
            owner.breakDetail.ResetGauge();
            owner.breakDetail.RecoverBreakLife(1, true);
            owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint());
            if (owner.faction == Faction.Enemy)
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init13());
            else
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init15());
            owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "1");
        }


        public override void OnRoundEndTheLast()
        {
            if (!_auraCheck) return;
            StartEgoTransform();
            UnitUtilities.ActiveAwakeningDeckPassive(owner, "Mio");
            if (owner.UnitData.unitData.bookItem != owner.UnitData.unitData.CustomBookItem) return;
            var skinId = owner.faction == Faction.Player ? 10000200 : 10000201;
            UnitUtilities.ChangeCustomSkin(owner, skinId);
            UnitUtilities.RefreshCombatUI();
        }

        public override void OnRoundStart()
        {
            if (!_usedMassEgo) return;
            _usedMassEgo = false;
            MapUtilities.ReturnFromEgoMap("Mio", owner, 2);
        }

        public override void OnWaveStart()
        {
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 903));
            InitVariables();
            InitDlgAndCheckSkin();
        }

        private void InitDlgAndCheckSkin()
        {
            if (owner.faction == Faction.Player)
                owner.UnitData.unitData.InitBattleDialogByDefaultBook(new LorId(ModPack21341Init.PackageId, 202));
            if (!string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) ||
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem) return;
            owner.view.ChangeSkin(owner.UnitData.unitData.CustomBookItem.GetCharacterName());
        }

        private void InitVariables()
        {
            _specialCase = false;
            _deathCheck = false;
            _phaseChanged = false;
            _dlg = owner.UnitData.unitData.battleDialogModel;
            _originalBook = owner.UnitData.unitData.GetCustomBookItemData();
        }

        public void SetOriginalBookForStoryBattle(BookModel book)
        {
            _originalBook = book;
        }
    }
}
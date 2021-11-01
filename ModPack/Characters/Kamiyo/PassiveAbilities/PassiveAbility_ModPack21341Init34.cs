using System.Collections.Generic;
using System.Linq;
using LOR_XML;
using ModPack21341.Characters.CommonBuffs;
using ModPack21341.Characters.Kamiyo.Buffs;
using ModPack21341.Harmony;
using ModPack21341.Models;
using ModPack21341.StageManager.MapManager.KamiyoStageMaps;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.Kamiyo.PassiveAbilities
{
    //Power of the Unknown
    public class PassiveAbility_ModPack21341Init34 : PassiveAbilityBase
    {
        private bool _auraCheck;
        private int _count;
        private bool _deathCheck;
        private bool _dialogActivated;
        private BattleDialogueModel _dlg;
        private bool _egoActivated;
        private bool _egoMap;
        private StageLibraryFloorModel _floor;
        private bool _mapChange;
        private BookModel _originalBook;
        private bool _phaseChanged;
        private bool _specialCase;
        private bool _summonMio;
        private bool _summonMioUsed;

        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            if (!(owner.hp - dmg <= 0) || _deathCheck) return base.BeforeTakeDamage(attacker, dmg);
            if (owner.faction == Faction.Player || _phaseChanged) PrepareEgoChange(true);
            return base.BeforeTakeDamage(attacker, dmg);
        }

        public void Restart()
        {
            _count = 4;
            _egoActivated = false;
            PrepareEgoChange();
            EgoChange();
        }

        public void SetEgoReadyFinalPhaseHayate()
        {
            _egoActivated = false;
            PrepareEgoChange();
            _specialCase = true;
            EgoChange();
            owner.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 929));
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

        private void PrepareEgoChange(bool lethalDamage = false)
        {
            if (!_egoActivated)
            {
                _dialogActivated = true;
                _egoActivated = true;
                _auraCheck = true;
                owner.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 928));
                owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 929));
                owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 927));
                if (owner.faction == Faction.Player)
                {
                    owner.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 38));
                    owner.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 12));
                }
            }

            if (!lethalDamage && !_deathCheck) return;
            _deathCheck = true;
            owner.SetHp(1);
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ModPack21341Init4());
            if (owner.faction == Faction.Player) owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "0");
            if (owner.faction == Faction.Player)
                owner.RecoverHP(owner.MaxHp * 60 / 100);
            else
                owner.RecoverHP(owner.MaxHp * 35 / 100);
            owner.breakDetail.nextTurnBreak = false;
            owner.breakDetail.ResetGauge();
        }

        public override void OnRoundStart()
        {
            if (owner.faction == Faction.Player && !_egoMap && _summonMioUsed &&
                BattleObjectManager.instance.GetAliveList(Faction.Player).All(x => x == owner))
                RemoveEgoMap();
            if (_summonMioUsed)
                SingletonBehavior<BattleSoundManager>.Instance.CheckTheme();
            if (!_dialogActivated) return;
            _dialogActivated = false;
            if (!_specialCase)
                UnitUtilities.BattleAbDialog(owner.view.dialogUI, new List<AbnormalityCardDialog>
                {
                    new AbnormalityCardDialog {id = "Kamiyo", dialog = "You need me,Don't you?"},
                    new AbnormalityCardDialog {id = "Kamiyo", dialog = "Leave it to me!Time to show what I can do!"},
                    new AbnormalityCardDialog {id = "Kamiyo", dialog = "Mhm...I guess is my turn now!"}
                });
            else
                UnitUtilities.BattleAbDialog(owner.view.dialogUI,
                    new List<AbnormalityCardDialog>
                        {new AbnormalityCardDialog {id = "Kamiyo", dialog = "Time to end this Hayate!!!"}});
        }

        private void RemoveEgoMap()
        {
            _egoMap = true;
            MapUtilities.RemoveValueInEgoMap("Kamiyo2");
            MapUtilities.ReturnFromEgoMap("Kamiyo2", owner, 1);
            SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(SingletonBehavior<BattleSceneRoot>
                .Instance.currentMapObject.mapBgm);
            SingletonBehavior<BattleSoundManager>.Instance.CheckTheme();
        }

        public override void OnRoundEndTheLast()
        {
            if (_phaseChanged && owner.faction == Faction.Enemy)
                owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint());
            CheckEgoMapUse();
            if (!_auraCheck) return;
            EgoChange();
            if (!_specialCase)
                UnitUtilities.ActiveAwakeningDeckPassive(owner, "Kamiyo");
        }

        public override void OnDie()
        {
            if (!_summonMioUsed || owner.faction == Faction.Enemy && _phaseChanged) return;
            var mio = BattleObjectManager.instance.GetAliveList(owner.faction).FirstOrDefault(x => x != owner);
            mio?.Die();
        }

        private void CheckEgoMapUse()
        {
            if (!_mapChange) return;
            _mapChange = false;
            ChangeToEgoMap();
        }

        private static void ChangeToEgoMap()
        {
            MapUtilities.ChangeMap(new MapModel
            {
                Stage = "Kamiyo2",
                StageId = 5,
                IsPlayer = true,
                Component = new ModPack21341InitKamiyo2PlayerMapManager(),
                Bgy = 0.475f,
                Fy = 0.225f
            });
        }

        private void EgoChange()
        {
            if (!owner.bufListDetail.GetActivatedBufList().Exists(x => x is BattleUnitBuf_ModPack21341Init12))
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init12());
            owner.breakDetail.ResetGauge();
            owner.breakDetail.nextTurnBreak = false;
            owner.breakDetail.RecoverBreakLife(1, true);
            owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint());
            if (owner.faction == Faction.Player)
            {
                owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "1");
                if (owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem &&
                    owner.faction == Faction.Player)
                {
                    UnitUtilities.ChangeCustomSkin(owner, 10000202);
                    UnitUtilities.RefreshCombatUI();
                }
            }

            _auraCheck = false;
        }

        public override void OnWaveStart()
        {
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 928));
            InitVar();
            if (owner.faction != Faction.Player) return;
            owner.UnitData.unitData.InitBattleDialogByDefaultBook(new LorId(ModPack21341Init.PackageId, 14));
            owner.view.DisplayDlg(DialogType.START_BATTLE, "0");
        }

        public void SetPhaseChanged()
        {
            _phaseChanged = true;
            _count = 4;
        }

        private void SummonMio()
        {
            _summonMio = false;
            _summonMioUsed = true;
            _mapChange = true;
            var index = owner.index == 1 ? 0 : 1;
            foreach (var unit in BattleObjectManager.instance.GetList(Faction.Player).Where(x => x.index == index))
                BattleObjectManager.instance.UnregisterUnit(unit);
            UnitUtilities.AddNewUnitPlayerSide(_floor, new UnitModel
            {
                Name = "ModPack21341InitEgoMio",
                OverrideName = "Mio's Memory",
                Pos = index,
                EmotionLevel = 5,
                Sephirah = _floor.Sephirah
            });
            UnitUtilities.RefreshCombatUI();
        }

        public void SetEgoReady()
        {
            _auraCheck = true;
            _egoActivated = true;
        }

        public override void OnRoundEnd()
        {
            if (_egoActivated && owner.bufListDetail.GetActivatedBuf(KeywordBuf.Burn)?.stack > 3)
                owner.cardSlotDetail.RecoverPlayPoint(1);
            if (owner.faction == Faction.Enemy && _phaseChanged)
                _count++;
            if (_summonMio)
                SummonMio();
        }

        private void InitVar()
        {
            _summonMio = false;
            _phaseChanged = false;
            _egoActivated = false;
            _deathCheck = false;
            _summonMioUsed = false;
            _dlg = owner.UnitData.unitData.battleDialogModel;
            var currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
            _floor = Singleton<StageController>.Instance.GetStageModel().GetFloor(currentStageFloorModel.Sephirah);
            _originalBook = owner.UnitData.unitData.GetCustomBookItemData();
            if (owner.faction == Faction.Player)
                UnitUtilities.FillUnitDataSingle(new UnitModel
                {
                    Id = 10000008,
                    Name = "ModPack21341InitEgoMio",
                    DialogId = 2
                }, _floor);
            if (!string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) ||
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem) return;
            owner.view.ChangeSkin(owner.UnitData.unitData.CustomBookItem.GetCharacterName());
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 928)) PrepareEgoChange();
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 929))
            {
                _summonMio = true;
                owner.personalEgoDetail.RemoveCard(curCard.card.GetID());
            }

            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 45))
                owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
        }

        public override BattleDiceCardModel OnSelectCardAuto(BattleDiceCardModel origin, int currentDiceSlotIdx)
        {
            if (owner.faction != Faction.Enemy || owner.IsBreakLifeZero() || !_phaseChanged || _count < 3)
                return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
            origin = BattleDiceCardModel.CreatePlayingCard(
                ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.PackageId, 45)));
            _count = 0;
            return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
        }

        public override void OnBattleEnd()
        {
            if (owner.faction == Faction.Player) UnitUtilities.RemoveUnitData(_floor, "ModPack21341InitEgoMio");
            owner.UnitData.unitData.battleDialogModel = _dlg;
            if (owner.faction == Faction.Player)
                UnitUtilities.ReturnToTheOriginalPlayerUnit(owner, _originalBook, _dlg);
        }
    }
}
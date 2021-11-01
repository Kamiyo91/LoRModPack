using System.Collections.Generic;
using System.Linq;
using CustomMapUtility;
using LOR_XML;
using ModPack21341.Characters;
using ModPack21341.Harmony;
using ModPack21341.Models;
using ModPack21341.StageManager.MapManager.KamiyoStageMaps;
using ModPack21341.Utilities;
using UnityEngine;

namespace ModPack21341.StageManager
{
    public class EnemyTeamStageManager_Kamiyo : EnemyTeamStageManager
    {
        private int _phase;
        private bool _restart;
        private Kamiyo2MapManager _mapManager;
        private BattleUnitModel _kamiyoModel;
        private BattleUnitModel _mioGhostModel;
        public override void OnWaveStart()
        {
            UnitUtilities.TestingUnitValues();
            _kamiyoModel = BattleObjectManager.instance.GetAliveList(Faction.Enemy).FirstOrDefault();
            _kamiyoModel?.UnitData.unitData.InitBattleDialogByDefaultBook(new LorId(ModPack21341Init.PackageId, 22));
            CustomMapHandler.InitCustomMap("Kamiyo1", new Kamiyo1MapManager(), false, true, 0.5f, 0.2f, 0.5f, 0.45f);
            CustomMapHandler.InitCustomMap("Kamiyo2", new Kamiyo2MapManager(), false, true, 0.5f, 0.475f, 0.5f, 0.225f);
            CustomMapHandler.EnforceMap();
            Singleton<StageController>.Instance.CheckMapChange();
            _kamiyoModel?.view.DisplayDlg(DialogType.START_BATTLE, "0");
            if (Singleton<StageController>.Instance.GetStageModel()
                .GetStageStorageData<int>("Phase", out var curPhase))
                _phase = curPhase;
            if (_phase < 1) return;
            _restart = true;
            _kamiyoModel?.UnitData.unitData.InitBattleDialogByDefaultBook(new LorId(ModPack21341Init.PackageId, 23));
            AddMioUnit();
            if (_kamiyoModel.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Power_of_the_Unknown) is
                PassiveAbility_Power_of_the_Unknown passive)
            {
                passive.Restart();
            }
            CustomMapHandler.EnforceMap(_phase);
            Singleton<StageController>.Instance.CheckMapChange();
            UnitUtilities.RefreshCombatUI();
            _kamiyoModel.view.DisplayDlg(DialogType.START_BATTLE, "0");
            _mapManager = SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject as Kamiyo2MapManager;
            _mapManager?.InitDlg(0, 6);

        }
        public override void OnRoundStart()
        {
            CustomMapHandler.EnforceMap(_phase);
        }

        public override void OnRoundStart_After()
        {
            if (!_restart) return;
            _restart = false;
            SetMassAttacks();
        }

        private void SetMassAttacks()
        {
            if (_kamiyoModel.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Power_of_the_Unknown) is
                PassiveAbility_Power_of_the_Unknown kamiyoPassive)
            {
                kamiyoPassive.SetPhaseChanged();
            }

            if (!(_mioGhostModel.passiveDetail.PassiveList.Find(x => x is PassiveAbility_MioEnemyDesc) is
                PassiveAbility_MioEnemyDesc mioPassive)) return;
            mioPassive.SetAwakened(true);
            mioPassive.Hide();
        }
        public override void OnEndBattle()
        {
            var stageModel = Singleton<StageController>.Instance.GetStageModel();
            var currentWaveModel = Singleton<StageController>.Instance.GetCurrentWaveModel();
            if (currentWaveModel == null)
            {
                Debug.LogError("floor not found");
                return;
            }
            if (currentWaveModel.IsUnavailable())
            {
                return;
            }
            stageModel.SetStageStorgeData("Phase", _phase);
            var list = new List<UnitBattleDataModel>();
            var kamiyo = BattleObjectManager.instance.GetList(Faction.Enemy).FirstOrDefault();
            if (kamiyo.IsDead())
            {
                kamiyo.Revive(10);
            }
            list.Add(kamiyo.UnitData);
            currentWaveModel.ResetUnitBattleDataList(list);
        }
        public override void OnRoundEndTheLast()
        {
            CheckPhase();
            if (_phase > 0)
                CheckSubUnit();
        }

        private void CheckSubUnit()
        {
            if (_kamiyoModel.IsDead()) return;
            if (!_mioGhostModel.IsDead()) return;
            _mioGhostModel.Revive(_mioGhostModel.MaxHp);
            _mioGhostModel.bufListDetail.RemoveBufAll(BufPositiveType.Negative);
            _mioGhostModel.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_sealTemp));
            _mioGhostModel.breakDetail.ResetGauge();
            _mioGhostModel.breakDetail.RecoverBreakLife(1, true);
            _mioGhostModel.breakDetail.nextTurnBreak = false;
            _mioGhostModel.cardSlotDetail.RecoverPlayPoint(_mioGhostModel.cardSlotDetail.GetMaxPlayPoint());
            _mioGhostModel.moveDetail.ReturnToFormationByBlink(true);
            _mioGhostModel.view.EnableView(true);
            _mioGhostModel.view.CreateSkin();
            UnitUtilities.RefreshCombatUI();

        }

        private void AddMioUnit()
        {
            _mioGhostModel = UnitUtilities.AddNewUnitEnemySide(new UnitModel
            {
                Id = 11,
                Pos = 1,
                EmotionLevel = 4,
                OnWaveStart = true
            });
            if (_mioGhostModel.passiveDetail.PassiveList.Find(x => x is PassiveAbility_MioEnemyDesc) is
                PassiveAbility_MioEnemyDesc mioPassive)
            {
                mioPassive.SetAwakened(true);
                mioPassive.Hide();
            }
            _mioGhostModel.UnitData.unitData.InitBattleDialogByDefaultBook(new LorId(ModPack21341Init.PackageId, 2));
        }

        private void AddKamiyoUnit()
        {
            _kamiyoModel = UnitUtilities.AddNewUnitEnemySide(new UnitModel
            {
                Id = 10,
                Pos = 0,
                EmotionLevel = 4
            });
            SetPhase();
            _kamiyoModel.UnitData.unitData.InitBattleDialogByDefaultBook(new LorId(ModPack21341Init.PackageId, 23));
        }

        private void SetPhase()
        {
            if (!(_kamiyoModel.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Power_of_the_Unknown) is
                PassiveAbility_Power_of_the_Unknown kamiyoPassive)) return;
            kamiyoPassive.SetPhaseChanged();
            kamiyoPassive.SetEgoReady();
        }
        private void CheckPhase()
        {
            if (BattleObjectManager.instance.GetAliveList(Faction.Enemy).Count >= 1 || _phase != 0) return;
            _phase++;
            foreach (var unit in BattleObjectManager.instance.GetList(Faction.Enemy))
            {
                BattleObjectManager.instance.UnregisterUnit(unit);
            }
            AddKamiyoUnit();
            AddMioUnit();
            CustomMapHandler.EnforceMap(_phase);
            Singleton<StageController>.Instance.CheckMapChange();
            UnitUtilities.RefreshCombatUI();
            _kamiyoModel.view.DisplayDlg(DialogType.START_BATTLE, "0");
            _mapManager = SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject as Kamiyo2MapManager;
            _mapManager?.InitDlg(0, 6);
        }
    }
}

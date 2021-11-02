using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LOR_XML;
using ModPack21341.Characters.Hayate.Buffs;
using ModPack21341.Characters.Hayate.PassiveAbilities;
using ModPack21341.Characters.Kamiyo.Buffs;
using ModPack21341.Characters.Kamiyo.PassiveAbilities;
using ModPack21341.Harmony;
using ModPack21341.Models;
using ModPack21341.StageManager.MapManager.HayateStageMaps;
using ModPack21341.Utilities;
using ModPack21341.Utilities.CustomMapUtility.Assemblies;

namespace ModPack21341.StageManager
{
    //Hayate
    public class EnemyTeamStageManager_ModPack21341Init4 : EnemyTeamStageManager
    {
        private Task _changeBgm;
        private bool _firstStep;
        private StageLibraryFloorModel _floor;
        private BattleUnitModel _hayateModel;
        private PassiveAbility_ModPack21341Init22 _hayatePassive;
        private bool _lastPhaseStarted;
        private bool _musicChanged;
        private BattleUnitModel _sephiraModel;

        public override void OnWaveStart()
        {
            var currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
            _floor = Singleton<StageController>.Instance.GetStageModel().GetFloor(currentStageFloorModel.Sephirah);
            CustomMapHandler.InitCustomMap("Hayate", new ModPack21341InitHayateMapManager(), false, true, 0.5f, 0.3f,
                0.5f, 0.475f);
            CustomMapHandler.EnforceMap();
            Singleton<StageController>.Instance.CheckMapChange();
            UnitUtilities.FillBaseUnit(_floor);
            _hayateModel = BattleObjectManager.instance.GetAliveList(Faction.Enemy).FirstOrDefault();
            _sephiraModel = BattleObjectManager.instance.GetAliveList(Faction.Player).FirstOrDefault();
            _hayatePassive =
                _hayateModel?.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init22) as
                    PassiveAbility_ModPack21341Init22;
            _firstStep = true;
            _musicChanged = false;
            _lastPhaseStarted = false;
        }

        public override void OnRoundStart()
        {
            CustomMapHandler.EnforceMap();
            MapUtilities.CheckAndChangeBgm(ref _changeBgm);
        }

        public override void OnRoundStart_After()
        {
            if (_hayateModel.bufListDetail.GetActivatedBufList().Exists(x => x is BattleUnitBuf_ModPack21341Init10))
                MapUtilities.ActiveCreatureBattleCamFilterComponent();
        }

        public override void OnRoundEndTheLast()
        {
            HayateIsDeadBeforePhase3();
            ChangeToPhase2Music();
            CheckUnitSummon();
            CheckLastPhase();
        }

        private void HayateIsDeadBeforePhase3()
        {
            if (_lastPhaseStarted) return;
            if (!_hayateModel.IsDead()) return;
            _hayateModel.Revive(10);
            _hayateModel.breakDetail.ResetGauge();
            _hayateModel.breakDetail.RecoverBreakLife(1, true);
            _hayateModel.breakDetail.nextTurnBreak = false;
        }

        private void ChangeToPhase2Music()
        {
            if (_musicChanged || !_hayatePassive.GetPhase2Status()) return;
            _musicChanged = true;
            MapUtilities.PrepareChangeBgm("HayatePhase2.mp3", ref _changeBgm);
        }

        private void CheckLastPhase()
        {
            if (_lastPhaseStarted || BattleObjectManager.instance.GetAliveList(Faction.Player).Count > 0) return;
            {
                _lastPhaseStarted = true;
                MapUtilities.PrepareChangeBgm("HayatePhase3.mp3", ref _changeBgm);
                foreach (var unit in BattleObjectManager.instance.GetList(Faction.Player))
                    BattleObjectManager.instance.UnregisterUnit(unit);
                var kamiyoModel = UnitUtilities.AddNewUnitPlayerSide(_floor, new UnitModel
                {
                    Id = 10000011,
                    Name = "Kamiyo",
                    Pos = 0,
                    EmotionLevel = 5,
                    Sephirah = _floor.Sephirah
                });
                _hayateModel.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_ModPack21341Init9));
                _hayateModel.RecoverHP(231);
                _hayateModel.breakDetail.ResetGauge();
                _hayateModel.breakDetail.RecoverBreakLife(1, true);
                _hayateModel.breakDetail.nextTurnBreak = false;
                if (_hayateModel.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init24) is
                    PassiveAbility_ModPack21341Init24 shimmeringPassive)
                {
                    _hayateModel.passiveDetail.DestroyPassive(shimmeringPassive);
                    _hayateModel.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 52));
                }

                _hayatePassive.SetFinalPhase(true);
                kamiyoModel.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init11());
                if (kamiyoModel.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init34) is
                    PassiveAbility_ModPack21341Init34 kamiyoPassive)
                    kamiyoPassive.SetEgoReadyFinalPhaseHayate();
                UnitUtilities.BattleAbDialog(_hayateModel.view.dialogUI,
                    new List<AbnormalityCardDialog>
                        {new AbnormalityCardDialog {id = "Hayate", dialog = "Kamiyo...The time has come!"}});
            }
        }

        private void CheckUnitSummon()
        {
            if (!_firstStep ||
                _sephiraModel.hp > _sephiraModel.MaxHp * 0.75f && !_hayatePassive.GetPhase2Status()) return;
            _firstStep = false;
            for (var i = 1; i < 5; i++)
                UnitUtilities.AddOriginalPlayerUnitPlayerSide(i, _sephiraModel.emotionDetail.EmotionLevel);
            UnitUtilities.RefreshCombatUI();
        }
    }
}
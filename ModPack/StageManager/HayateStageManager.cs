using System.Linq;
using System.Threading.Tasks;
using CustomMapUtility;
using ModPack21341.Characters;
using ModPack21341.Characters.Buffs;
using ModPack21341.Models;
using ModPack21341.StageManager.MapManager.HayateStageMaps;
using ModPack21341.Utilities;

namespace ModPack21341.StageManager
{
    public class EnemyTeamStageManager_Hayate : EnemyTeamStageManager
    {
        private bool _firstStep;
        private BattleUnitModel _hayateModel;
        private PassiveAbility_Hayate _hayatePassive;
        private BattleUnitModel _sephiraModel;
        private StageLibraryFloorModel _floor;
        private bool _lastPhaseStarted;
        private Task _changeBgm;
        public override void OnWaveStart()
        {
            var currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
            _floor = Singleton<StageController>.Instance.GetStageModel().GetFloor(currentStageFloorModel.Sephirah);
            CustomMapHandler.InitCustomMap("Hayate", new HayateMapManager(), false, true, 0.5f, 0.3f, 0.5f, 0.475f);
            CustomMapHandler.EnforceMap();
            Singleton<StageController>.Instance.CheckMapChange();
            UnitUtilities.FillBaseUnit(_floor);
            _hayateModel = BattleObjectManager.instance.GetAliveList(Faction.Enemy).FirstOrDefault();
            _sephiraModel = BattleObjectManager.instance.GetAliveList(Faction.Player).FirstOrDefault();
            _hayatePassive = _hayateModel?.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Hayate) as PassiveAbility_Hayate;
            _firstStep = true;
        }
        public override void OnRoundStart()
        {
            UnitUtilities.TestingUnitValues();
            CustomMapHandler.EnforceMap();
            MapUtilities.CheckAndChangeBGM(ref _changeBgm);
        }
        public override void OnRoundStart_After()
        {
            if (_hayateModel.bufListDetail.GetActivatedBufList().Exists(x => x is BattleUnitBuf_TrueGodAura))
            {
                MapUtilities.ActiveCreatureBattleCamFilterComponent();
            }
        }

        public override void OnRoundEndTheLast()
        {
            CheckUnitSummon();
            CheckLastPhase();
        }

        public override void OnEndBattle()
        {
            UnitUtilities.RemoveUnitData(_floor, "Kamiyo");
        }

        private void CheckLastPhase()
        {
            if (_lastPhaseStarted || BattleObjectManager.instance.GetAliveList(Faction.Player).Count > 0) return;
            {
                _lastPhaseStarted = true;
                MapUtilities.PrepareChangeBGM("HayatePhase3.mp3", ref _changeBgm);
                var kamiyoModel = UnitUtilities.AddNewUnitPlayerSide(_floor, new UnitModel
                {
                    Name = "Kamiyo",
                    Pos = 0,
                    EmotionLevel = 5,
                    Sephirah = _floor.Sephirah
                });
                _hayateModel.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_HayateImmortal));
                _hayateModel.RecoverHP(231);
                _hayateModel.breakDetail.ResetGauge();
                _hayateModel.breakDetail.RecoverBreakLife(1, true);
                _hayatePassive.SetFinalPhase(true);
                kamiyoModel.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_KamiyoAndHayate());
                if (kamiyoModel.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Power_of_the_Unknown) is
                    PassiveAbility_Power_of_the_Unknown kamiyoPassive)
                    kamiyoPassive.SetEgoReadyFinalPhaseHayate();
            }
        }
        private void CheckUnitSummon()
        {
            if (!_firstStep || _sephiraModel.hp > _sephiraModel.MaxHp * 0.75f && !_hayatePassive.GetPhase2Status()) return;
            _firstStep = false;
            MapUtilities.PrepareChangeBGM("HayatePhase2.mp3", ref _changeBgm);
            for (var i = 1; i < 5; i++)
                UnitUtilities.AddOriginalPlayerUnitPlayerSide(i);
            UnitUtilities.RefreshCombatUI();
            UnitUtilities.FillUnitDataSingle(new UnitModel
            {
                Id = 10000011,
                Name = "Kamiyo",
                DialogId = 2
            }, _floor);
        }
    }
}

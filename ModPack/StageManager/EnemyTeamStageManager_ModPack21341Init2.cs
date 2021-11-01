using System.Linq;
using System.Threading.Tasks;
using LOR_XML;
using ModPack21341.Characters.Mio.Buffs;
using ModPack21341.Characters.Mio.PassiveAbilities;
using ModPack21341.Harmony;
using ModPack21341.Models;
using ModPack21341.StageManager.MapManager.MioStageMaps;
using ModPack21341.Utilities;
using ModPack21341.Utilities.CustomMapUtility.Assemblies;

namespace ModPack21341.StageManager
{
    //Mio
    public class EnemyTeamStageManager_ModPack21341Init2 : EnemyTeamStageManager
    {
        private Task _changeBgm;
        private StageLibraryFloorModel _floor;
        private ModPack21341InitMioMapManager _mioMapManager;
        private bool _mioStarterDlg;
        private bool _phase2Activated;
        private bool _phaseChanged;
        private BattleUnitModel _tempMioAllyUnit;

        public override void OnWaveStart()
        {
            UnitUtilities.TestingUnitValues();
            var currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
            _floor = Singleton<StageController>.Instance.GetStageModel().GetFloor(currentStageFloorModel.Sephirah);
            UnitUtilities.FillUnitDataSingle(new UnitModel
            {
                Id = 10000004,
                Name = "ModPack21341InitStoryMio",
                DialogId = 201
            }, _floor);
            CustomMapHandler.InitCustomMap("Mio", new ModPack21341InitMioMapManager(), false, true, 0.5f, 0.2f);
            CustomMapHandler.EnforceMap();
            Singleton<StageController>.Instance.CheckMapChange();
            _mioMapManager =
                SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject as ModPack21341InitMioMapManager;
        }

        public override void OnEndBattle()
        {
            UnitUtilities.RemoveUnitData(_floor, "ModPack21341InitStoryMio");
        }

        public override void OnRoundStart()
        {
            CustomMapHandler.EnforceMap();
            MapUtilities.CheckAndChangeBgm(ref _changeBgm);
            if (!_mioStarterDlg) return;
            _tempMioAllyUnit.view.DisplayDlg(DialogType.START_BATTLE, "0");
            _mioStarterDlg = false;
        }

        public override void OnRoundStart_After()
        {
            if (_phase2Activated) MapUtilities.ActiveCreatureBattleCamFilterComponent();
        }

        public override void OnRoundEndTheLast()
        {
            CheckPhase();
        }

        private void CheckPhase()
        {
            if (!_phaseChanged) return;
            _phaseChanged = false;
            _mioStarterDlg = true;
            _phase2Activated = true;
            MapUtilities.PrepareChangeBgm("MioPhase2.mp3", ref _changeBgm);
            PrepareAllyUnit();
            MapUtilities.ActiveCreatureBattleCamFilterComponent();
            SetPassiveValues();
            _mioMapManager.InitDlg(0, 3);
        }

        public bool GetPhaseStatus()
        {
            return _phase2Activated;
        }

        public void SetPhaseChange()
        {
            _phaseChanged = true;
        }

        private void PrepareAllyUnit()
        {
            var playerUnitList = BattleObjectManager.instance.GetList(Faction.Player);
            _tempMioAllyUnit = UnitUtilities.AddNewUnitPlayerSide(_floor, new UnitModel
            {
                Id = 10000004,
                Name = "ModPack21341InitStoryMio",
                OverrideName = "Mio?",
                Pos = playerUnitList.Count,
                Sephirah = _floor.Sephirah
            });
            _tempMioAllyUnit.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init15());
            _tempMioAllyUnit.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init14());
            _tempMioAllyUnit.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 16));
            UnitUtilities.ChangeCustomSkin(_tempMioAllyUnit, 10000200);
            _tempMioAllyUnit.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 903));
            _tempMioAllyUnit.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 904));
            _tempMioAllyUnit.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 905));
            UnitUtilities.ChangeDeck(_tempMioAllyUnit, UnitUtilities.GetMioCardsId());
            _tempMioAllyUnit.UnitData.unitData.InitBattleDialogByDefaultBook(new LorId(ModPack21341Init.PackageId,
                201));
        }

        private void SetPassiveValues()
        {
            var enemyPassive = BattleObjectManager.instance.GetAliveList(Faction.Enemy).FirstOrDefault()
                ?.passiveDetail.PassiveList
                .Find(x => x is PassiveAbility_ModPack21341Init38) as PassiveAbility_ModPack21341Init38;
            enemyPassive?.SetAwakened(true);
            var tempMioPassive =
                _tempMioAllyUnit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init37) as
                    PassiveAbility_ModPack21341Init37;
            tempMioPassive?.SetSpecialCase();
        }
    }
}
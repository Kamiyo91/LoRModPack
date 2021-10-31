using ModPack21341.Models;
using System.Linq;
using System.Threading.Tasks;
using CustomMapUtility;
using LOR_XML;
using ModPack21341.Characters;
using ModPack21341.Characters.Buffs;
using ModPack21341.Harmony;
using ModPack21341.StageManager.MapManager.MioStageMaps;
using ModPack21341.Utilities;

namespace ModPack21341.StageManager
{
    public class EnemyTeamStageManager_Mio : EnemyTeamStageManager
    {
        private MioMapManager _mioMapManager;
        private BattleUnitModel _tempMioAllyUnit;
        private bool _phaseChanged;
        private bool _mioStarterDlg;
        private bool _phase2Activated;
        private StageLibraryFloorModel _floor;
        private Task _changeBgm;
        public override void OnWaveStart()
        {
            var currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
            _floor = Singleton<StageController>.Instance.GetStageModel().GetFloor(currentStageFloorModel.Sephirah);
            UnitUtilities.FillUnitDataSingle(new UnitModel
                    {
                        Id = 10000004,
                        Name = "Mio?",
                        DialogId = 201
                   }, _floor);
            CustomMapHandler.InitCustomMap("Mio", new MioMapManager(),false,true,0.5f,0.2f);
            CustomMapHandler.EnforceMap();
            Singleton<StageController>.Instance.CheckMapChange();
            _mioMapManager = SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject as MioMapManager;
        }

        public override void OnEndBattle() => UnitUtilities.RemoveUnitData(_floor, "Mio?");

        public override void OnRoundStart()
        {
            CustomMapHandler.EnforceMap();
            MapUtilities.CheckAndChangeBGM(ref _changeBgm);
            if (!_mioStarterDlg) return;
            _tempMioAllyUnit.view.DisplayDlg(DialogType.START_BATTLE, "0");
            _mioStarterDlg = false;
        }

        public override void OnRoundStart_After()
        {
            if (_phase2Activated) MapUtilities.ActiveCreatureBattleCamFilterComponent();
        }
        public override void OnRoundEndTheLast() => CheckPhase();
        private void CheckPhase()
        {
            if (!_phaseChanged) return;
            _phaseChanged = false;
            _mioStarterDlg = true;
            _phase2Activated = true;
            MapUtilities.PrepareChangeBGM("MioPhase2.mp3", ref _changeBgm);
            PrepareAllyUnit();
            MapUtilities.ActiveCreatureBattleCamFilterComponent();
            SetPassiveValues();
            _mioMapManager.InitDlg(0, 3);
        }
        public bool GetPhaseStatus() => _phase2Activated;
        public void SetPhaseChange() => _phaseChanged = true;
        private void PrepareAllyUnit()
        {
            var playerUnitList = BattleObjectManager.instance.GetList(Faction.Player);
            _tempMioAllyUnit = UnitUtilities.AddNewUnitPlayerSide(_floor, new UnitModel
            {
                        Id = 10000004,
                        Name = "Mio?",
                        Pos = playerUnitList.Count,
                        Sephirah = _floor.Sephirah
            });
            _tempMioAllyUnit.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_GodAuraRelease());
            _tempMioAllyUnit.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_CorruptionResist());
            _tempMioAllyUnit.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 16));
            UnitUtilities.ChangeCustomSkin(_tempMioAllyUnit, 10000200);
            _tempMioAllyUnit.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 903));
            _tempMioAllyUnit.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 904));
            _tempMioAllyUnit.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 905));
            UnitUtilities.ChangeDeck(_tempMioAllyUnit, UnitUtilities.GetMioCardsId());
            _tempMioAllyUnit.UnitData.unitData.InitBattleDialogByDefaultBook(new LorId(ModPack21341Init.PackageId, 201));
        }

        private void SetPassiveValues()
        {
            var enemyPassive = BattleObjectManager.instance.GetAliveList(Faction.Enemy).FirstOrDefault()
                ?.passiveDetail.PassiveList.Find(x => x is PassiveAbility_MioEnemyDesc) as PassiveAbility_MioEnemyDesc;
            enemyPassive?.SetAwakened(true);
            var tempMioPassive = _tempMioAllyUnit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_God_Fragment) as PassiveAbility_God_Fragment;
            tempMioPassive?.SetSpecialCase();
        }
    }
}

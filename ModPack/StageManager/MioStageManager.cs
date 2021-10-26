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
        private BookModel _originalBook;
        private BattleDialogueModel _originalDialog;
        private bool _phaseChanged;
        private bool _mioStarterDlg;
        private bool _phase2Activated;
        private Task _changeBgm;
        public override void OnWaveStart()
        {
            CustomMapHandler.InitCustomMap("Mio", new MioMapManager());
            CustomMapHandler.EnforceMap();
            Singleton<StageController>.Instance.CheckMapChange();
            _mioMapManager = SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject as MioMapManager;
        }

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

        public override void OnEndBattle()
        {
            UnitUtilities.ReturnToTheOriginalPlayerUnit(_tempMioAllyUnit, _originalBook, _originalDialog);
        }
        public override void OnRoundEndTheLast() => CheckPhase();
        private void CheckPhase()
        {
            if (!_phaseChanged) return;
            _phaseChanged = false;
            _mioStarterDlg = true;
            _phase2Activated = true;
            MapUtilities.PrepareChangeBGM("MioPhase2.mp3",ref _changeBgm);
            PrepareAllyUnit();
            MapUtilities.ActiveCreatureBattleCamFilterComponent();
            SetPassiveValues();
            _mioMapManager.InitDlg(0, 3);
        }

        public bool GetPhaseStatus() => _phase2Activated;
        public void SetPhaseChange() => _phaseChanged = true;
        private void PrepareAllyUnit()
        {
            var currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
            var floor = Singleton<StageController>.Instance.GetStageModel().GetFloor(currentStageFloorModel.Sephirah);
            var playerUnitList = BattleObjectManager.instance.GetList(Faction.Player);
            var currentStageFloorUnitList = currentStageFloorModel.GetUnitBattleDataList();
            foreach (var (unit, i) in currentStageFloorUnitList.Select((value, i) => (value, i)))
            {
                if (playerUnitList.Exists(x => x.UnitData == unit)) continue;
                _tempMioAllyUnit = UnitUtilities.AddNewUnitPlayerSide(floor, new UnitModel
                    {
                        Index = i,
                        Id = 10000004,
                        Name = "Mio?",
                        Pos = playerUnitList.Count,
                        EmotionLevel = 4,
                        CurrentLight = 8,
                        CustomDialog = true,
                        DialogId = 201
                    },
                    ref _originalBook, ref _originalDialog);
                break;
            }
            _tempMioAllyUnit.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_GodAuraRelease());
            _tempMioAllyUnit.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_CorruptionResist());
            _tempMioAllyUnit.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 16));
            UnitUtilities.ChangeCustomSkin(_tempMioAllyUnit, 10000200);
            _tempMioAllyUnit.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 903));
            _tempMioAllyUnit.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 904));
            _tempMioAllyUnit.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 905));
            UnitUtilities.ChangeDeck(_tempMioAllyUnit, UnitUtilities.GetMioCardsId());
        }

        private void SetPassiveValues()
        {
            var enemyPassive = BattleObjectManager.instance.GetAliveList(Faction.Enemy).FirstOrDefault()
                ?.passiveDetail.PassiveList.Find(x => x is PassiveAbility_MioEnemyDesc) as PassiveAbility_MioEnemyDesc;
            enemyPassive?.SetAwakened(true);
            var allyPassive = _tempMioAllyUnit?.passiveDetail.PassiveList.Find(x => x is PassiveAbility_God_Fragment) as PassiveAbility_God_Fragment;
            allyPassive?.SetOriginalBookForStoryBattle(_originalBook);
        }
    }
}

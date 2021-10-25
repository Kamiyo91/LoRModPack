using ModPack21341.Models;
using System.Linq;
using LOR_XML;
using ModPack21341.Characters;
using ModPack21341.Characters.Buffs;
using ModPack21341.Harmony;
using ModPack21341.StageManager.MapManager.DlgManager;
using ModPack21341.StageManager.MapManager.MioStageMaps;
using ModPack21341.Utilities;
using UnityEngine;

namespace ModPack21341.StageManager
{
    public class EnemyTeamStageManager_Mio : EnemyTeamStageManager
    {
        private MioDlgManager _dlgManager;
        private BattleUnitModel _tempMioAllyUnit;
        private BookModel _originalBook;
        private BattleDialogueModel _originalDialog;
        private bool _phaseChanged;
        private bool _mioStarterDlg;
        private bool _phase2Activated;
        public override void OnWaveStart()
        {
            MapUtilities.EnemyTeamEmotionCoinValueChange();
            InitCustomMap();
            if (!(SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject is MioMapManager))
            {
                Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(0);
            }
            Singleton<StageController>.Instance.CheckMapChange();
        }

        public override void OnRoundStart()
        {
            MapUtilities.EnemyTeamEmotionCoinValueChange();
            if (!(SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject is MioMapManager))
            {
                Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(0);
            }

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
            _dlgManager.gameObject.SetActive(false);
            UnitUtilities.ReturnToTheOriginalPlayerUnit(_tempMioAllyUnit, _originalBook, _originalDialog);
        }
        public override void OnRoundEndTheLast() => CheckPhase();
        private void CheckPhase()
        {
            if (!_phaseChanged) return;
            _phaseChanged = false;
            _mioStarterDlg = true;
            _phase2Activated = true;
            PrepareAllyUnit();
            MapUtilities.ActiveCreatureBattleCamFilterComponent();
            AudioUtilities.ChangeEnemyTeamTheme("MioPhase2");
            SetPassiveValues();
            InitDlgManager();
        }
        public void SetPhaseChange() => _phaseChanged = true;
        private static void InitCustomMap()
        {
            var mapManager = MapUtilities.PrepareMapComponent(new MapModel
            {
                Name = "CryingChild",
                Stage = "Mio",
                ArtworkBG = "CityBG",
                ArtworkFloor = "CityFloor",
                BgFx = 0.5f,
                BgFy = 0.2f,
                FloorFx = 0.5f,
                FloorFy = 0.375f,
                BgmName = "MioPhase1",
                ExtraSettings = new MapExtraSettings { MapManagerType = typeof(MioMapManager) }
            });
            SingletonBehavior<BattleSceneRoot>.Instance.InitInvitationMap(mapManager);
        }

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
            _tempMioAllyUnit.passiveDetail.AddPassive(new LorId(ModPack21341Init.packageId, 16));
            //_tempMioAllyUnit.passiveDetail.AddPassive(new LorId(ModPack21341Init.packageId, 17));
            UnitUtilities.ChangeCustomSkin(_tempMioAllyUnit, 10000200);
            _tempMioAllyUnit.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.packageId, 903));
            _tempMioAllyUnit.personalEgoDetail.AddCard(new LorId(ModPack21341Init.packageId, 904));
            _tempMioAllyUnit.personalEgoDetail.AddCard(new LorId(ModPack21341Init.packageId, 905));
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
        private void InitDlgManager()
        {
            _dlgManager = new GameObject { transform = { parent = SingletonBehavior<BattleSceneRoot>.Instance.transform } }
                .AddComponent<MioDlgManager>();
            _dlgManager.Init(0, 3);
        }
    }
}

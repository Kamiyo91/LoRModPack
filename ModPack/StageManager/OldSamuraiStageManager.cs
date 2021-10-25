using System.Linq;
using LOR_XML;
using ModPack21341.Characters;
using ModPack21341.Harmony;
using ModPack21341.Models;
using ModPack21341.StageManager.MapManager.DlgManager;
using ModPack21341.StageManager.MapManager.OldSamuraiStageMaps;
using ModPack21341.Utilities;
using UnityEngine;

namespace ModPack21341.StageManager
{
    public class EnemyTeamStageManager_OldSamurai : EnemyTeamStageManager
    {
        private OldSamuraiDlgManager _dlgManager;
        private BattleUnitModel _mainEnemyModel;
        private BattleUnitModel _hodUnitModel;
        private int _phase;
        private bool _finished;
        public override void OnWaveStart()
        {
            MapUtilities.EnemyTeamEmotionCoinValueChange();
            InitCustomMap();
            if (!(SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject is OldSamuraiMapManager))
            {
                Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(0);
            }
            Singleton<StageController>.Instance.CheckMapChange();
            UnitStarterSet();
            InitDlgManager();
            _phase = 0;
        }

        private void UnitStarterSet()
        {
            _hodUnitModel = BattleObjectManager.instance.GetList(Faction.Player).FirstOrDefault();
            var passive = _hodUnitModel?.passiveDetail.AddPassive(new LorId(ModPack21341Init.packageId, 7));
            passive?.Hide();
            passive?.OnWaveStart();
            _mainEnemyModel = BattleObjectManager.instance.GetList(Faction.Enemy).FirstOrDefault();
        }

        private void InitDlgManager()
        {
            _dlgManager = new GameObject { transform = { parent = SingletonBehavior<BattleSceneRoot>.Instance.transform } }
                .AddComponent<OldSamuraiDlgManager>();
            _dlgManager.Init(0, 4);
        }
        public override void OnRoundEndTheLast() => CheckPhase();

        public override void OnEndBattle() => _dlgManager.gameObject.SetActive(false);

        public void SetFinishable() => _finished = true;
        private void CheckSubUnit()
        {
            if (_mainEnemyModel.IsDead()) return;
            var deadEnemy = BattleObjectManager.instance.GetList(Faction.Enemy)
                .Where(x => x != _mainEnemyModel && x.IsDead()).ToList();
            foreach (var unit in deadEnemy)
            {
                unit.Revive(25);
                unit.breakDetail.ResetGauge();
                unit.breakDetail.RecoverBreakLife(1, true);
                unit.breakDetail.nextTurnBreak = false;
                unit.cardSlotDetail.RecoverPlayPoint(unit.cardSlotDetail.GetMaxPlayPoint());
                unit.view.ChangeScale(MapSize.L);
                unit.moveDetail.ReturnToFormationByBlink(true);
                unit.view.EnableView(true);
                unit.view.CreateSkin();
            }
        }

        public override void OnRoundStart()
        {
            MapUtilities.EnemyTeamEmotionCoinValueChange();
            if (!(SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject is OldSamuraiMapManager))
            {
                Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(0);
            }
            CheckSubUnit();
        }

        private void MainEnemySetNewPhase()
        {
            var enemyPassive = _mainEnemyModel?.passiveDetail.PassiveList.Find(x => x is PassiveAbility_OldSamurai) as PassiveAbility_OldSamurai;
            enemyPassive?.PhaseChanged();
            UnitUtilities.PhaseChangeAllPlayerUnitRecoverBonus(20, 20, 0, true);
            if (_mainEnemyModel == null) return;
            _mainEnemyModel.Revive(_mainEnemyModel.MaxHp);
            _mainEnemyModel.moveDetail.ReturnToFormationByBlink(true);
            _mainEnemyModel.breakDetail.ResetGauge();
            _mainEnemyModel.breakDetail.RecoverBreakLife(1, true);
            _mainEnemyModel.breakDetail.nextTurnBreak = false;
            _mainEnemyModel.cardSlotDetail.RecoverPlayPoint(_mainEnemyModel.cardSlotDetail.GetMaxPlayPoint());
            UnitUtilities.ChangeDeck(_mainEnemyModel, UnitUtilities.GetSamuraiCardsId());
            _mainEnemyModel.allyCardDetail.DrawCards(4);
        }

        private static void SubUnitSummon()
        {
            for (var i = 1; i < 4; i++)
            {
                UnitUtilities.AddNewUnitEnemySide(new UnitModel
                {
                    CreatureMapIsActivated = true,
                    Id = 2,
                    Pos = i,
                    LockedEmotion = true,
                    DeckActionType = DeckActionType.Change,
                    CustomDialog = true,
                    DialogId = 2,
                    CardsId = UnitUtilities.GetSamuraiCardsId()
                });
            }
            UnitUtilities.RefreshCombatUI(true);
        }
        private void CheckPhase()
        {
            if (BattleObjectManager.instance.GetAliveList(Faction.Enemy).Count > 0) return;
            _phase++;
            if (_phase >= 2) return;
            MainEnemySetNewPhase();
            SubUnitSummon();
            AudioUtilities.ChangeEnemyTeamTheme("Hornet");
            _dlgManager.DestroyAndInit(4, 8);
            _hodUnitModel.view.DisplayDlg(DialogType.SPECIAL_EVENT, "0");
        }
        public override bool IsStageFinishable() => _finished;
        public global::MapManager InitCustomMap()
        {
            var mapManager = MapUtilities.PrepareMapComponent(new MapModel
            {
                Name = "RedHood",
                Stage = "OldSamurai",
                ArtworkBG = "OldSamurai_Background",
                ArtworkFloor = "OldSamurai_Floor",
                BgFx = 0.5f,
                BgFy = 0.2f,
                FloorFx = 0.5f,
                FloorFy = 0.2f,
                BgmName = "Reflection",
                StageType = Models.StageType.CreatureType,
                ExtraSettings = new MapExtraSettings { MapManagerType = typeof(OldSamuraiMapManager) }
            });
            SingletonBehavior<BattleSceneRoot>.Instance.InitInvitationMap(mapManager);
            return mapManager;
        }
    }
}

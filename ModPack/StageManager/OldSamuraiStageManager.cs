using System.Linq;
using System.Threading.Tasks;
using CustomMapUtility;
using LOR_XML;
using ModPack21341.Characters;
using ModPack21341.Harmony;
using ModPack21341.Models;
using ModPack21341.StageManager.MapManager.OldSamuraiStageMaps;
using ModPack21341.Utilities;

namespace ModPack21341.StageManager
{
    public class EnemyTeamStageManager_OldSamurai : EnemyTeamStageManager
    {
        private OldSamuraiMapManager _mapManager;
        private BattleUnitModel _mainEnemyModel;
        private BattleUnitModel _hodUnitModel;
        private int _phase;
        private bool _finished;
        private Task _changeBgm;
        public override void OnWaveStart()
        {
            CustomMapHandler.InitCustomMap("OldSamurai", new OldSamuraiMapManager(),false,true,0.5f,0.2f);
            CustomMapHandler.EnforceMap();
            Singleton<StageController>.Instance.CheckMapChange();
            _mapManager = SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject as OldSamuraiMapManager;
            _mapManager?.InitDlg(0,4);
            UnitStarterSet();
            _phase = 0;
        }

        private void UnitStarterSet()
        {
            _hodUnitModel = BattleObjectManager.instance.GetList(Faction.Player).FirstOrDefault();
            var passive = _hodUnitModel?.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 7));
            passive?.Hide();
            passive?.OnWaveStart();
            _mainEnemyModel = BattleObjectManager.instance.GetList(Faction.Enemy).FirstOrDefault();
        }
        public override void OnRoundEndTheLast() => CheckPhase();

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
            CustomMapHandler.EnforceMap();
            MapUtilities.CheckAndChangeBGM(ref _changeBgm);
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
                    Id = 2,
                    Pos = i,
                    LockedEmotion = true
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
            MapUtilities.PrepareChangeBGM("Hornet.mp3", ref _changeBgm);
            _mapManager.ChangeDlgPhase(4, 8);
            _hodUnitModel.view.DisplayDlg(DialogType.SPECIAL_EVENT, "0");
        }
        public override bool IsStageFinishable() => _finished;
    }
}

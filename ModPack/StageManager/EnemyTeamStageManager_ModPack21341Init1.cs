using System.Linq;
using System.Threading.Tasks;
using LOR_XML;
using ModPack21341.Characters.OldSamurai.PassiveAbilities;
using ModPack21341.Harmony;
using ModPack21341.Models;
using ModPack21341.StageManager.MapManager.OldSamuraiStageMaps;
using ModPack21341.Utilities;
using ModPack21341.Utilities.CustomMapUtility.Assemblies;

namespace ModPack21341.StageManager
{
    //OldSamurai
    public class EnemyTeamStageManager_ModPack21341Init1 : EnemyTeamStageManager
    {
        private Task _changeBgm;
        private bool _finished;
        private BattleUnitModel _hodUnitModel;
        private BattleUnitModel _mainEnemyModel;
        private ModPack21341InitOldSamuraiMapManager _mapManager;
        private int _phase;

        public override void OnWaveStart()
        {
            UnitUtilities.TestingUnitValues();
            CustomMapHandler.InitCustomMap("OldSamurai", new ModPack21341InitOldSamuraiMapManager(), false, true, 0.5f,
                0.2f);
            CustomMapHandler.EnforceMap();
            Singleton<StageController>.Instance.CheckMapChange();
            _mapManager =
                SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject as ModPack21341InitOldSamuraiMapManager;
            _mapManager?.InitDlg(0, 4);
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

        public override void OnRoundEndTheLast()
        {
            CheckPhase();
        }

        public void SetFinishable()
        {
            _finished = true;
        }

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
                unit.moveDetail.ReturnToFormationByBlink(true);
                unit.view.EnableView(true);
                unit.view.CreateSkin();
            }
        }

        public override void OnRoundStart()
        {
            CustomMapHandler.EnforceMap();
            MapUtilities.CheckAndChangeBgm(ref _changeBgm);
            CheckSubUnit();
        }

        private void MainEnemySetNewPhase()
        {
            var enemyPassive =
                _mainEnemyModel?.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init44) as
                    PassiveAbility_ModPack21341Init44;
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
                UnitUtilities.AddNewUnitEnemySide(new UnitModel
                {
                    Id = 2,
                    Pos = i,
                    LockedEmotion = true
                });
            UnitUtilities.RefreshCombatUI(true);
        }

        private void CheckPhase()
        {
            if (BattleObjectManager.instance.GetAliveList(Faction.Enemy).Count > 0) return;
            _phase++;
            if (_phase >= 2) return;
            MainEnemySetNewPhase();
            SubUnitSummon();
            MapUtilities.PrepareChangeBgm("Hornet.mp3", ref _changeBgm);
            _mapManager.ChangeDlgPhase(4, 8);
            _hodUnitModel.view.DisplayDlg(DialogType.SPECIAL_EVENT, "0");
        }

        public override bool IsStageFinishable()
        {
            return _finished;
        }
    }
}